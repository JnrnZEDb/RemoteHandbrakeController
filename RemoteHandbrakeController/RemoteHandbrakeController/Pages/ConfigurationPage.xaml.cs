﻿using System;
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
		XMLConfig xmlConfig;
		/// <summary> Constructor </summary>
		/// <param name="p"></param>
		public ConfigurationPage(XMLConfig config)
		{
			xmlConfig = config;
			InitializeComponent();
			DataContext = config;
		}

		#region BUTTON_CLICKS
		/// <summary> Apply Button - saves settings </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnApplyConfig_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Globals.SaveConfig(Globals.CONFIG_NAME, xmlConfig);
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
			NavigationService.GoBack();
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
				xmlConfig.LocalHandbrakeCLIPath = txtHandbrakeCLI_Path.Text;
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
					xmlConfig.LocalOutputDirectory = txtLocalOutput.Text;
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
				using (var testClient = new SshClient(xmlConfig.PlexIP, xmlConfig.Username, password))
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
