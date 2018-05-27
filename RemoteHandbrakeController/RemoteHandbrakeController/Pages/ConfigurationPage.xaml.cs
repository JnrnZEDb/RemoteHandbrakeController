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

		/// <summary> Constructor </summary>
		/// <param name="p"></param>
		public ConfigurationPage(Page p)
		{
			pagePrevious = p;
			InitializeComponent();
		}

		#region BUTTON_CLICKS
		/// <summary> Apply Button - saves settings </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

		/// <summary> Back Button - Goes back to last page </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(pagePrevious);
		}

		/// <summary> Sets path for HandbrakeCLI.exe locally </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnHandbrakeCLI_Path_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog pathDialog = new OpenFileDialog();
			pathDialog.Filter = "Executable (*.exe) | *.exe";
			pathDialog.InitialDirectory = @"C:\";
			var result = pathDialog.ShowDialog();

			if (result == true)
			{
				txtHandbrakeCLI_Path.Text = pathDialog.FileName;
				Properties.Settings.Default.LOCAL_HANDBRAKECLI_PATH = txtHandbrakeCLI_Path.Text;
			}
		}

		/// <summary> Sets path for video input files </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnInputDirPath_Click(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				System.Windows.Forms.DialogResult dlgResult = dialog.ShowDialog();
				if (dlgResult == System.Windows.Forms.DialogResult.OK)
				{
					txtInput.Text = dialog.SelectedPath;
					Properties.Settings.Default.INPUT_DIRECTORY = txtInput.Text;
				}
			}
		}

		/// <summary> Sets path for local video output </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnLocalOutput_Path_Click(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				System.Windows.Forms.DialogResult dlgResult = dialog.ShowDialog();
				if (dlgResult == System.Windows.Forms.DialogResult.OK)
				{
					txtLocalOutput.Text = dialog.SelectedPath;
					Properties.Settings.Default.LOCAL_OUTPUT = txtLocalOutput.Text;
				}
			}
		}

		/// <summary> Tests IP address (ping with 10s timeout) </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
						MessageBox.Show("Test Connection Failed", "CONNECTION FAILED", MessageBoxButton.OK);
						return;
					}

				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"ERROR \n Test Connection Failed: {ex.Message}", "ERROR (CONNECTION FAILED)", MessageBoxButton.OK);
			}
			
		}
		#endregion
	}
}
