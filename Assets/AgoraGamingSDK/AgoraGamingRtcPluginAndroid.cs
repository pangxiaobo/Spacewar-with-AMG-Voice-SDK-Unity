#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

namespace agora_gaming_rtc
{

	public class AgoraGamingRtcPluginAndroid : IAgoraGamingRtcPlugin
	{
		private AndroidJavaObject agoraSDK = null;

		public void InitPlugin (string appId)
		{
			AndroidJNIHelper.debug = true;
			using (AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
				AndroidJavaObject curActivity = jc.GetStatic<AndroidJavaObject> ("currentActivity");
				agoraSDK = new AndroidJavaObject ("io.agora.gaming.unity3d.AgoraGamingRtcSDKWrapper", curActivity);
				initRtcEngine (appId);
			}
		}

		private void initRtcEngine (string appId)
		{
			agoraSDK.Call ("initRtcEngine", new[] { appId });
		}
	}

}

#endif
