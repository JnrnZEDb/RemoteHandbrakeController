using System;
using System.ComponentModel;

namespace RemoteHandbrakeController
{
	public enum Presets
	{
		// General
		[Description("Very Fast 1080p30")]
		VeryFast1080p30 = 1,
		[Description("Very Fast 720p30")]
		VeryFast720p30 = 2,
		[Description("Very Fast 576p25")]
		VeryFast576p25 = 3,
		[Description("Very Fast 480p30")]
		VeryFast480p30 = 4,
		[Description("Fast 1080p30")]
		Fast1080p30 = 5,
		[Description("Fast 720p30")]
		Fast720p30 = 6,
		[Description("Fast 576p25")]
		Fast576p25 = 7,
		[Description("Fast 480p30")]
		Fast480p30 = 8,
		[Description("HQ 1080p30 Surround")]
		HQ1080p30Surround = 9,
		[Description("HQ 720p30 Surround")]
		HQ720p30Surround = 10,
		[Description("HQ 576p25 Surround")]
		HQ576p25Surround = 11,
		[Description("HQ 480p30 Surround")]
		HQ480p30Surround = 12,
		[Description("Super HQ 1080p30 Surround")]
		SuperHQ1080p30Surround = 13,
		[Description("Super HQ 720p30 Surround")]
		SuperHQ720p30Surround = 14,
		[Description("Super HQ 576p25 Surround")]
		SuperHQ576p25Surround = 15,
		[Description("Super HQ 480p30 Surround")]
		SuperHQ480p30Surround = 16,
	}
}
