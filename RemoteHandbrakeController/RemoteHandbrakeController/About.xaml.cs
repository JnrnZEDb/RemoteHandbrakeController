using System.Windows;
using System.Reflection;

namespace RemoteHandbrakeController
{
	/// <summary>
	/// Interaction logic for About.xaml
	/// </summary>
	public partial class About : Window
	{
		private string _versionInfo;
		public string versionInfo
		{
			get { return _versionInfo; }
			set { _versionInfo = value; }
		}

		public About()
		{
			versionInfo = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			this.DataContext = this;
			InitializeComponent();
		}
	}
}
