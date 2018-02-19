using System;
using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Renci.SshNet;


namespace RemoteHandbrakeController
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		List<FileInfo> lstFilesToBeEncoded = new List<FileInfo>();

		/// <summary>
		/// Shows connection status
		/// </summary>
		public bool IsConnected
		{
			get { return IsConnected; }
			set
			{
				switch (value)
				{
					case true:
						Dispatcher.Invoke(() => txtConnectionStatus.Text = String.Format("CONNECTED TO {0}", Properties.Settings.Default.PLEX_IP));
						break;
					case false:
						Dispatcher.Invoke(() => txtConnectionStatus.Text = String.Format("NOT CONNECTED TO {0}", Properties.Settings.Default.PLEX_IP));
						break;
				}
			}
		}

		public MainWindow()
		{
			Globals.client = new SshClient(Properties.Settings.Default.PLEX_IP, Properties.Settings.Default.USERNAME, Properties.Settings.Default.PASSWORD);

			System.Timers.Timer timerStatus = new System.Timers.Timer();
			timerStatus.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			timerStatus.Interval = 2000;
			timerStatus.Enabled = true;

			MediaSelectionPage mediaSelectionPage = new MediaSelectionPage();
			InitializeComponent();
			MainFrame.Navigate(mediaSelectionPage);
		}

		private void mnuConfig_Click(object sender, RoutedEventArgs e)
		{
			Config wndConfig = new Config();
			wndConfig.Show();
		}

		private void mnuAbout_Click(object sender, RoutedEventArgs e)
		{
			About wndAbout = new About();
			wndAbout.Show();
		}

		private void mnuExit_Click(object sender, RoutedEventArgs e)
		{
			lstFilesToBeEncoded = null;
			Application.Current.Shutdown();
			return;
		}

		private void mnuConnect_Click(object sender, RoutedEventArgs e)
		{
			Globals.ConnectToServer(Globals.client);
		}

		private void mnuDisconnect_Click(object sender, RoutedEventArgs e)
		{
			Globals.DisconnectFromServer(Globals.client);
		}

		private void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			IsConnected = Globals.client.IsConnected;
			if (Globals.currentFileBeingEncoded != String.Empty) Dispatcher.Invoke(() => txtCurrentFile.Text = string.Format("CURRENTLY ENCODING {0}", Globals.currentFileBeingEncoded));
			else Dispatcher.Invoke(() => txtCurrentFile.Text = String.Empty);
		}
			
			
	}
}
