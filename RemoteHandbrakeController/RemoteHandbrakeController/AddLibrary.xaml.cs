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
	/// Interaction logic for AddLibrary.xaml
	/// </summary>
	public partial class AddLibrary : Window
	{
		public string LibraryName { get; set; }
		public string Path { get; set; }

		public AddLibrary()
		{
			InitializeComponent();
		}

		#region BUTTON_CLICKS
		/// <summary> Opens browse dialog </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnPathBrowse_Click(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				System.Windows.Forms.DialogResult dlgResult = dialog.ShowDialog();
				if (dlgResult == System.Windows.Forms.DialogResult.OK)
				{
					txtPath.Text = dialog.SelectedPath;
				}
			}
		}

		/// <summary> Saves entered info </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			if ((txtName.Text != string.Empty) && (txtPath.Text != string.Empty))
			{
				LibraryName = txtName.Text;
				Path = txtPath.Text;
				DialogResult = true;
				this.Close();
			}
			else
			{
				MessageBox.Show("One of the fields is empty.", "Empty field");
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			Name = null;
			Path = null;
			DialogResult = false;
			this.Close();
		}
		#endregion
	}
}
