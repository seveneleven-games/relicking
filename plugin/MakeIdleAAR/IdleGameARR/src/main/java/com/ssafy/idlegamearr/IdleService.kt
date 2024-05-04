import android.app.Notification
import android.app.PendingIntent
import android.app.Service
import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.content.IntentFilter
import android.os.IBinder
import androidx.core.app.NotificationCompat
import com.ssafy.idlegamearr.R
import com.unity3d.player.UnityPlayer

class IdleService : Service() {

    private val screenReceiver = ScreenUsageReceiver()
    private val NOTIFICATION_ID = 1
    private val CHANNEL_ID = "channel_01"

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        val notificationIntent = Intent(this, UnityPlayer.currentActivity::class.java)
        val pendingIntent = PendingIntent.getActivity(this, 0, notificationIntent, 0)

        val notification: Notification = NotificationCompat.Builder(this, CHANNEL_ID)
            .setContentTitle("앱이 백그라운드에서 동작 중")
            .setContentText("눌러서 앱으로 돌아가기")
            .setContentIntent(pendingIntent)
            .setTicker("호호홋 세븐일레븐")
            .build()

        startForeground(NOTIFICATION_ID, notification)

        // 앱 감지를 위한 리시버 등록
        val filter = IntentFilter().apply {
            addAction(Intent.ACTION_SCREEN_ON)
            addAction(Intent.ACTION_USER_PRESENT)
            addAction(Intent.ACTION_CLOSE_SYSTEM_DIALOGS)
        }
        registerReceiver(screenReceiver, filter)

        return START_STICKY
    }

    override fun onCreate() {
        super.onCreate()
    }

    override fun onDestroy() {
        super.onDestroy()
        unregisterReceiver(screenReceiver)
        stopForeground(STOP_FOREGROUND_REMOVE)
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
            val unityIntent = Intent(context, UnityPlayer.currentActivity::class.java).apply {
                flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TOP
                putExtra("targetScene", "LockScreen") // LockScreen은 유니티에서 로드하길 원하는 씬의 이름으로 두쟝
            }
            context.startActivity(unityIntent)
        }
    }
}
