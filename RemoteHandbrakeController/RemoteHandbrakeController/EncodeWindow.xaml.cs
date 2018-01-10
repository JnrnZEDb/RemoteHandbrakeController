using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
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
	/// Interaction logic for EncodeWindow.xaml
	/// </summary>
	public partial class EncodeWindow : Window
	{
		private List<FileInfo> _lstFilesToEncode = new List<FileInfo>();
		public List<FileInfo> lstFilesToEncode
		{
			get
			{
				return _lstFilesToEncode;
			}
			set
			{
				_lstFilesToEncode = value;
			}
		}

		public EncodeWindow(List<FileInfo> lstFiles)
		{
			lstFilesToEncode = lstFiles;
			InitializeComponent();
			lstBoxFiles.ItemsSource = lstFilesToEncode;
		}

		private void BtnStartEncode_Click(object sender, RoutedEventArgs e)
		{
			txtOutput.Text += "Hello\n";
		}
	}
}
