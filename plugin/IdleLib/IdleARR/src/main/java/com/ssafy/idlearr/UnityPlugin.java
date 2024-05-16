package com.ssafy.idlearr;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.util.Log;

public class UnityPlugin {

    public static void startForegroundService(Activity activity) {
        Intent intent = new Intent(activity, OverlayService.class);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            activity.startForegroundService(intent);
        } else {
            activity.startService(intent);
        }
    }

    public static void stopForegroundService(Activity activity) {
        Intent intent = new Intent(activity, OverlayService.class);
        activity.stopService(intent);
    }
}
