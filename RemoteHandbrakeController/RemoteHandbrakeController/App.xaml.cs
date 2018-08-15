using System;
using System.Diagnostics;
using System.Windows;

namespace RemoteHandbrakeController
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
			MainWindow mainWnd = new MainWindow();
			mainWnd.Show();
		}

		private static void OnProcessExit(object sender, EventArgs e)
		{
			// Clear out any still running HandbrakeCLI processes
			Process[] lstProcHandbrake = Process.GetProcessesByName("HandbrakeCLI");
			if (lstProcHandbrake.Length > 0)
			{
				foreach (Process p in lstProcHandbrake)
				{
					p.Kill();
					p.Dispose();
				}
			}
		}
	}
}
