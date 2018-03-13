using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Diagnostics;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace RemoteHandbrakeController
{
	/// <summary>
	/// Interaction logic for EncodePage.xaml
	/// </summary>
	public partial class EncodePage : Page
	{
		private bool bCurrentlyEncoding { get; set; } = false;
		private bool bStopEncodePressed { get; set; } = false;
		private Process procWnds;
		private SshCommand cmdCurrent;
		private BackgroundWorker workerEncode;

		private ObservableCollection<FileInfo> _lstFilesToEncode;
		public ObservableCollection<FileInfo> lstFilesToEncode
		{
			get
			{
				return _lstFilesToEncode;
			}
			set
			{
				_lstFilesToEncode = value;
			}
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="lstFiles"></param>
		public EncodePage(List<FileInfo> lstFiles)
		{
			lstFilesToEncode = new ObservableCollection<FileInfo>(lstFiles);
			InitializeComponent();
			this.DataContext = this;
			Globals.ConnectToServer(Globals.client);
		}

		#region DO_COMMANDS
		/// <summary>
		/// Runs command and prints output to window if talking to Debian Linux
		/// </summary>
		/// <param name="command"></param>
		private bool DoLinuxCommand(string command)
		{
			if (Properties.Settings.Default.TEST_MODE) command = "ping 192.168.1.12 -c 5";

			Dispatcher.BeginInvoke(new Action(delegate
			{
				txtOutput.AppendText(String.Format("{0}\n", command));
			}));
			try
			{
				cmdCurrent = Globals.client.CreateCommand(command);
				var asynch = cmdCurrent.BeginExecute();
				var reader = new StreamReader(cmdCurrent.OutputStream);

				while (!asynch.IsCompleted)
				{
					if (workerEncode.CancellationPending) return false;
					var result = reader.ReadToEnd();
					if (string.IsNullOrEmpty(result)) continue;
					if (result != null)	workerEncode.ReportProgress(0, result);
				}
				cmdCurrent.EndExecute(asynch);
				cmdCurrent = null;
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(String.Format("{0}", ex.Message), "ERROR", MessageBoxButton.OK);
				return false;
			}
		}

		/// <summary>
		/// Runs command and prints output to window if talking to Windows
		/// </summary>
		/// <param name="command"></param>
		private bool DoWindowsCommand(string command)
		{
			string arguments = command.Remove(0, 12);
			Dispatcher.BeginInvoke(new Action(delegate
			{
				txtOutput.AppendText(String.Format("{0}\n", command));
			}));
			try
			{
				procWnds = new Process();
				if (Properties.Settings.Default.TEST_MODE)
				{
					procWnds.StartInfo.FileName = "cmd.exe";
					procWnds.StartInfo.Arguments = "/C ping 192.168.1.12";
				}
				else
				{
					procWnds.StartInfo.FileName = Properties.Settings.Default.LOCAL_HANDBRAKECLI_PATH;
					procWnds.StartInfo.Arguments = arguments;
				}
				
				procWnds.StartInfo.UseShellExecute = false;
				procWnds.StartInfo.RedirectStandardOutput = true;
				procWnds.StartInfo.RedirectStandardInput = true;
				procWnds.StartInfo.CreateNoWindow = true;
				procWnds.OutputDataReceived += (sender, args) =>
				{
					if (args.Data != null) workerEncode.ReportProgress(0, args.Data);
				};
				procWnds.Start();
				procWnds.BeginOutputReadLine();
				while(!procWnds.HasExited)
				{
					if (workerEncode.CancellationPending)
					{
						procWnds.CancelOutputRead();
						return false;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("ERROR: {0}", ex.Message), "ERROR", MessageBoxButton.OK);
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
				HandbrakeCommand cmd = new HandbrakeCommand(lstFilesToEncode[i].FullName, BuildOutputString(lstFilesToEncode[i]));
				// WINDOWS MODE
				if (Properties.Settings.Default.LOCAL_WINDOWS_MODE)
				{
					string strCurrentFile = lstFilesToEncode[i].Name;
					if (!DoWindowsCommand(cmd.ToString()))
					{
						Dispatcher.BeginInvoke(new Action(delegate
						{
							txtOutput.AppendText(String.Format("FAILED TO ENCODE {0} | ABORTING\n", strCurrentFile));
							txtOutput.ScrollToEnd();
						}));
						break;
					}
					else
					{
						DispatcherOperation disOp = Dispatcher.BeginInvoke(new Action(delegate
						{
							txtOutput.AppendText(String.Format("{0} FINISHED ENCODING\n", strCurrentFile));
							if (lstFilesToEncode.Count > 0) lstFilesToEncode.RemoveAt(i);
							prgEncode.Value = 0;
						}));
						while (disOp.Status != DispatcherOperationStatus.Completed) ;
					}

				}
				// LINUX MODE
				else
				{
					string strCurrentFile = lstFilesToEncode[i].Name;
					if (!DoLinuxCommand(cmd.ToString()))
					{
						Dispatcher.BeginInvoke(new Action(delegate
						{
							txtOutput.AppendText(String.Format("FAILED TO ENCODE {0} | ABORTING\n", strCurrentFile));
							txtOutput.ScrollToEnd();
						}));
						break;
					}
					else
					{
						DispatcherOperation disOp = Dispatcher.BeginInvoke(new Action(delegate
						{
							txtOutput.AppendText(String.Format("{0} FINISHED ENCODING\n", strCurrentFile));
							if (lstFilesToEncode.Count > 0) lstFilesToEncode.RemoveAt(i);
							prgEncode.Value = 0;
						}));
						while (disOp.Status != DispatcherOperationStatus.Completed);
					}
				}
				if (workerEncode.CancellationPending) return;
			}
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
				int iProgress = (int)Convert.ToDouble(strProgress.Substring(0, 4));
				prgEncode.Value = iProgress;
			}
			txtOutput.AppendText(string.Format("{0}\n", strOutput));
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
			btnCancel.IsEnabled = true;
			txtOutput.AppendText("ENCODING CANCELLED BY USER\n");
			txtOutput.ScrollToEnd();
			prgEncode.Value = 0;
		}
		#endregion
		/// <summary>
		/// Builds proper output path for encoded file
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		private string BuildOutputString(FileInfo inputFile)
		{
			string outputDir = "";
			// Local Windows Build
			if (Properties.Settings.Default.LOCAL_WINDOWS_MODE)
			{
				if (inputFile.FullName.Contains("Movies"))
				{
					outputDir = Properties.Settings.Default.LOCAL_OUTPUT + "\\Movies (Encoded)\\" + inputFile.Name.Replace("mkv", "m4v");
				}
				else if (inputFile.FullName.Contains("TV Shows"))
				{
					string[] folders = inputFile.FullName.Split('\\');
					outputDir = Properties.Settings.Default.LOCAL_OUTPUT + "\\TV Shows (Encoded)\\" + folders[folders.Length - 3] + "\\" + folders[folders.Length - 2];
					Directory.CreateDirectory(outputDir);
					outputDir += "\\" + inputFile.Name.Replace("mkv", "m4v");
				}
				else if (inputFile.FullName.Contains("Anime"))
				{
					string[] folders = inputFile.FullName.Split('\\');
					outputDir = Properties.Settings.Default.LOCAL_OUTPUT + "\\Anime (Encoded)\\" + folders[folders.Length - 3] + "\\" + folders[folders.Length - 2];
					Directory.CreateDirectory(outputDir);
					outputDir += "\\" + inputFile.Name.Replace("mkv", "m4v");
				}

			}
			// Linux Build
			else
			{

			}
			return outputDir;
		}

		#region BUTTON_CLICKS
		private void BtnStartStopEncode_Click(object sender, RoutedEventArgs e)
		{
			if (lstFilesToEncode.Count != 0)
			{
				if (!bCurrentlyEncoding)
				{
					btnStartStopEncode.Content = "STOP";
					btnCancel.IsEnabled = false;
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
					btnCancel.IsEnabled = true;
					workerEncode.CancelAsync();
				}
			}	
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			var response = MessageBox.Show("Are you sure you want to cancel?", "CANCEL", MessageBoxButton.YesNo);
			if (response == MessageBoxResult.Yes)
			{
				//EndEncodeTask();
				Globals.DisconnectFromServer(Globals.client);
				MediaSelectionPage pageMediaSelection = new MediaSelectionPage();
				NavigationService.Navigate(pageMediaSelection);
			}
		}
		#endregion
	}
}
