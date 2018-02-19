using System;
using System.Configuration;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace RemoteHandbrakeController
{
	/// <summary>
	/// Interaction logic for EncodePage.xaml
	/// </summary>
	public partial class EncodePage : Page
	{
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
		/// Runs command and prints output to window
		/// </summary>
		/// <param name="command"></param>
		private bool DoCommand(string command)
		{
			Dispatcher.BeginInvoke(new Action(delegate
			{
				txtOutput.Text += String.Format("{0}\n", command);
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
						txtOutput.Text += result;
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
		/// Goes through list of files and sends encode command
		/// </summary>
		private void EncodeFiles()
		{
			for (int i = 0; i < lstFilesToEncode.Count;)
			{
				Globals.currentFileBeingEncoded = lstFilesToEncode[i].Name;
				HandbrakeCommand cmd = new HandbrakeCommand(lstFilesToEncode[i].FullName, BuildOutputString(lstFilesToEncode[i]));
				//DoCommand(cmd.ToString());
				if (!DoCommand("ping 192.168.1.12 -c 5"))
				{
					Dispatcher.BeginInvoke(new Action(delegate
					{
						txtOutput.Text += String.Format("FAILED TO ENCODE {0} | ABORTING", lstFilesToEncode[i].Name);
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
			Globals.currentFileBeingEncoded = String.Empty;
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
					outputDir = Properties.Settings.Default.LOCAL_OUTPUT + "\\Movies (Encoded)\\";
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

		private void BtnStartEncode_Click(object sender, RoutedEventArgs e)
		{
			Thread childThread = new Thread(() => EncodeFiles());
			childThread.Start();
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			var response = MessageBox.Show("Are you sure you want to cancel?", "CANCEL", MessageBoxButton.YesNo);
			if (response == MessageBoxResult.Yes)
			{
				Globals.DisconnectFromServer(Globals.client);
				MediaSelectionPage pageMediaSelection = new MediaSelectionPage();
				NavigationService.Navigate(pageMediaSelection);
			}
		}
	}
}
