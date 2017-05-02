package io.agora.gaming.unity3d;

import android.util.Log;

import com.unity3d.player.UnityPlayer;

public class Logger {
    public static void log(String tag, String message) {
        Log.d(tag, message);
        UnityPlayer.UnitySendMessage("LoggerBridge", "doLog", tag + ": " + message);
    }
}
