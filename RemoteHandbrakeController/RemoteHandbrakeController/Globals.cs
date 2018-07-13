using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RemoteHandbrakeController
{
	public static class Globals
	{
		public static string currentFileBeingEncoded { get; set; } = string.Empty;

		public static readonly string LINUX_PLEX_PRESET = "/var/lib/handbrakecli/PlexHandbrake.json";
		public static readonly string WINDOWS_PLEX_PRESET = @"E:\Libraries\Documents\Plex Presets\PlexHandbrake.json";
		public static readonly string CONFIG_NAME = $@"{System.Environment.CurrentDirectory}\RemoteHandbrakeController_Config.xml";

		/// <summary>
		/// Saves config file through XML Serializer
		/// </summary>
		/// <param name="strFile"> Full path and filename to be saved </param>
		/// <param name="xmlConfig"> The Config file object </param>
		/// <returns></returns>
		public static bool SaveConfig(string strFile, XMLConfig xmlConfig)
		{
			try
			{
				XmlSerializer xs = new XmlSerializer(xmlConfig.GetType());
				StreamWriter writer = File.CreateText(strFile);
				xs.Serialize(writer, xmlConfig);
				writer.Flush();
				writer.Close();
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
			
		}

		/// <summary>
		/// Loads config file through XML Serializer
		/// </summary>
		/// <param name="strFile"></param>
		/// <returns></returns>
		public static XMLConfig LoadConfig(string strFile)
		{
			try
			{
				XmlSerializer xs = new XmlSerializer(typeof(XMLConfig));
				StreamReader reader = File.OpenText(strFile);
				XMLConfig xmlConfig = (XMLConfig)xs.Deserialize(reader);
				reader.Close();
				return xmlConfig;
			}
			catch
			{
				return null;
			}
			
		}

		/// <summary>
		/// Builds proper input path for encoded file
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		public static string BuildInputString(FileInfo inputFile)
		{
			string strInput = string.Empty;
			// Local Windows Mode
			if (Properties.Settings.Default.LOCAL_WINDOWS_MODE)
			{
				strInput = inputFile.FullName;
			}
			// Linux Mode
			else
			{
				if (inputFile.FullName.Contains("Movies"))
				{
					strInput = $@"'{Properties.Settings.Default.LINUX_INPUT_DIRECTORY}/Movies/{inputFile.Name}'";
				}
				else
				{
					string[] folders = inputFile.FullName.Split('\\');
					string strShow = folders[folders.Length - 3];
					string strSeason = folders[folders.Length - 2];

					if (inputFile.FullName.Contains("TV Shows"))
					{
						strInput = $@"'{Properties.Settings.Default.LINUX_INPUT_DIRECTORY}/TV Shows/{strShow}/{strSeason}/{inputFile.Name}'";
					}
					else if (inputFile.FullName.Contains("Anime"))
					{
						strInput = $@"'{Properties.Settings.Default.LINUX_INPUT_DIRECTORY}/Anime/{strShow}/{strSeason}/{inputFile.Name}'";
					}
				}	
			}
			return strInput;
		}

		/// <summary>
		/// Builds proper output path for encoded file
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns> 
		public static string BuildOutputString(FileInfo inputFile)
		{
			string outputDir = string.Empty;
			// Local Windows Mode
			if (Properties.Settings.Default.LOCAL_WINDOWS_MODE)
			{
				if (inputFile.FullName.Contains("Movies"))
				{
					outputDir = $@"{Properties.Settings.Default.LOCAL_OUTPUT}\Movies (Encoded)\{inputFile.Name.Replace("mkv", "m4v")}";
				}
				else
				{
					string[] folders = inputFile.FullName.Split('\\');
					string strShow = folders[folders.Length - 3];
					string strSeason = folders[folders.Length - 2];

					if (inputFile.FullName.Contains("TV Shows"))
					{
						outputDir = $@"{Properties.Settings.Default.LOCAL_OUTPUT}\TV Shows (Encoded)\{strShow}\{strSeason}";
						Directory.CreateDirectory(outputDir);
						outputDir += $@"\{inputFile.Name.Replace("mkv", "m4v")}";
					}
					else if (inputFile.FullName.Contains("Anime"))
					{
						outputDir = $@"{Properties.Settings.Default.LOCAL_OUTPUT}\Anime (Encoded)\{strShow}\{strSeason}";
						Directory.CreateDirectory(outputDir);
						outputDir += $@"\{inputFile.Name.Replace("mkv", "m4v")}";
					}
				}
			}
			// Linux Mode
			else
			{
				if (inputFile.FullName.Contains("Movies"))
				{
					outputDir = $@"'{Properties.Settings.Default.OUTPUT_DIRECTORY}/Movies (Encoded)/{inputFile.Name.Replace("mkv", "m4v")}'";
				}
				else
				{
					string[] folders = inputFile.FullName.Split('\\');
					string strShow = folders[folders.Length - 3];
					string strSeason = folders[folders.Length - 2];

					if (inputFile.FullName.Contains("TV Shows"))
					{
						outputDir = $@"'{Properties.Settings.Default.OUTPUT_DIRECTORY}/TV Shows (Encoded)/{strShow}/{strSeason}";
						Directory.CreateDirectory($@"\\{Properties.Settings.Default.PLEX_IP}\Media\TV Shows (Encoded)\{strShow}\{strSeason}"); // Uses mapped network drive Z:/
						outputDir += $@"\{inputFile.Name.Replace("mkv", "m4v")}'";
					}
					else if (inputFile.FullName.Contains("Anime"))
					{
						outputDir = $@"'{Properties.Settings.Default.OUTPUT_DIRECTORY}/Anime (Encoded)/{strShow}/{strSeason}";
						Directory.CreateDirectory($@"\\{Properties.Settings.Default.PLEX_IP}\Media\Anime (Encoded)\{strShow}\{strSeason}"); // Uses mapped network drive Z:/
						outputDir += $@"\{inputFile.Name.Replace("mkv", "m4v")}'";
					}
				}
			}
			return outputDir;
		}
	}
}
