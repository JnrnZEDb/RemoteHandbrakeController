using System.Windows;

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
