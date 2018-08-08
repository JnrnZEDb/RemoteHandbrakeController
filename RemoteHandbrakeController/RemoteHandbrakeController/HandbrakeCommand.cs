using System;

namespace RemoteHandbrakeController
{
	/// <summary>
	/// Class that constructs a HandBrakeCLI command based off input;
	/// </summary>
	class HandbrakeCommand
	{
		/// <summary>
		/// default constructor
		/// </summary>
		/// <param name="inputFile"></param>
		/// <param name="outputDir"></param>
		public HandbrakeCommand(string inputFile, string outputDir)
		{
			m_strInputFile = inputFile;
			m_strOutputDir = outputDir;
		}

		/// <summary>
		/// Preset Contructor
		/// </summary>
		/// <param name="inputFile"></param>
		/// <param name="outputDir"></param>
		/// <param name="preset"></param>
		public HandbrakeCommand(string inputFile, string outputDir, string preset, bool import = false)
		{
			m_strInputFile = inputFile;
			m_strOutputDir = outputDir;
			HandBrakePreset = preset;
			m_bIsImport = import;
		}

		public string HandBrakePreset { get; set; }

		private string m_strInputFile { get; set; }
		private string m_strOutputDir { get; set; }
		private bool m_bIsImport { get; set; }

		public override string ToString()
		{
			if (m_bIsImport)
			{
				return $"HandBrakeCLI -i {m_strInputFile} -o {m_strOutputDir} --preset-import-file {HandBrakePreset}";
			}
			else
			{
				return $"HandBrakeCLI -i {m_strInputFile} -o {m_strOutputDir} --preset \"{HandBrakePreset}\"";
			}
		}

	}
}
