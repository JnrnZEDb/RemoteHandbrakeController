using System;
using System.Collections.Generic;

namespace RemoteHandbrakeController
{
	[Serializable]
	public class XMLConfig
	{
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

		private string _remoteInputDirectory;
		/// <summary> Remote view of input directories </summary>
		public string RemoteInputDirectory
		{
			get { return _remoteInputDirectory; }
			set { _remoteInputDirectory = value; }
		}

		private string _CustomHandbrakePresetsDirectory;
		/// <summary> Location of custom handbrake presets (.json files) </summary>
		public string CustomHandbrakePresetsDirectory
		{
			get { return _CustomHandbrakePresetsDirectory; }
			set { _CustomHandbrakePresetsDirectory = value; }
		}

		/// <summary> List of input directories to scan from </summary>
		public List<InputDirectory> InputDirectories { get; set; }

		/// <summary> Default Constructor </summary>
		public XMLConfig() : base()
		{
			_username = "dshinevar";
			_plexIP = "10.0.0.2";
			_outputDirectory = "/library/video";
			_remoteInputDirectory = @"/media/windows";
			_CustomHandbrakePresetsDirectory = @"Z:\";
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
