package io.agora.gaming.unity3d;

import android.app.Activity;

import java.util.Locale;

import io.agora.rtc.Constants;
import io.agora.rtc.RtcEngineForGaming;

public class AgoraGamingRtcSDKWrapper {

    private static final String TAG = AgoraGamingRtcSDKWrapper.class.getSimpleName();

    private Activity context;
    private static RtcEngineForGaming rtcEngine = null;

    public AgoraGamingRtcSDKWrapper(Activity context) {
        this.context = context;
    }

    public void initRtcEngine(String appId) {
        if (rtcEngine != null) {
            return;
        }
        rtcEngine = RtcEngineForGaming.create(context.getApplicationContext(), appId, null);
    }
}
