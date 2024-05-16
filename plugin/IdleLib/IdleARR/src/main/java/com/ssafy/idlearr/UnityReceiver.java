package com.ssafy.idlearr;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.os.Looper;
import android.util.Log;
import android.os.Handler;
import com.unity3d.player.UnityPlayer;

public class UnityReceiver extends BroadcastReceiver {
    @Override
    public void onReceive(Context context, Intent intent) {
        //Handler Handler = new Handler(Looper(GetMain));
        Intent launchIntent = new Intent(context, UnityPlayer.currentActivity.getClass());
        launchIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
        context.startActivity(launchIntent);
        Log.d("UnityReceiver", "현재 유니티 액티비티 확인 : " + (UnityPlayer.currentActivity != null ? UnityPlayer.currentActivity.getClass().getName() : "null"));
        Log.d("UnityReceiver", "리시버 알림 받음 ");
    }
}


