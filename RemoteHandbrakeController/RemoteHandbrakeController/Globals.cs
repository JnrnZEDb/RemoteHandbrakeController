using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Windows.Controls;

namespace RemoteHandbrakeController
{
	public static class Globals
	{
		public static string currentFileBeingEncoded { get; set; } = string.Empty;

		public static readonly string LINUX_PLEX_PRESET = "/var/lib/handbrakecli/PlexHandbrake.json";
		public static readonly string WINDOWS_PLEX_PRESET = @"E:\Libraries\Documents\Plex Presets\PlexHandbrake.json";
		public static readonly string CONFIG_NAME = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\RemoteHandbrakeController\RemoteHandbrakeController_Config.xml";

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
				Directory.CreateDirectory($@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\RemoteHandbrakeController");
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
		public static string BuildInputString(FileInfo inputFile, XMLConfig config)
		{
			string strInput = string.Empty;
			// Local Windows Mode
			if (config.LocalWindowsMode)
			{
				strInput = inputFile.FullName;
			}
			// Linux Mode
			else
			{
				if (inputFile.FullName.Contains("Movies"))
				{
					strInput = $@"'{config.RemoteInputDirectory}/Movies/{inputFile.Name}'";
				}
				else
				{
					string[] folders = inputFile.FullName.Split('\\');
					string strShow = folders[folders.Length - 3];
					string strSeason = folders[folders.Length - 2];

					if (inputFile.FullName.Contains("TV Shows"))
					{
						strInput = $@"'{config.RemoteInputDirectory}/TV Shows/{strShow}/{strSeason}/{inputFile.Name}'";
					}
					else if (inputFile.FullName.Contains("Anime"))
					{
						strInput = $@"'{config.RemoteInputDirectory}/Anime/{strShow}/{strSeason}/{inputFile.Name}'";
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
		public static string BuildOutputString(FileInfo inputFile, XMLConfig config)
		{
			string outputDir = string.Empty;
			// Local Windows Mode
			if (config.LocalWindowsMode)
			{
				if (inputFile.FullName.Contains("Movies"))
				{
					outputDir = $@"{config.LocalOutputDirectory}\Movies (Encoded)\{inputFile.Name.Replace("mkv", "m4v")}";
				}
				else
				{
					string[] folders = inputFile.FullName.Split('\\');
					string strShow = folders[folders.Length - 3];
					string strSeason = folders[folders.Length - 2];

					if (inputFile.FullName.Contains("TV Shows"))
					{
						outputDir = $@"{config.LocalOutputDirectory}\TV Shows (Encoded)\{strShow}\{strSeason}";
						Directory.CreateDirectory(outputDir);
						outputDir += $@"\{inputFile.Name.Replace("mkv", "m4v")}";
					}
					else if (inputFile.FullName.Contains("Anime"))
					{
						outputDir = $@"{config.LocalOutputDirectory}\Anime (Encoded)\{strShow}\{strSeason}";
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
					outputDir = $@"'{config.OutputDirectory}/Movies (Encoded)/{inputFile.Name.Replace("mkv", "m4v")}'";
				}
				else
				{
					string[] folders = inputFile.FullName.Split('\\');
					string strShow = folders[folders.Length - 3];
					string strSeason = folders[folders.Length - 2];

					if (inputFile.FullName.Contains("TV Shows"))
					{
						outputDir = $@"'{config.OutputDirectory}/TV Shows (Encoded)/{strShow}/{strSeason}";
						Directory.CreateDirectory($@"\\{config.PlexIP}\Media\TV Shows (Encoded)\{strShow}\{strSeason}"); // Uses mapped network drive Z:/
						outputDir += $@"\{inputFile.Name.Replace("mkv", "m4v")}'";
					}
					else if (inputFile.FullName.Contains("Anime"))
					{
						outputDir = $@"'{config.OutputDirectory}/Anime (Encoded)/{strShow}/{strSeason}";
						Directory.CreateDirectory($@"\\{config.PlexIP}\Media\Anime (Encoded)\{strShow}\{strSeason}"); // Uses mapped network drive Z:/
						outputDir += $@"\{inputFile.Name.Replace("mkv", "m4v")}'";
					}
				}
			}
			return outputDir;
		}

		#region EXTENSION_METHODS
		/// <summary> Enum extension method - gets enum description </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetDescription(this Enum value)
		{
			return
				value
					.GetType()
					.GetMember(value.ToString())
					.FirstOrDefault()
					?.GetCustomAttribute<DescriptionAttribute>()
					?.Description;
		}
		#endregion
	}
}
