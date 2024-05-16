package com.ssafy.idlearr;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Intent;
import android.graphics.Color;
import android.graphics.PixelFormat;
import android.graphics.Typeface;
import android.graphics.drawable.GradientDrawable;
import android.os.IBinder;
import android.view.Gravity;
import android.view.View;
import android.view.ViewTreeObserver;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

import androidx.core.app.NotificationCompat;

import com.unity3d.player.UnityPlayer;

public class OverlayService extends Service {

    private static final String CHANNEL_ID = "OverlayServiceChannel";
    private WindowManager windowManager;
    private View overlayView;

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onCreate() {
        super.onCreate();

        createNotificationChannel();
        startForeground(1, getNotification());

        windowManager = (WindowManager) getSystemService(WINDOW_SERVICE);
        overlayView = createOverlayView();

        int layoutFlag;
        layoutFlag = WindowManager.LayoutParams.TYPE_APPLICATION_OVERLAY;

        WindowManager.LayoutParams params = new WindowManager.LayoutParams(
                WindowManager.LayoutParams.MATCH_PARENT, // 화면 전체 너비
                WindowManager.LayoutParams.MATCH_PARENT, // 화면 전체 높이
                layoutFlag,
                WindowManager.LayoutParams.FLAG_NOT_FOCUSABLE | WindowManager.LayoutParams.FLAG_LAYOUT_NO_LIMITS,
                PixelFormat.TRANSLUCENT);

        params.systemUiVisibility = View.SYSTEM_UI_FLAG_FULLSCREEN | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION | View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY;

        params.gravity = Gravity.TOP | Gravity.LEFT;
        windowManager.addView(overlayView, params);
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        if (overlayView != null) {
            windowManager.removeView(overlayView);
        }
    }

    private Notification getNotification() {
        Intent notificationIntent = new Intent(this, UnityPlayer.currentActivity.getClass());
        PendingIntent pendingIntent = PendingIntent.getActivity(this,
                0, notificationIntent, PendingIntent.FLAG_IMMUTABLE);

        return new NotificationCompat.Builder(this, CHANNEL_ID)
                .setContentTitle("다른 앱 위에 표시 서비스")
                .setContentText("다른 앱 위에 표시 중...")
                .setContentIntent(pendingIntent)
                .build();
    }

    private void createNotificationChannel() {
        NotificationChannel serviceChannel = new NotificationChannel(
                CHANNEL_ID,
                "Overlay Service Channel",
                NotificationManager.IMPORTANCE_DEFAULT
        );
        NotificationManager manager = getSystemService(NotificationManager.class);
        if (manager != null) {
            manager.createNotificationChannel(serviceChannel);
        }
    }

    private View createOverlayView() {
        // LinearLayout 설정
        LinearLayout linearLayout = new LinearLayout(this);
        linearLayout.setOrientation(LinearLayout.VERTICAL);
        linearLayout.setBackgroundColor(0xFF332B1F); // 배경 색
        linearLayout.setGravity(Gravity.CENTER); // 레이아웃 중앙 정렬

        // 설명 텍스트 설정
        TextView textView = new TextView(this);
        textView.setText("앱이 잠금 중입니다.");
        textView.setTextColor(Color.WHITE);
        textView.setTextSize(40);
        textView.setTypeface(null, Typeface.BOLD);
        textView.setGravity(Gravity.CENTER);

        // 텍스트 뷰 레이아웃 파라미터 설정
        LinearLayout.LayoutParams textParams = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.WRAP_CONTENT,
                LinearLayout.LayoutParams.WRAP_CONTENT
        );
        textParams.setMargins(0, 0, 0, 120); // 텍스트 뷰 아래에 마진 추가

        // 버튼 생성 및 스타일 설정
        Button button = new Button(this);
        button.setText("게임으로");
        button.setTextColor(Color.WHITE); // 버튼 텍스트 색상
        button.setTextSize(30); // 버튼 텍스트 크기 설정
        button.setPadding(90, 30, 90, 30); // 버튼 패딩

        // 버튼 배경 설정 (둥근 모서리와 배경 색상)
        GradientDrawable drawable = new GradientDrawable();
        drawable.setShape(GradientDrawable.RECTANGLE);
        drawable.setColor(0xFF4CAF50); // 버튼 배경 색상
        drawable.setCornerRadius(30); // 둥근 모서리 반경 설정
        button.setBackground(drawable);

        // 버튼 클릭 리스너 설정
        button.setOnClickListener(v -> {
            Toast.makeText(OverlayService.this, "게임에 다시 입장!", Toast.LENGTH_SHORT).show();
            Intent intent = new Intent(OverlayService.this, UnityPlayer.currentActivity.getClass());
            intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_SINGLE_TOP);
            startActivity(intent);
            stopSelf(); // 오버레이 서비스 중지
        });

        // 텍스트 뷰와 버튼을 레이아웃에 추가
        linearLayout.addView(textView, textParams);

        LinearLayout.LayoutParams buttonParams = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.WRAP_CONTENT,
                LinearLayout.LayoutParams.WRAP_CONTENT
        );
        linearLayout.addView(button, buttonParams);

        return linearLayout;
    }

}
