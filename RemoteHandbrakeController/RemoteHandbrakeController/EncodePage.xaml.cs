using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
		private Process procWnds;
		private bool bCurrentlyEncoding { get; set; } = false;

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

		/// <summary>
		/// Runs command and prints output to window if talking to Debian Linux
		/// </summary>
		/// <param name="command"></param>
		private bool DoLinuxCommand(string command)
		{
			Dispatcher.BeginInvoke(new Action(delegate
			{
				txtOutput.AppendText(String.Format("{0}\n", command));
			}));
			try
			{
				var cmd = Globals.client.CreateCommand(command);
				var asynch = cmd.BeginExecute();
				var reader = new StreamReader(cmd.OutputStream);

				while (!asynch.IsCompleted)
				{
					var result = reader.ReadToEnd();
					if (string.IsNullOrEmpty(result)) continue;
					Dispatcher.BeginInvoke(new Action(delegate
					{
						txtOutput.AppendText(result);
					}));
				}
				cmd.EndExecute(asynch);
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
				//p.StartInfo.FileName = "cmd.exe";
				procWnds.StartInfo.FileName = "C:\\Program Files (x86)\\HandbrakeCLI\\HandbrakeCLI.exe";
				//p.StartInfo.Arguments = "/C ping 192.168.1.12";
				procWnds.StartInfo.Arguments = arguments;
				procWnds.StartInfo.UseShellExecute = false;
				procWnds.StartInfo.RedirectStandardOutput = true;
				procWnds.StartInfo.RedirectStandardInput = true;
				procWnds.StartInfo.CreateNoWindow = true;
				procWnds.OutputDataReceived += (sender, args) => Dispatcher.BeginInvoke(new Action(delegate
				{
					txtOutput.AppendText(string.Format("{0}\n", args.Data));
					txtOutput.ScrollToEnd();
				}));

				procWnds.Start();
				procWnds.BeginOutputReadLine();
				procWnds.WaitForExit();
				procWnds = null;
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("ERROR: {0}", ex.Message), "ERROR", MessageBoxButton.OK);
				return false;
			}

		}

		/// <summary>
		/// Goes through list of files and sends encode command
		/// </summary>
		private void EncodeFiles()
		{
			bCurrentlyEncoding = true;
			for (int i = 0; i < lstFilesToEncode.Count;)
			{
				Globals.currentFileBeingEncoded = lstFilesToEncode[i].Name;
				HandbrakeCommand cmd = new HandbrakeCommand(lstFilesToEncode[i].FullName, BuildOutputString(lstFilesToEncode[i]));
				// WINDOWS MODE
				if (Properties.Settings.Default.LOCAL_WINDOWS_MODE)
				{
					//DoCommand(cmd.ToString());
					if (!DoWindowsCommand(cmd.ToString()))
					{
						Dispatcher.BeginInvoke(new Action(delegate
						{
							txtOutput.AppendText(String.Format("FAILED TO ENCODE {0} | ABORTING", lstFilesToEncode[i].Name));
							txtOutput.ScrollToEnd();
						}));
						break;
					}
					else
					{
						DispatcherOperation disOp = Dispatcher.BeginInvoke(new Action(delegate
						{
							lstFilesToEncode.RemoveAt(i);
						}));
						while (disOp.Status != DispatcherOperationStatus.Completed) ;
					}

				}
				// LINUX MODE
				else
				{
					//DoCommand(cmd.ToString());
					if (!DoLinuxCommand("ping 192.168.1.12 -c 5"))
					{
						Dispatcher.BeginInvoke(new Action(delegate
						{
							txtOutput.AppendText(String.Format("FAILED TO ENCODE {0} | ABORTING", lstFilesToEncode[i].Name));
							txtOutput.ScrollToEnd();
						}));
						break;
					}
					else
					{
						DispatcherOperation disOp = Dispatcher.BeginInvoke(new Action(delegate
						{
							lstFilesToEncode.RemoveAt(i);
						}));
						while (disOp.Status != DispatcherOperationStatus.Completed) ;
					}
				}
				
			}
			Globals.currentFileBeingEncoded = String.Empty;
			bCurrentlyEncoding = false;
		}

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
					outputDir = Properties.Settings.Default.LOCAL_OUTPUT + "\\Movies (Encoded)\\" + inputFile.Name;
				}
				else if (inputFile.FullName.Contains("TV Shows"))
				{
					string[] folders = inputFile.FullName.Split('\\');
					outputDir = Properties.Settings.Default.LOCAL_OUTPUT + "\\TV Shows (Encoded)\\" + folders[folders.Length - 3] + "\\" + folders[folders.Length - 2];
					Directory.CreateDirectory(outputDir);
				}
				else if (inputFile.FullName.Contains("Anime"))
				{
					string[] folders = inputFile.FullName.Split('\\');
					outputDir = Properties.Settings.Default.LOCAL_OUTPUT + "\\Anime (Encoded)\\" + folders[folders.Length - 3] + "\\" + folders[folders.Length - 2];
					Directory.CreateDirectory(outputDir);
				}

			}
			// Linux Build
			else
			{

			}
			return outputDir;
		}

		private void BtnStartStopEncode_Click(object sender, RoutedEventArgs e)
		{
			if (!bCurrentlyEncoding)
			{
				btnStartStopEncode.Content = "STOP";
				Thread childThread = new Thread(() => EncodeFiles());
				childThread.Start();
			}
			else if (bCurrentlyEncoding)
			{
				btnStartStopEncode.Content = "START";
				procWnds.Kill();
				procWnds = null;
				txtOutput.AppendText("ENCODING CANCELLED BY USER\n");
				txtOutput.ScrollToEnd();
			}
			
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			var response = MessageBox.Show("Are you sure you want to cancel?", "CANCEL", MessageBoxButton.YesNo);
			if (response == MessageBoxResult.Yes)
			{
				Globals.DisconnectFromServer(Globals.client);
				MediaSelectionPage pageMediaSelection = new MediaSelectionPage();
				if (procWnds != null)
				{
					procWnds.Kill();
					procWnds = null;
				}
				NavigationService.Navigate(pageMediaSelection);
			}
		}
	}
}
