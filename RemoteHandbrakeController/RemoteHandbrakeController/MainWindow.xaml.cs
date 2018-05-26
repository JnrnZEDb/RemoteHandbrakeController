using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		/// <summary> Constructor</summary>
		public MainWindow()
		{
			System.Timers.Timer timerStatus = new System.Timers.Timer();
			timerStatus.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			timerStatus.Interval = 1500;
			timerStatus.Enabled = true;

			MediaSelectionPage mediaSelectionPage = new MediaSelectionPage();
			InitializeComponent();
			MainFrame.Navigate(mediaSelectionPage);
		}

		#region MENU_CLICKS
		private void mnuConfig_Click(object sender, RoutedEventArgs e)
		{
			ConfigurationPage pageConfig = new ConfigurationPage((Page)MainFrame.Content);
			MainFrame.Navigate(pageConfig);
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
		#endregion

		/// <summary>
		/// Status Timer Event
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			if (Globals.currentFileBeingEncoded != String.Empty) Dispatcher.Invoke(() => txtCurrentFile.Text = string.Format("CURRENTLY ENCODING {0}", Globals.currentFileBeingEncoded));
			else Dispatcher.Invoke(() => txtCurrentFile.Text = String.Empty);

			if (Properties.Settings.Default.LOCAL_WINDOWS_MODE) Dispatcher.Invoke(() => txtMode.Text = "LOCAL WINDOWS MODE");
			else Dispatcher.Invoke(() => txtMode.Text = "REMOTE LINUX MODE");

			if (Properties.Settings.Default.TEST_MODE) Dispatcher.Invoke(() => txtTestMode.Text = "TEST MODE");
			else Dispatcher.Invoke(() => txtTestMode.Text = "ENCODE MODE");
		}
					
	}
}
