﻿using System;

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
		public HandbrakeCommand(string inputFile, string outputDir, string preset)
		{
			m_strInputFile = inputFile;
			m_strOutputDir = outputDir;
			HandBrakePreset = preset;
		}

		public string HandBrakePreset { get; set; } = "/var/lib/handbrakecli/PlexHandbrake.json";

		private string m_strInputFile { get; set; }
		private string m_strOutputDir { get; set; }

		public override string ToString()
		{
			return $"HandBrakeCLI -i {m_strInputFile} -o {m_strOutputDir} --preset-import-file {HandBrakePreset}";
		}

	}
}
