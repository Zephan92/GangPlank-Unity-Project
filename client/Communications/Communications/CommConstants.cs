﻿namespace Gangplank.Communications {
	public delegate void UpdateDelegate(string message);
		internal class CommConstants {
		internal const char endTransmission = (char)0x17, unitSplit = (char)0x1F, recordSplit = (char)0x1E;
		internal static readonly char[] unitSplitArr = {unitSplit};
		//internal const string matchServer = "ec2-54-200-46-105.us-west-2.compute.amazonaws.com";
		internal const string matchServer = "54.200.46.105";
		//internal const string matchServer = "192.168.1.113";
		internal const int matchPort = 80;
		//internal const int matchPort = 5555;
	}
}
