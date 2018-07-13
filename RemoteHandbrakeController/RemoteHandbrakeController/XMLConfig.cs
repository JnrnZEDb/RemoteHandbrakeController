using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHandbrakeController
{
	[Serializable]
	public class XMLConfig
	{
		private bool _localWindowsMode;
		/// <summary> Sets if it is running in local mode </summary>
		public bool LocalWindowsMode
		{
			get { return _localWindowsMode; }
			set { _localWindowsMode = value; }
		}

		private bool _pingTestMode;
		/// <summary> Sets if it is running in test mode which will
		/// send pings out instead of the Handbrake command.
		/// </summary>
		public bool PingTestMode
		{
			get { return _pingTestMode; }
			set { _pingTestMode = value; }
		}

		private string _username;
		/// <summary> Plex Server username </summary>
		public string Username
		{
			get { return _username; }
			set { _username = value; }
		}

		private string _plexIP;
		/// <summary> IP of Plex/Encoding Server </summary>
		public string PlexIP
		{
			get { return _plexIP; }
			set { _plexIP = value; }
		}

		private string _outputDirectory;
		/// <summary> Encoding output file path </summary>
		public string OutputDirectory
		{
			get { return _outputDirectory; }
			set { _outputDirectory = value; }
		}

		private string _localOutputDirectory;
		/// <summary> Local encoding output file path </summary>
		public string LocalOutputDirectory
		{
			get { return _localOutputDirectory; }
			set { _localOutputDirectory = value; }
		}

		private string _localHandbrakeCLIPath;
		/// <summary> Path of local Windows HandbrakeCLI executable </summary>
		public string LocalHandbrakeCLIPath
		{
			get { return _localHandbrakeCLIPath; }
			set { _localHandbrakeCLIPath = value; }
		}

		private string _remoteInputDirectory;
		/// <summary> Remote view of input directories </summary>
		public string RemoteInputDirectory
		{
			get { return _remoteInputDirectory; }
			set { _remoteInputDirectory = value; }
		}

		/// <summary> List of input directories to scan from </summary>
		public List<InputDirectory> InputDirectories { get; set; }

		/// <summary> Default Constructor </summary>
		public XMLConfig() : base()
		{
			_username = "dshinevar";
			_plexIP = "10.0.0.2";
			_outputDirectory = "/home/Media";
			_localHandbrakeCLIPath = @"C:\Program Files (x86)\";
			_localOutputDirectory = @"E:\Libraries\Videos";
			_remoteInputDirectory = @"/media/windowsmedia";
			_localWindowsMode = false;
			_pingTestMode = false;
			InputDirectories = new List<InputDirectory>();
		}
	}

	/// <summary> Config class for input directories </summary>
	public class InputDirectory
	{
		public InputDirectory()
		{ }

		/// <summary> Constructor </summary>
		public InputDirectory(string strName, string strDirectoryPath)
		{
			_name = strName;
			_directoryPath = strDirectoryPath;
		}

		private string _name;
		/// <summary> Name for identification</summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private string _directoryPath;
		/// <summary> Full directory path to look for input files </summary>
		public string DirectoryPath
		{
			get { return _directoryPath; }
			set { _directoryPath = value; }
		}
	}


}
