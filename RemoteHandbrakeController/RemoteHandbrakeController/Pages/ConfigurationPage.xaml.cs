using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;


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
		#endregion
	}
}
