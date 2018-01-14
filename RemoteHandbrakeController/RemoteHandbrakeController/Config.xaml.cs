using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RemoteHandbrakeController
{
	/// <summary>
	/// Interaction logic for Config.xaml
	/// </summary>
	public partial class Config : Window
	{

		public Config()
		{
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
	}
}
