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
	/// Interaction logic for EncodeWindow.xaml
	/// </summary>
	public partial class EncodeWindow : Window
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
		/// Shows connection status
		/// </summary>
		private bool IsConnected
		{
			get { return IsConnected; }
			set
			{
				switch (value)
				{
					case true:
						txtConnectionStatus.Text = String.Format("CONNECTED TO {0}", Properties.Settings.Default.PLEX_IP);
						break;
					case false:
						txtConnectionStatus.Text = String.Format("NOT CONNECTED TO {0}", Properties.Settings.Default.PLEX_IP);
						break;
				}	
			}
		}
		private SshClient client = new SshClient(Properties.Settings.Default.PLEX_IP, Properties.Settings.Default.USERNAME, Properties.Settings.Default.PASSWORD);

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="lstFiles"></param>
		public EncodeWindow(List<FileInfo> lstFiles)
		{
			lstFilesToEncode = new ObservableCollection<FileInfo>(lstFiles);
			InitializeComponent();
			this.DataContext = this;
			ConnectToServer();
		}

		/// <summary>
		/// Connects to server
		/// </summary>
		private void ConnectToServer()
		{
			try
			{
				client.Connect();
				IsConnected = true;
			}
			catch (Exception ex)
			{
				IsConnected = false;
				MessageBox.Show(String.Format("Failed to connect to {0} | {1}", Properties.Settings.Default.PLEX_IP, ex.Message), "FAILED TO CONNECT", MessageBoxButton.OK);
				return;
			}
			
		}

		/// <summary>
		/// Disconnects from server
		/// </summary>
		private void DisconnectFromServer()
		{
			try
			{
				client.Disconnect();
				IsConnected = false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(String.Format("{0}", ex.Message), "ALREADY DISCONNECTED", MessageBoxButton.OK);
			}	
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
				var cmd = client.CreateCommand(command);
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
			for (int i = 0; i < lstFilesToEncode.Count - 1;)
			{
				Dispatcher.BeginInvoke(new Action(delegate
				{
					txtCurrentFile.Text = String.Format("CURRENTLY ENCODING {0}", lstFilesToEncode[i].Name);
				}));
				HandbrakeCommand cmd = new HandbrakeCommand(lstFilesToEncode[i].FullName, Properties.Settings.Default.OUTPUT_DIRECTORY);
				//DoCommand(cmd.ToString());
				if (!DoCommand("ping 192.168.1.10 -c 5"))
				{
					Dispatcher.BeginInvoke(new Action(delegate
					{
						txtOutput.Text += String.Format("FAILED TO ENCODE {0} | ABORTING", lstFilesToEncode[i].Name);
					}));
					break;
				}
				else
				{
					Dispatcher.BeginInvoke(new Action(delegate
					{
						lstFilesToEncode.RemoveAt(i);
					}));
				}
			}
			Dispatcher.BeginInvoke(new Action(delegate
			{
				txtCurrentFile.Text = "";
			}));
		}

		private void BtnStartEncode_Click(object sender, RoutedEventArgs e)
		{
			Thread childThread = new Thread(() => EncodeFiles());
			childThread.Start();
		}

		private void mnuConnect_Click(object sender, RoutedEventArgs e)
		{
			ConnectToServer();
		}

		private void mnuDisconnect_Click(object sender, RoutedEventArgs e)
		{
			DisconnectFromServer();
		}
	}
}
