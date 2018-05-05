using System;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet;

namespace RemoteHandbrakeController
{
	public static class Globals
	{
		public static SshClient client { get; set; }
		public static string currentFileBeingEncoded { get; set; } = String.Empty;

		public static readonly string LINUX_PLEX_PRESET = "/var/lib/handbrakecli/PlexHandbrake.json";
		// <summary>
		/// Connects to server
		/// </summary>
		public static bool ConnectToServer(SshClient client)
		{
			try
			{
				client.Connect();
				return true;
			}
			catch
			{
				return false;
			}

		}

		/// <summary>
		/// Disconnects from server
		/// Return value is based on connection status
		/// </summary>
		public static bool DisconnectFromServer(SshClient client)
		{
			try
			{
				client.Disconnect();
				return false;
			}
			catch
			{
				return true;
			}
		}
	}
}
