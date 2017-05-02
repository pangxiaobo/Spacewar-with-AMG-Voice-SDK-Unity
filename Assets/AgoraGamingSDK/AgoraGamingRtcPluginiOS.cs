#if UNITY_IPHONE
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

namespace agora_gaming_rtc
{

	public class AgoraGamingRtcPluginiOS : IAgoraGamingRtcPlugin
	{
		#region DllImport
		[DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
	    private static extern void initRtcEngine(string appId);
		#endregion

		public void InitPlugin (string appId)
		{
			initRtcEngine(appId);
		}
	}

}

#endif
