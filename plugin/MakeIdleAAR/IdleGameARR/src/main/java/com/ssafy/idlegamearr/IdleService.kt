package com.ssafy.idlegamearr

import android.app.Service
import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.os.IBinder


import com.unity3d.player.UnityPlayer.currentActivity

class IdleService : Service() {

    private val screenReceiver = ScreenUsageReceiver()

    // 앱 백그라운드에서 sticky하게 동작하도록 서비스 구현

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        return START_STICKY
    }

    override fun onCreate() {
        super.onCreate()
        // 앱 감지를 위한 리시버 등록
        val filter = IntentFilter().apply {
            addAction(Intent.ACTION_SCREEN_ON)
            addAction(Intent.ACTION_USER_PRESENT)
            addAction(Intent.ACTION_CLOSE_SYSTEM_DIALOGS)
        }
        registerReceiver(screenReceiver, filter)
    }





    override fun onBind(intent: Intent?): IBinder? {
        return null
    }

    fun stopService() {
        stopSelf()  // 현재 인스턴스의 서비스를 종료
    }
}

class ScreenUsageReceiver : BroadcastReceiver() {
    override fun onReceive(context: Context, intent: Intent) {
        if (intent.action == Intent.ACTION_SCREEN_ON ||
            intent.action == Intent.ACTION_USER_PRESENT ||
            intent.action == Intent.ACTION_CLOSE_SYSTEM_DIALOGS) {
            // 유니티 앱으로 강제 전환
            Intent(context, currentActivity::class.java).apply {
                flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TOP
                context.startActivity(this)
            }
        }
    }
}