// 안쓰는 코드
// package com.ssafy.idlearr;
//
//import android.app.Service;
//import android.content.Context;
//import android.content.Intent;
//import android.os.IBinder;
//import android.util.Log;
//import com.unity3d.player.UnityPlayer;
//
//public class IdleService extends Service {
//    private static IdleService instance;
//
//    public static void startService(Context context) {
//        if (instance == null) {
//            Intent serviceIntent = new Intent(context, IdleService.class);
//            context.startService(serviceIntent);
//        }
//    }
//
//    public static void stopService(Context context) {
//        if (instance != null) {
//            Intent serviceIntent = new Intent(context, IdleService.class);
//            context.stopService(serviceIntent);
//            instance = null;
//        }
//    }
//
//    @Override
//    public int onStartCommand(Intent intent, int flags, int startId) {
//        if (instance == null) instance = this;
//        return START_STICKY;
//    }
//
//    @Override
//    public void onDestroy() {
//        super.onDestroy();
//        instance = null;
//    }
//
//    @Override
//    public IBinder onBind(Intent intent) {
//        return null;
//    }
//
//    // 앱이 백그라운드로 이동할 때 호출되는 static 메서드
//    public static void onAppBackgrounded(Context context) {
//        if (instance != null) {
//            instance.bringAppToFront(context);
//        }
//    }
//
//    private void bringAppToFront(Context context) {
//        try {
//            Intent intent = new Intent(context, UnityPlayer.currentActivity.getClass());
//            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
//            startActivity(intent);
//        } catch (Exception e) {
//            Log.e("IdleService", "Failed to bring app to front: " + e.getMessage());
//        }
//    }
//}
