using System;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet;

namespace RemoteHandbrakeController
{
	public static class Globals
	{
		public static string currentFileBeingEncoded { get; set; } = String.Empty;

		public static readonly string LINUX_PLEX_PRESET = "/var/lib/handbrakecli/PlexHandbrake.json";
		public static readonly string WINDOWS_PLEX_PRESET = @"E:\Libraries\Documents\Plex Presets\PlexHandbrake.json";
	}
}
