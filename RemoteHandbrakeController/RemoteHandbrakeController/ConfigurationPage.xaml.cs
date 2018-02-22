using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


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
	}
}
