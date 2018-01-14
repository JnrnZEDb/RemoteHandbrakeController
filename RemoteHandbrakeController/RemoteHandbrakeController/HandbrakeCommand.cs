using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		/// Full Constructor
		/// </summary>
		/// <param name="inputFile"></param>
		/// <param name="outputDir"></param>
		/// <param name="videoEncode"></param>
		/// <param name="audioEncode"></param>
		/// <param name="quality"></param>
		/// <param name="audioBitrate"></param>
		/// <param name="videoFramerate"></param>
		/// <param name="subtitle"></param>
		public HandbrakeCommand(string inputFile, string outputDir, string outputFormat, string videoEncode, string videoFramerate, string videoQuality, string audioEncode, string audioMixdown, string audioBitrate, string subtitle)
		{
			m_strInputFile = inputFile;
			m_strOutputDir = outputDir;
			m_strOutputFormat = outputFormat;
			m_strVideoEncode = videoEncode;
			m_strVideoQuality = videoQuality;
			m_strVideoFramerate = videoFramerate;
			m_strAudioEncode = audioEncode;
			m_strAudioMixdown = audioMixdown;
			m_strAudioBitrate = audioBitrate;
			m_strSubtitle = subtitle;
		}

		private string m_strInputFile { get; set; }
		private string m_strOutputDir { get; set; }
		private string m_strOutputFormat { get; set; } = "av_mp4";
		private string m_strVideoEncode { get; set; } = "x264";
		private string m_strVideoQuality { get; set; } = "22";
		private string m_strVideoFramerate { get; set; } = "30";
		private string m_strAudioEncode { get; set; } = "av_aac";
		private string m_strAudioMixdown { get; set; } = "dpl2";
		private string m_strAudioBitrate { get; set; } = "160";
		private string m_strSubtitle { get; set; } = "none";

		public override string ToString()
		{
			return String.Format("HandBrakeCLI -i {0} -o {1} -f {2} --optimize -e {3} --encoder-preset fast --encoder-profile main -q {4} --pfr -r {5} -E {6} --mixdown {7} -B {8} -R Auto -D 0 --gain 0.0 --audio-fallback ac3 --auto-anamorphic -s {9}",
				m_strInputFile,
				m_strOutputDir,
				m_strOutputFormat,
				m_strVideoEncode,
				m_strVideoQuality,
				m_strVideoFramerate,
				m_strAudioEncode,
				m_strAudioMixdown,
				m_strAudioBitrate,
				m_strSubtitle);
		}

	}
}
