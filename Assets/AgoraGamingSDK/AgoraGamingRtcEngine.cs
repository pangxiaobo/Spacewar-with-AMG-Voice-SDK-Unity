using UnityEngine;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace agora_gaming_rtc
{
	public struct RtcStats
	{
		public uint duration;
		public uint txBytes;
		public uint rxBytes;
		public ushort txKBitRate;
		public ushort rxKBitRate;
		public ushort txAudioKBitRate;
		public ushort rxAudioKBitRate;
		public uint lastmileQuality;
		public uint users;
		public double cpuAppUsage;
		public double cpuTotalUsage;
	};

	public struct AudioVolumeInfo
	{
		public uint uid;
		public uint volume; // [0, 255]
	};

	public enum LOG_FILTER
	{
		OFF = 0,
		DEBUG = 0x80f,
		INFO = 0x0f,
		WARNING = 0x0e,
		ERROR = 0x0c,
		CRITICAL = 0x08,
	};

	public enum CHANNEL_PROFILE
	{
		GAME_FREE_MODE = 2,
		GAME_COMMAND_MODE = 3,
	};

	public enum CLIENT_ROLE
	{
		BROADCASTER = 1,
		AUDIENCE = 2,
	};

	public enum AUDIO_ROUTE
	{
		DEFAULT = -1,
		HEADSET = 0,
		EARPIECE = 1,
		SPEAKERPHONE = 3,
		BLUETOOTH = 5,
	};

	public enum USER_OFFLINE_REASON
	{
		QUIT = 0,
		DROPPED = 1,
		BECOME_AUDIENCE = 2,
	};

	enum MEDIA_ENGINE_EVENT_CODE
	{
		AUDIO_FILE_MIX_FINISH = 10,
	};

	public interface IAudioEffectManager
	{
		/**
		 * get audio effect volume in the range of [0.0..100.0]
		 *
		 * @return return effect volume
		 */
		double GetEffectsVolume();

		/**
		 * set audio effect volume
		 *
		 * @param [in] volume
		 *        in the range of [0..100]
		 * @return return 0 if success or an error code
		 */
		int SetEffectsVolume(double volume);

		/**
		 * start playing local audio effect specified by file path and other parameters
		 *
		 * @param [in] soundId
		 *        specify the unique sound id
		 * @param [in] filePath
		 *        specify the path and file name of the effect audio file to be played
		 * @param [in] loop
		 *        whether to loop the effect playing or not, default value is false
		 * @param [in] pitch
		 *        frequency, in the range of [0.5..2.0], default value is 1.0
		 * @param [in] pan
		 *        stereo effect, in the range of [-1..1] where -1 enables only left channel, default value is 0.0
		 * @param [in] gain
		 *        volume, in the range of [0..100], default value is 100
		 * @return return 0 if success or an error code
		 */
		int PlayEffect (int soundId, String filePath,
		               bool loop = false,
		               double pitch = 1.0D,
		               double pan = 0.0D,
		               double gain = 100.0D
		);

		/**
		 * stop playing specified audio effect
		 *
		 * @param [in] soundId
		 *        specify the unique sound id
		 * @return return 0 if success or an error code
		 */
		int StopEffect(int soundId);

		/**
		 * stop all playing audio effects
		 *
		 * @return return 0 if success or an error code
		 */
		int StopAllEffects();

		/**
		 * preload a compressed audio effect file specified by file path for later playing
		 *
		 * @param [in] soundId
		 *        specify the unique sound id
		 * @param [in] filePath
		 *        specify the path and file name of the effect audio file to be preloaded
		 * @return return 0 if success or an error code
		 */
		int PreloadEffect(int soundId, String filePath);

		/**
		 * unload specified audio effect file from SDK
		 *
		 * @return return 0 if success or an error code
		 */
		int UnloadEffect(int soundId);

		/**
		 * pause playing specified audio effect
		 *
		 * @param [in] soundId
		 *        specify the unique sound id
		 * @return return 0 if success or an error code
		 */
		int PauseEffect(int soundId);

		/**
		 * pausing all playing audio effects
		 *
		 * @return return 0 if success or an error code
		 */
		int PauseAllEffects();

		/**
		 * resume playing specified audio effect
		 *
		 * @param [in] soundId
		 *        specify the unique sound id
		 * @return return 0 if success or an error code
		 */
		int ResumeEffect(int soundId);

		/**
		 * resume all playing audio effects
		 *
		 * @return return 0 if success or an error code
		 */
		int ResumeAllEffects();

		/**
		 * set voice only mode(e.g. keyboard strokes sound will be eliminated)
		 *
		 * @param [in] enable
		 *        true for enable, false for disable
		 * @return return 0 if success or an error code
		 */
		int SetVoiceOnlyMode(bool enable);

		/**
		 * place specified speaker's voice with pan and gain
		 *
		 * @param [in] uid
		 *        speaker's uid
		 * @param [in] pan
		 *        stereo effect, in the range of [-1..1] where -1 enables only left channel, default value is 0.0
		 * @param [in] gain
		 *        volume, in the range of [0..100], default value is 100
		 * @return return 0 if success or an error code
		 */
		int SetRemoteVoicePosition(uint uid, double pan, double gain);

		/**
		 * change the pitch of local speaker's voice
		 *
		 * @param [in] pitch
		 *        frequency, in the range of [0.5..2.0], default value is 1.0
		 * @return return 0 if success or an error code
		 */
		int SetLocalVoicePitch (double pitch);
	}

	public abstract class IRtcEngineForGaming
	{
		// RTC callback
		/// <summary>
		/// Callback when joinChannel successful.
		/// </summary>
		/// <param name="channelName">channel name</param>
		/// <returns></returns>
		public delegate void JoinChannelSuccessHandler (string channelName, uint uid, int elapsed);

		public abstract event JoinChannelSuccessHandler OnJoinChannelSuccess;

		// RTC callback
		/// <summary>
		/// Callback when re-joinChannel successful.
		/// </summary>
		/// <param name="channelName">channel name</param>
		/// <returns></returns>
		public delegate void ReJoinChannelSuccessHandler (string channelName, uint uid, int elapsed);

		public abstract event ReJoinChannelSuccessHandler OnReJoinChannelSuccess;

		public delegate void ConnectionLostHandler ();

		public abstract event ConnectionLostHandler OnConnectionLost;

		public delegate void ConnectionInterruptedHandler ();

		public abstract event ConnectionInterruptedHandler OnConnectionInterrupted;

		public delegate void RequestChannelKeyHandler ();

		public abstract event RequestChannelKeyHandler OnRequestChannelKey;

		public delegate void UserJoinedHandler (uint uid, int elapsed);

		public abstract event UserJoinedHandler OnUserJoined;

		public delegate void UserOfflineHandler (uint uid, USER_OFFLINE_REASON reason);

		public abstract event UserOfflineHandler OnUserOffline;

		public delegate void LeaveChannelHandler (RtcStats stats);

		public abstract event LeaveChannelHandler OnLeaveChannel;

		public delegate void VolumeIndicationHandler (AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume);

		public abstract event VolumeIndicationHandler OnVolumeIndication;

		public delegate void UserMutedHandler (uint uid, bool muted);

		public abstract event UserMutedHandler OnUserMuted;

		public delegate void SDKWarningHandler (int warn, string msg);

		public abstract event SDKWarningHandler OnWarning;

		public delegate void SDKErrorHandler (int error, string msg);

		public abstract event SDKErrorHandler OnError;

		public delegate void RtcStatsHandler (RtcStats stats);

		public abstract event RtcStatsHandler OnRtcStats;

		public delegate void AudioMixingFinishedHandler ();

		public abstract event AudioMixingFinishedHandler OnAudioMixingFinished;

		public delegate void AudioRouteChangedHandler (AUDIO_ROUTE route);

		public abstract event AudioRouteChangedHandler OnAudioRouteChanged;

		/**
		 * Set the channel profile: such as game free mode, game command mode
		 *
		 * @param profile the channel profile
		 * @return return 0 if success or an error code
		 */
		public abstract int SetChannelProfile(CHANNEL_PROFILE profile);

		/**
		 * Set the role of user: such as broadcaster, audience
		 *
		 * @param role the role of client
		 * @return return 0 if success or an error code
		 */
		public abstract int SetClientRole(CLIENT_ROLE role);

		/**
		 * set the log information filter level
		 *
		 * @param [in] filter
		 *        the filter level
		 * @return return 0 if success or an error code
		 */
		public abstract int SetLogFilter (LOG_FILTER filter);

		/**
		 * set path to save the log file
		 *
		 * @param [in] filePath
		 *        the .log file path you want to saved
		 * @return return 0 if success or an error code
		 */
		public abstract int SetLogFile (string filePath);

		/**
		 * get IAudioEffectManager object
		 *
		 * @return return IAudioEffectManager object associated with current rtc engine
		 */
		public abstract IAudioEffectManager GetAudioEffectManager();

		/**
		 * join the channel, if the channel have not been created, it will been created automatically
		 *
		 * @param [in] channelName
		 *        the channel name
		 * @param [in] info
		 *        the additional information, it can be null here
		 * @param [in] uid
		 *        the uid of you, if 0 the system will automatically allocate one for you
		 * @return return 0 if success or an error code
		 */
		public abstract int JoinChannel (string channelName, string info, uint uid);

		public abstract int JoinChannelWithKey (string channelKey, string channelName, string info, uint uid);

		public abstract int RenewChannelKey (string channelKey);

		/**
		 * leave the current channel
		 *
		 * @return return 0 if success or an error code
		 */
		public abstract int LeaveChannel ();

		public abstract void Pause ();

		public abstract void Resume();

		/**
		 * trigger the SDK event working according to vertical synchronization such as Update(Unity3D)
		 *
		 * @return return void
		 */
		public abstract void Poll ();

		/**
		 * set parameters of the SDK
		 *
		 * @param [in] parameters
		 *        the parameters(in json format)
		 * @return return 0 if success or an error code
		 */
		public abstract int SetParameters (string parameters);

		/**
		 * enable audio function, which is enabled by deault.
		 *
		 * @return return 0 if success or an error code
		 */
		public abstract int EnableAudio();

		/**
		 * disable audio function
		 *
		 * @return return 0 if success or an error code
		 */
		public abstract int DisableAudio();

		/**
		 * mute/unmute the local audio stream capturing
		 *
		 * @param [in] mute
		 *       true: mute
		 *       false: unmute
		 * @return return 0 if success or an error code
		 */
		public abstract int MuteLocalAudioStream (bool mute);

		/**
		 * mute/unmute all the remote audio stream receiving
		 *
		 * @param [in] mute
		 *       true: mute
		 *       false: unmute
		 * @return return 0 if success or an error code
		 */
		public abstract int MuteAllRemoteAudioStreams (bool mute);

		/**
		 * mute/unmute specified remote audio stream receiving
		 *
		 * @param [in] uid
		 *        the uid of the remote user you want to mute/unmute
		 * @param [in] mute
		 *       true: mute
		 *       false: unmute
		 * @return return 0 if success or an error code
		 */
		public abstract int MuteRemoteAudioStream (uint uid, bool mute);

		public abstract int SetEnableSpeakerphone (bool speakerphone);

		public abstract int SetDefaultAudioRouteToSpeakerphone(bool speakerphone);

		/**
		 * enable or disable the audio volume indication
		 *
		 * @param [in] interval
		 *        the period of the callback cycle, in ms
		 *        interval <= 0: disable
		 *        interval >  0: enable
		 * @param [in] smooth
		 *        the smooth parameter
		 * @return return 0 if success or an error code
		 */
		public abstract int EnableAudioVolumeIndication (int interval, int smooth);

		/**
		 * adjust recording signal volume
		 *
		 * @param [in] volume range from 0 to 400
		 * @return return 0 if success or an error code
		 */
		public abstract int AdjustRecordingSignalVolume (int volume);

		/**
		 * adjust playback signal volume
		 *
		 * @param [in] volume range from 0 to 400
		 * @return return 0 if success or an error code
		 */
		public abstract int AdjustPlaybackSignalVolume (int volume);

		/**
		 * mix microphone and local audio file into the audio stream
		 *
		 * @param [in] filePath
		 *        specify the path and file name of the audio file to be played
		 * @param [in] loopback
		 *        specify if local and remote participant can hear the audio file.
		 *        false (default): both local and remote party can hear the the audio file
		 *        true: only the local party can hear the audio file
		 * @param [in] replace
		 *        false (default): mix the local microphone captured voice with the audio file
		 *        true: replace the microphone captured voice with the audio file
		 * @param [in] cycle
		 *        specify the number of cycles to play
		 *        -1, infinite loop playback
		 * @param [in] playTime
		 *        specify the start time(ms) of the audio file to play
		 *        0, from the start
		 * @return return 0 if success or an error code
		 */
		public abstract int StartAudioMixing (string filePath, bool loopback, bool replace, int cycle, int playTime = 0);

		/**
		 * stop mixing the local audio stream
		 *
		 * @return return 0 if success or an error code
		 */
		public abstract int StopAudioMixing();

		/**
		 * pause mixing the local audio stream
		 *
		 * @return return 0 if success or an error code
		 */
		public abstract int PauseAudioMixing();

		/**
		 * resume mixing the local audio stream
		 *
		 * @return return 0 if success or an error code
		 */
		public abstract int ResumeAudioMixing();

		/**
		 * adjust mixing audio file volume
		 *
		 * @param [in] volume range from 0 to 100
		 * @return return 0 if success or an error code
		 */
		public abstract int AdjustAudioMixingVolume (int volume);

		/**
		 * get the duration of the specified mixing audio file
		 *
		 * @return return duration(ms)
		 */
		public abstract int GetAudioMixingDuration();

		/**
		 * get the current playing position of the specified mixing audio file
		 *
		 * @return return the current playing(ms)
		 */
		public abstract int GetAudioMixingCurrentPosition();

		/**
		 * start recording audio streaming to file specified by the file path
		 *
		 * @param filePath file path to save recorded audio streaming
		 * @return return 0 if success or an error code
		 */
		public abstract int StartAudioRecording(string filePath);

		/**
		 * stop audio streaming recording
		 *
		 * @return return 0 if success or an error code
		 */
		public abstract int StopAudioRecording();
	}

	class AudioEffectManagerImpl : IAudioEffectManager
	{
		private readonly RtcEngineForGaming mEngine;

		public AudioEffectManagerImpl (RtcEngineForGaming rtcEngine)
		{
			mEngine = rtcEngine;
		}

		public double GetEffectsVolume ()
		{
			return mEngine.GetParameter ("che.audio.game_get_effects_volume");
		}

		public int SetEffectsVolume (double volume)
		{
			return mEngine.SetParameter ("che.audio.game_set_effects_volume", volume);
		}

		public int PlayEffect (int soundId, String filePath,
		                       bool loop = false,
		                       double pitch = 1.0D,
		                       double pan = 0.0D,
		                       double gain = 100.0D)
		{
			string loopValue = loop ? "true" : "false";
			string fmt = "{{\"che.audio.game_play_effect\": {{\"soundId\":{0},\"filePath\":\"{1}\",\"loop\":{2},\"pitch\":{3:0.00},\"pan\":{4:0.00},\"gain\":{5:0.00}}}}}";
			string parameters = mEngine.doFormat (fmt, soundId, filePath, loopValue, pitch, pan, gain);
			return mEngine.SetParameters (parameters);
		}

		public int StopEffect (int soundId)
		{
			return mEngine.SetParameter ("che.audio.game_stop_effect", soundId);
		}

		public int StopAllEffects ()
		{
			return mEngine.SetParameter ("che.audio.game_stop_all_effects", true);
		}

		public int PreloadEffect (int soundId, String filePath)
		{
			string fmt = "{{\"che.audio.game_preload_effect\": {{\"soundId\":{0},\"filePath\":\"{1}}}}}";
			string parameters = mEngine.doFormat (fmt, soundId, filePath);
			return mEngine.SetParameters (parameters);
		}

		public int UnloadEffect(int soundId)
		{
			return mEngine.SetParameter ("che.audio.game_unload_effect", soundId);
		}

		public int PauseEffect(int soundId)
		{
			return mEngine.SetParameter ("che.audio.game_pause_effect", soundId);
		}

		public int PauseAllEffects()
		{
			return mEngine.SetParameter ("che.audio.game_pause_all_effects", true);
		}

		public int ResumeEffect(int soundId)
		{
			return mEngine.SetParameter ("che.audio.game_resume_effect", soundId);
		}

		public int ResumeAllEffects()
		{
			return mEngine.SetParameter ("che.audio.game_resume_all_effects", true);
		}

		public int SetVoiceOnlyMode(bool enable)
		{
			return mEngine.SetParameter ("che.audio.game_voice_over_mode", enable);
		}

		public int SetRemoteVoicePosition(uint uid, double pan, double gain)
		{
			string fmt = "{{\"che.audio.game_place_sound_position\": {{\"uid\":{0},\"pan\":{1:0.00},\"gain\":{2:0.00}}}}}";
			string parameters = mEngine.doFormat (fmt, uid, pan, gain);
			return mEngine.SetParameters (parameters);
		}

		public int SetLocalVoicePitch (double pitch)
		{
			return mEngine.SetParameter ("che.audio.game_local_pitch_shift", pitch * 100);
		}
	}

	public class RtcEngineForGaming : IRtcEngineForGaming
	{
		#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		public const string MyLibName = "agora-rtc-sdk-jni";
		#else
		#if UNITY_IPHONE
		public const string MyLibName = "__Internal";
		#else
		public const string MyLibName = "agora-rtc-sdk-jni";
		#endif
		#endif

		enum EngineEventType
		{
			ON_ERROR_EVENT = 101,
			ON_WARNING_EVENT = 102,
			ON_MEDIA_ENGINE_EVENT = 1104,
			ON_REQUEST_CHANNEL_KEY = 1108,
			ON_OPEN_CHANNEL_SUCCESS = 13001,
			ON_LEAVE_CHANNEL = 13006,
			ON_USER_OFFLINE = 13008,
			ON_RTC_STATS = 13010,
			ON_USER_JOINED = 13013,
			ON_USER_MUTE_AUDIO = 13014,
			ON_AUDIO_VOLUME_INDICATION = 14001,
			ON_CONNECTION_LOST = 14008,
			ON_CONNECTION_INTERRUPTED = 14010,
		};

		public class EngineMessageData
		{
			public int what;
			public int intArg0;
			public int intArg1;
			public int intArg2;
			public string strArg;
			public int extraLength;
			public byte[] extra;

			public EngineMessageData (int size)
			{
				what = -1;
				intArg0 = 0;
				intArg1 = 0;
				intArg2 = 0;
				strArg = "";
				extraLength = 0;
				extra = new byte[size];
			}

			public void clear ()
			{
				what = -1;
				intArg0 = 0;
				intArg1 = 0;
				intArg2 = 0;
				strArg = "";
				extraLength = 0;
			}
		}

		private const bool DBG_ENABLED = false;

		private const int BUFFER_LENGTH = 2048;
		private readonly byte[] mPollBuf;
		private readonly EngineMessageData mPollMsg;

		public override event JoinChannelSuccessHandler OnJoinChannelSuccess;

		public override event ReJoinChannelSuccessHandler OnReJoinChannelSuccess;

		public override event ConnectionInterruptedHandler OnConnectionInterrupted;

		public override event ConnectionLostHandler OnConnectionLost;

		public override event RequestChannelKeyHandler OnRequestChannelKey;

		public override event LeaveChannelHandler OnLeaveChannel;

		public override event UserJoinedHandler OnUserJoined;

		public override event UserOfflineHandler OnUserOffline;

		public override event VolumeIndicationHandler OnVolumeIndication;

		public override event UserMutedHandler OnUserMuted;

		public override event SDKWarningHandler OnWarning;

		public override event SDKErrorHandler OnError;

		public override event RtcStatsHandler OnRtcStats;

		public override event AudioMixingFinishedHandler OnAudioMixingFinished;

		public override event AudioRouteChangedHandler OnAudioRouteChanged;

		#region DllImport
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr AgoraRtcEngineForGaming_getVersion();
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr AgoraRtcEngineForGaming_getErrorDescription(int code);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_enableCustomizedPoll();
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_poll([In, Out] byte[] buf, int length);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_joinChannel([MarshalAs(UnmanagedType.LPArray)] string channelName, [MarshalAs(UnmanagedType.LPArray)] string info, uint uid);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_joinChannelWithKey([MarshalAs(UnmanagedType.LPArray)] string channelKey, [MarshalAs(UnmanagedType.LPArray)] string channelName, [MarshalAs(UnmanagedType.LPArray)] string info, uint uid);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_renewChannelKey([MarshalAs(UnmanagedType.LPArray)] string channelKey);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_leaveChannel();
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_setParameters([MarshalAs(UnmanagedType.LPArray)] string parameters);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_getIntParameter([MarshalAs(UnmanagedType.LPArray)] string parameter, [MarshalAs(UnmanagedType.LPArray)] string args);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_enableAudio();
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_disableAudio();
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern void AgoraRtcEngineForGaming_pause();
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern void AgoraRtcEngineForGaming_resume();
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_startAudioMixing([MarshalAs(UnmanagedType.LPArray)] string filePath, int loopback, int replace, int cycle, int playTime);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_stopAudioMixing();
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_muteLocalAudioStream(int mute);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_setChannelProfile(int profile);
		[DllImport(MyLibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int AgoraRtcEngineForGaming_setClientRole(int role);
		#endregion

		private readonly IAgoraGamingRtcPlugin plugin;

		private readonly AudioEffectManagerImpl mAudioEffectM;

		private RtcEngineForGaming (string appId)
		{
			#if UNITY_ANDROID
			plugin = new AgoraGamingRtcPluginAndroid ();
			#elif UNITY_IPHONE
			plugin = new AgoraGamingRtcPluginiOS();
			#endif

			plugin.InitPlugin (appId);

			mPollBuf = new byte[BUFFER_LENGTH];
			mPollMsg = new EngineMessageData (BUFFER_LENGTH);

			AgoraRtcEngineForGaming_enableCustomizedPoll ();

			mAudioEffectM = new AudioEffectManagerImpl (this);
		}

		public string doFormat (string format, params object[] args)
		{
			return string.Format (CultureInfo.InvariantCulture, format, args);
		}

		/**
		 * get the version information of the SDK
		 *
		 * @return return the version string
		 */
		public static string GetSdkVersion ()
		{
			return Marshal.PtrToStringAnsi (AgoraRtcEngineForGaming_getVersion ());
		}

		/**
		 * get the error description from SDK
		 * @param [in] code
		 *        the error code
		 * @return return the error description string
		 */
		public static string GetErrorDescription (int code)
		{
			return Marshal.PtrToStringAnsi (AgoraRtcEngineForGaming_getErrorDescription (code));
		}

		public override int JoinChannel (string channelName, string info, uint uid)
		{
			return AgoraRtcEngineForGaming_joinChannel(channelName, info, uid);
		}

		public override int JoinChannelWithKey (string channelKey, string channelName, string info, uint uid)
		{
			return AgoraRtcEngineForGaming_joinChannelWithKey(channelKey, channelName, info, uid);
		}

		public override int RenewChannelKey (string channelKey)
		{
			return AgoraRtcEngineForGaming_renewChannelKey (channelKey);
		}

		public override void Poll ()
		{
			int leftMessageCount = AgoraRtcEngineForGaming_poll (mPollBuf, BUFFER_LENGTH);

			if (leftMessageCount < 0) {
				return;
			}

			EngineMessageData returnedMsgData = parseEngineMessage (mPollBuf, BUFFER_LENGTH);

			if (DBG_ENABLED) {
				Debug.Log (doFormat ("polling {0}, what {1}, extra size: {2}", leftMessageCount, returnedMsgData.what, returnedMsgData.extraLength));
			}

			if (returnedMsgData.what == (int)EngineEventType.ON_OPEN_CHANNEL_SUCCESS) {

				if (returnedMsgData.intArg0 > 0) {
					if (OnJoinChannelSuccess != null) {
						OnJoinChannelSuccess (returnedMsgData.strArg, (uint)returnedMsgData.intArg1, returnedMsgData.intArg2);
					}
				} else {
					if (OnReJoinChannelSuccess != null) {
						OnReJoinChannelSuccess (returnedMsgData.strArg, (uint)returnedMsgData.intArg1, returnedMsgData.intArg2);
					}
				}
			} else if (returnedMsgData.what == (int)EngineEventType.ON_REQUEST_CHANNEL_KEY) {
				if (OnRequestChannelKey != null) {
					OnRequestChannelKey ();
				}
			} else if (returnedMsgData.what == (int)EngineEventType.ON_CONNECTION_LOST) {
				if (OnConnectionLost != null) {
					OnConnectionLost ();
				}
			} else if (returnedMsgData.what == (int)EngineEventType.ON_CONNECTION_INTERRUPTED) {
				if (OnConnectionInterrupted != null) {
					OnConnectionInterrupted ();
				}
			} else if (returnedMsgData.what == (int)EngineEventType.ON_LEAVE_CHANNEL) {
				if (OnLeaveChannel != null && returnedMsgData.extraLength > 0) {
					RtcStats stats;

					int statsIdx = 0;
					stats.duration = BitConverter.ToUInt32 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt32);
					stats.txBytes = BitConverter.ToUInt32 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt32);
					stats.rxBytes = BitConverter.ToUInt32 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt32);
					stats.txKBitRate = BitConverter.ToUInt16 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt16);
					stats.rxKBitRate = BitConverter.ToUInt16 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt16);

					stats.txAudioKBitRate = 0;
					stats.rxAudioKBitRate = 0;
					stats.cpuAppUsage = 0;
					stats.cpuTotalUsage = 0;
					stats.lastmileQuality = 0;
					stats.users = 0;

					OnLeaveChannel (stats);
				}
			} else if (returnedMsgData.what == (int)EngineEventType.ON_USER_JOINED) {
				if (OnUserJoined != null) {
					OnUserJoined ((uint)returnedMsgData.intArg0, returnedMsgData.intArg1);
				}
			} else if (returnedMsgData.what == (int)EngineEventType.ON_USER_OFFLINE) {
				if (OnUserOffline != null) {
					OnUserOffline ((uint)returnedMsgData.intArg0, (USER_OFFLINE_REASON)returnedMsgData.intArg1);
				}
			} else if (returnedMsgData.what == (int)EngineEventType.ON_AUDIO_VOLUME_INDICATION) {
				if (OnVolumeIndication != null) {
					int totalVolume = returnedMsgData.intArg0;
					int speakersCount = returnedMsgData.extraLength / (sizeof(Int32) * 2); // uid_t, int32_t
					if (speakersCount <= 0) {
						OnVolumeIndication (null, 0, totalVolume);
					} else {
						AudioVolumeInfo[] infos = new AudioVolumeInfo[speakersCount];

						int statsIdx = 0;
						for (int idx = 0; idx < speakersCount; idx++) {
							infos [idx].uid = BitConverter.ToUInt32 (mPollMsg.extra, statsIdx);
							statsIdx += sizeof(UInt32);
							infos [idx].volume = BitConverter.ToUInt32 (mPollMsg.extra, statsIdx);
							statsIdx += sizeof(UInt32);
						}

						OnVolumeIndication (infos, speakersCount, totalVolume);
					}
				}
			} else if (returnedMsgData.what == (int)EngineEventType.ON_USER_MUTE_AUDIO) {
				if (OnUserMuted != null) {
					OnUserMuted ((uint)returnedMsgData.intArg0, returnedMsgData.intArg1 > 0);
				}
			} else if (returnedMsgData.what == (int)EngineEventType.ON_WARNING_EVENT) {
				if (OnWarning != null) {
					OnWarning (returnedMsgData.intArg0, returnedMsgData.strArg);
				}
			} else if (returnedMsgData.what == (int)EngineEventType.ON_ERROR_EVENT) {
				if (OnError != null) {
					OnError (returnedMsgData.intArg0, returnedMsgData.strArg);
				}
			} else if (returnedMsgData.what == (int)EngineEventType.ON_MEDIA_ENGINE_EVENT) {
				int routing = (int)returnedMsgData.intArg0 - 100; // refer to AUDIO_ROUTE_CODE_DEVIATION

				if (returnedMsgData.intArg0 == (int)MEDIA_ENGINE_EVENT_CODE.AUDIO_FILE_MIX_FINISH) {
					if (OnAudioMixingFinished != null) {
						OnAudioMixingFinished ();
					}
				} else if (routing >= (int)AUDIO_ROUTE.DEFAULT &&
				           routing <= (int)AUDIO_ROUTE.BLUETOOTH) {
					if (OnAudioRouteChanged != null) {
						OnAudioRouteChanged ((AUDIO_ROUTE)routing);
					}
				}
			} else if (returnedMsgData.what == (int)EngineEventType.ON_RTC_STATS) {
				if (OnRtcStats != null && returnedMsgData.extraLength > 0) {
					RtcStats stats;

					int statsIdx = 0;
					stats.duration = BitConverter.ToUInt32 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt32);
					stats.txBytes = BitConverter.ToUInt32 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt32);
					stats.rxBytes = BitConverter.ToUInt32 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt32);
					stats.txKBitRate = BitConverter.ToUInt16 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt16);
					stats.rxKBitRate = BitConverter.ToUInt16 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt16);
					stats.txAudioKBitRate = BitConverter.ToUInt16 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt16);
					stats.rxAudioKBitRate = BitConverter.ToUInt16 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt16);

					stats.cpuAppUsage = 0;
					stats.cpuTotalUsage = 0;
					stats.lastmileQuality = 0;

					stats.users = BitConverter.ToUInt32 (mPollMsg.extra, statsIdx);
					statsIdx += sizeof(UInt32);

					OnRtcStats (stats);
				}
			}
		}

		public override int LeaveChannel ()
		{
			return AgoraRtcEngineForGaming_leaveChannel ();
		}

		public override void Pause ()
		{
			AgoraRtcEngineForGaming_pause ();
		}

		public override void Resume ()
		{
			AgoraRtcEngineForGaming_resume ();
		}

		public override int SetParameters (string parameters)
		{
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public int SetParameter (string parameter, double value)
		{
			string parameters = doFormat ("{{\"{0}\": {1}}}", parameter, value);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public int SetParameter (string parameter, bool value)
		{
			string boolValue = value ? "true" : "false";
			string parameters = doFormat ("{{\"{0}\": {1}}}", parameter, boolValue);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public int GetParameter(string parameter)
		{
			return AgoraRtcEngineForGaming_getIntParameter (parameter, null);
		}

		public override int SetLogFilter (LOG_FILTER filter)
		{
			string parameters = doFormat ("{{\"rtc.log_filter\": {0}}}", (uint)filter);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int SetLogFile (string filePath)
		{
			string parameters = doFormat ("{{\"rtc.log_file\": \"{0}\"}}", filePath);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int EnableAudio ()
		{
			return AgoraRtcEngineForGaming_enableAudio ();
		}

		public override int DisableAudio ()
		{
			return AgoraRtcEngineForGaming_disableAudio ();
		}

		public override int SetChannelProfile (CHANNEL_PROFILE profile)
		{
			return AgoraRtcEngineForGaming_setChannelProfile ((int)profile);
		}

		public override int SetClientRole (CLIENT_ROLE role)
		{
			return AgoraRtcEngineForGaming_setClientRole ((int)role);
		}

		public override int EnableAudioVolumeIndication(int interval, int smooth)
		{
			string parameters = doFormat ("{{\"che.audio.volume_indication\":{{\"interval\":{0},\"smooth\":{1}}}}}", interval, smooth);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int MuteLocalAudioStream (bool mute)
		{
			return AgoraRtcEngineForGaming_muteLocalAudioStream (mute ? 1 : 0);
		}

		public override int MuteAllRemoteAudioStreams (bool mute)
		{
			string boolValue = mute ? "true" : "false";
			string parameters = doFormat ("{{\"rtc.audio.mute_peers\":{0}}}", boolValue);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int MuteRemoteAudioStream (uint uid, bool mute)
		{
			string boolValue = mute ? "true" : "false";
			string parameters = doFormat ("{{\"rtc.audio.mute_peer\":{{\"uid\":{0},\"mute\":{1}}}}}", uid, boolValue);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int SetEnableSpeakerphone (bool speakerphone)
		{
			string parameters = doFormat ("{{\"rtc.set_enable_speakerphone\":{0}}}", speakerphone ? 1 : 0);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int SetDefaultAudioRouteToSpeakerphone (bool speakerphone)
		{
			string parameters = doFormat ("{{\"rtc.set_default_audio_route_to_speakerphone\":{0}}}", speakerphone ? 1 : 0);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int AdjustRecordingSignalVolume (int volume)
		{
			if (volume < 0)
				volume = 0;
			else if (volume > 400)
				volume = 400;

			string parameters = doFormat ("{{\"che.audio.record.signal.volume\":{0}}}", volume);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int AdjustPlaybackSignalVolume (int volume)
		{
			if (volume < 0)
				volume = 0;
			else if (volume > 400)
				volume = 400;

			string parameters = doFormat ("{{\"che.audio.playout.signal.volume\":{0}}}", volume);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int StartAudioMixing (string filePath, bool loopback, bool replace, int cycle, int playTime = 0)
		{
			return AgoraRtcEngineForGaming_startAudioMixing (filePath, loopback ? 1 : 0, replace ? 1 : 0, cycle, playTime);
		}

		public override int StopAudioMixing()
		{
			return AgoraRtcEngineForGaming_stopAudioMixing ();
		}

		public override int ResumeAudioMixing()
		{
			string parameters = doFormat ("{{\"che.audio.pause_file_as_playout\":false}}");
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int PauseAudioMixing()
		{
			string parameters = doFormat ("{{\"che.audio.pause_file_as_playout\":true}}");
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int AdjustAudioMixingVolume (int volume)
		{
			string parameters = doFormat ("{{\"che.audio.set_file_as_playout_volume\":{0}}}", volume);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int GetAudioMixingDuration ()
		{
			return AgoraRtcEngineForGaming_getIntParameter ("che.audio.get_mixing_file_length_ms", null);
		}

		public override int GetAudioMixingCurrentPosition ()
		{
			return AgoraRtcEngineForGaming_getIntParameter ("che.audio.get_mixing_file_played_ms", null);
		}

		public override int StartAudioRecording (string filePath)
		{
			string parameters = doFormat ("{{\"che.audio.start_recording\":\"{0}\"}}", filePath);
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override int StopAudioRecording ()
		{
			string parameters = doFormat ("{{\"che.audio.stop_recording\":\"{0}\"}}");
			return AgoraRtcEngineForGaming_setParameters (parameters);
		}

		public override IAudioEffectManager GetAudioEffectManager ()
		{
			return mAudioEffectM;
		}

		private EngineMessageData parseEngineMessage (byte[] buffer, int length)
		{
			int startIdx = 0;
			if (length - startIdx < sizeof(Int32)) {
				Debug.LogError (doFormat ("no data or broken data {0}", length));
				return null;
			}

			mPollMsg.clear ();
			mPollMsg.what = BitConverter.ToInt32 (buffer, startIdx);
			startIdx += sizeof(Int32);
			mPollMsg.intArg0 = BitConverter.ToInt32 (buffer, startIdx);
			startIdx += sizeof(Int32);
			mPollMsg.intArg1 = BitConverter.ToInt32 (buffer, startIdx);
			startIdx += sizeof(Int32);
			mPollMsg.intArg2 = BitConverter.ToInt32 (buffer, startIdx);
			startIdx += sizeof(Int32);

			int strLen = BitConverter.ToInt32 (buffer, startIdx);
			startIdx += sizeof(Int32);

			if (strLen == 0) {
				mPollMsg.strArg = "";
			} else {
				byte[] byteStr = new byte[strLen];
				Array.Copy (buffer, startIdx, byteStr, 0, strLen);
				mPollMsg.strArg = System.Text.Encoding.Default.GetString (byteStr);
			}
			startIdx += strLen;

			mPollMsg.extraLength = BitConverter.ToInt32 (buffer, startIdx);
			startIdx += sizeof(Int32);

			if (mPollMsg.extraLength > 0) {
				Array.Copy(buffer, startIdx, mPollMsg.extra, 0, mPollMsg.extraLength);
			}

			return mPollMsg;
		}

		public static IRtcEngineForGaming getEngine (string appId)
		{
			if (instance == null) {
				instance = new RtcEngineForGaming (appId);
			}
			return instance;
		}

		private static IRtcEngineForGaming instance = null;
	}
}
