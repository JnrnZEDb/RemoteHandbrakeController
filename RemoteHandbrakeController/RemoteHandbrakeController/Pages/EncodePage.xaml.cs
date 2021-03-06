﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Diagnostics;
using Renci.SshNet;


namespace RemoteHandbrakeController
{
	/// <summary>
	/// Interaction logic for EncodePage.xaml
	/// </summary>
	public partial class EncodePage : Page
	{
		private bool bCurrentlyEncoding { get; set; } = false;
		private bool bStopEncodePressed { get; set; } = false;
		private string strPassword { get; set; } = string.Empty;
		private Process procWnds;
		private SshCommand cmdCurrent;
		private BackgroundWorker workerEncode;
		private XMLConfig xmlConfig;
		private MediaSelectionPage mediaSelectionPage;
		private Preset selectedPreset;

		private ObservableCollection<FileInfo> _lstFilesToEncode;
		public ObservableCollection<FileInfo> lstFilesToEncode
		{
			get { return _lstFilesToEncode;	}
			set { _lstFilesToEncode = value; }
		}

		public ObservableCollection<Preset> lstPresets { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="lstFiles"></param>
		public EncodePage(List<FileInfo> lstFiles, XMLConfig config, MediaSelectionPage mediaPage)
		{
			lstFilesToEncode = new ObservableCollection<FileInfo>(lstFiles);
			lstPresets = new ObservableCollection<Preset>();
			mediaSelectionPage = mediaPage;
			xmlConfig = config;

			foreach (Presets preset in Enum.GetValues(typeof(Presets)))
			{
				lstPresets.Add(new Preset(preset.GetDescription(), false));
			}
			string[] customPresets = Directory.GetFiles(xmlConfig.CustomHandbrakePresetsDirectory, "*.json");
			foreach (string preset in customPresets)
			{
				lstPresets.Add(new Preset(preset.Remove(0, xmlConfig.CustomHandbrakePresetsDirectory.Length), true));
			}

			InitializeComponent();
			this.DataContext = this;
		}

		#region DO_COMMANDS
		/// <summary>
		/// Runs command and prints output to window if talking to Debian Linux
		/// </summary>
		/// <param name="command"></param>
		private bool DoLinuxCommand(string command, SshClient linuxClient)
		{
			if (xmlConfig.PingTestMode) command = $"ping {xmlConfig.PlexIP} -c 5";

			Dispatcher.BeginInvoke(new Action(delegate
			{
				txtOutput.AppendText($"{command}\n");
			}));
			try
			{
				cmdCurrent = linuxClient.CreateCommand(command);
				var asynch = cmdCurrent.BeginExecute();
				var reader = new StreamReader(cmdCurrent.OutputStream);

				while (!asynch.IsCompleted)
				{
					if (workerEncode.CancellationPending) return false;
					var result = reader.ReadLine();
					if (string.IsNullOrEmpty(result)) continue;
					if (result != null) workerEncode.ReportProgress(0, result);
					reader.DiscardBufferedData();
					Thread.Sleep(750);
				}
				cmdCurrent.EndExecute(asynch);
				cmdCurrent = null;
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex.Message}", "ERROR", MessageBoxButton.OK);
				return false;
			}
		}
		#endregion

		#region WORKER_FUNCTIONS
		/// <summary>
		/// Goes through list of files and sends encode command
		/// </summary>
		private void worker_DoEncode(object sender, DoWorkEventArgs e)
		{
			bCurrentlyEncoding = true;
			for (int i = 0; i < lstFilesToEncode.Count;)
			{
				Globals.currentFileBeingEncoded = lstFilesToEncode[i].Name;
				bool bImport = selectedPreset.IsImport;
				string strPreset;
				if (bImport)
				{
					strPreset = $"{xmlConfig.OutputDirectory}/{selectedPreset.PresetName}";
				}
				else
				{
					strPreset = selectedPreset.PresetName;
				}			 
				HandbrakeCommand cmd = new HandbrakeCommand(Globals.BuildInputString(lstFilesToEncode[i], xmlConfig), Globals.BuildOutputString(lstFilesToEncode[i], xmlConfig), strPreset, bImport);
				try
				{
					using (var client = new SshClient(xmlConfig.PlexIP, xmlConfig.Username, strPassword))
					{
						client.Connect();
						string strCurrentFile = lstFilesToEncode[i].Name;
						if (!DoLinuxCommand(cmd.ToString(), client))
						{
							Dispatcher.BeginInvoke(new Action(delegate
							{
								txtOutput.AppendText($"FAILED TO ENCODE {strCurrentFile} | ABORTING\n");
								txtOutput.ScrollToEnd();
							}));
							break;
						}
						else
						{
							DispatcherOperation disOp = Dispatcher.BeginInvoke(new Action(delegate
							{
								txtOutput.AppendText($"{strCurrentFile} FINISHED ENCODING\n");
								if (lstFilesToEncode.Count > 0) lstFilesToEncode.RemoveAt(i);
								prgEncode.Value = 0;
							}));
							while (disOp.Status != DispatcherOperationStatus.Completed) ;
						}
						client.Disconnect();
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show($"ERROR: {ex}", "ERROR", MessageBoxButton.OK);
				}
				if (workerEncode.CancellationPending) return;
			}
			return;
		}

		/// <summary> Updates Progress </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			string strOutput = (string)e.UserState;
			if (Regex.Match(strOutput, @"\d+(?:\.\d+) %").Success)
			{
				string strProgress = Regex.Match(strOutput, @"\d+(?:\.\d+) %").Value;
				double iProgress = Convert.ToDouble(strProgress.Substring(0, 4));
				prgEncode.Value = iProgress;
			}
			txtOutput.AppendText($"{strOutput}\n");
			txtOutput.ScrollToEnd();
		}

		/// <summary> Runs when background worker completes </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void worker_EncodeCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (cmdCurrent != null)
			{
				cmdCurrent.CancelAsync();
				cmdCurrent = null;
			}
			if (procWnds != null)
			{
				if (!procWnds.HasExited) procWnds.Kill();
				procWnds = null;
			}
			Globals.currentFileBeingEncoded = String.Empty;
			lstFilesToEncode.Clear();
			bCurrentlyEncoding = false;
			btnStartStopEncode.Content = "START";
			btnBack.IsEnabled = true;
			comboPresets.IsEnabled = true;
			if (workerEncode.CancellationPending) txtOutput.AppendText("ENCODING CANCELLED BY USER\n");
			txtOutput.ScrollToEnd();
			prgEncode.Value = 0;
		}
		#endregion
		

		#region BUTTON_CLICKS
		private void BtnStartStopEncode_Click(object sender, RoutedEventArgs e)
		{
			if (lstFilesToEncode.Count != 0)
			{
				if (!bCurrentlyEncoding)
				{

					strPassword = string.Empty;
					PasswordPrompt passPrompt = new PasswordPrompt();
					if (passPrompt.ShowDialog().Value)
					{
						strPassword = passPrompt.strPassword;
					}
					else
					{
						passPrompt = null;
						return;
					}
					passPrompt = null;

					selectedPreset = (Preset)comboPresets.SelectedValue;
					comboPresets.IsEnabled = false;

					btnStartStopEncode.Content = "STOP";
					btnBack.IsEnabled = false;
					workerEncode = new BackgroundWorker();
					workerEncode.WorkerReportsProgress = true;
					workerEncode.WorkerSupportsCancellation = true;
					workerEncode.DoWork += worker_DoEncode;
					workerEncode.ProgressChanged += worker_ProgressChanged;
					workerEncode.RunWorkerCompleted += worker_EncodeCompleted;
					workerEncode.RunWorkerAsync();
				}
				else if (bCurrentlyEncoding)
				{
					btnBack.IsEnabled = true;
					comboPresets.IsEnabled = true;
					workerEncode.CancelAsync();
				}
			}	
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			if (lstFilesToEncode.Count > 0)
			{
				var response = MessageBox.Show("Are you sure you want to cancel?", "CANCEL", MessageBoxButton.YesNo);
				if (response == MessageBoxResult.Yes)
				{
					NavigationService.Navigate(mediaSelectionPage);
				}
			}
			else
			{
				NavigationService.Navigate(mediaSelectionPage);
			}
		}
	#endregion
	}

	/// <summary> Preset class </summary>
	public class Preset
	{
		public Preset(String presetName, bool bIsImport)
		{
			PresetName = presetName;
			IsImport = bIsImport;
		}

		public String PresetName { get; set; }
		public bool IsImport { get; set; }
	}
}
