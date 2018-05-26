using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Renci.SshNet;


namespace RemoteHandbrakeController
{
	/// <summary>
	/// Interaction logic for ConfigurationPage.xaml
	/// </summary>
	public partial class ConfigurationPage : Page
	{
		private Page pagePrevious { get; set; }

		public ConfigurationPage(Page p)
		{
			pagePrevious = p;
			InitializeComponent();
		}

		#region BUTTON_CLICKS
		private void btnApplyConfig_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Properties.Settings.Default.Save();
				Properties.Settings.Default.Reload();
			}
			catch
			{
				MessageBox.Show("Config Settings Failed To Save", "CONFIG SAVED", MessageBoxButton.OK);
				return;
			}
			MessageBox.Show("Config Settings Saved Succesfully", "CONFIG SAVED", MessageBoxButton.OK);
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(pagePrevious);
		}

		private void btnHandbrakeCLI_Path_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog pathDialog = new OpenFileDialog();
			pathDialog.Filter = "Executable (*.exe) | *.exe";
			pathDialog.InitialDirectory = @"C:\";
			var result = pathDialog.ShowDialog();

			if (result == true) txtHandbrakeCLI_Path.Text = pathDialog.FileName;
		}

		private void btnTestIP_Click(object sender, RoutedEventArgs e)
		{
			string password = string.Empty;
			PasswordPrompt passPrompt = new PasswordPrompt();
			if (passPrompt.ShowDialog().Value)
			{
				password = passPrompt.strPassword;
			}
			else
			{
				passPrompt = null;
				return;
			}
			passPrompt = null;
			try
			{
				using (var testClient = new SshClient(Properties.Settings.Default.PLEX_IP, Properties.Settings.Default.USERNAME, password))
				{
					testClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(10);
					testClient.Connect();
					if (testClient.IsConnected)
					{
						MessageBox.Show("Connection was successful", "SUCCESS", MessageBoxButton.OK);
						testClient.Disconnect();
						return;
					}
					else
					{
						MessageBox.Show(string.Format("Test Connection Failed"), "CONNECTION FAILED", MessageBoxButton.OK);
						return;
					}

				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("ERROR \n Test Connection Failed: {0}", ex.Message), "ERROR (CONNECTION FAILED)", MessageBoxButton.OK);
			}
			
		}
		#endregion
	}
}
