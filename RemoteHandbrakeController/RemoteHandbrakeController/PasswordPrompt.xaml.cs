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
	/// Interaction logic for PasswordPrompt.xaml
	/// </summary>
	public partial class PasswordPrompt : Window
	{
		public string strPassword { get; set; }

		public PasswordPrompt()
		{
			InitializeComponent();
		}

		private void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			if (passwordBox.Password != string.Empty)
			{
				strPassword = passwordBox.Password;
				this.DialogResult = true;
				this.Close();
			}
			else
			{
				MessageBox.Show("Please enter a password.", "EMPTY PASSWORD", MessageBoxButton.OK);
			}

		}
	}
}
