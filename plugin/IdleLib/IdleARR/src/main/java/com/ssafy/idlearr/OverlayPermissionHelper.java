package com.ssafy.idlearr;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.provider.Settings;
import android.util.Log;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;

public class OverlayPermissionHelper {
    private static final int OVERLAY_PERMISSION_REQ_CODE = 1234;
    private final Activity activity;

    public OverlayPermissionHelper(Activity activity) {
        this.activity = activity;
    }

    public void checkAndRequestOverlayPermission() {
        if (!Settings.canDrawOverlays(activity)) {
            new AlertDialog.Builder(activity)
                    .setTitle("오버레이 권한 요청")
                    .setMessage("이 앱은 다른 앱 위에 표시 권한이 필요합니다. 설정에서 권한을 허용해 주세요.")
                    .setPositiveButton("설정", (dialog, which) -> {
                        Intent intent = new Intent(Settings.ACTION_MANAGE_OVERLAY_PERMISSION, Uri.parse("package:" + activity.getPackageName()));
                        activity.startActivityForResult(intent, OVERLAY_PERMISSION_REQ_CODE);
                        Toast.makeText(activity, activity.getPackageName(), Toast.LENGTH_SHORT).show();
                    })
                    .setNegativeButton("취소", (dialog, which) -> {
                        Toast.makeText(activity, "다른 앱 위에 표시 권한이 필요합니다.", Toast.LENGTH_SHORT).show();
                        Log.d("OverlayPermissionHelper", "취소버튼클릭함");
                        notifyUnity(false);
                    })
                    .show();
        } else {
            notifyUnity(true);
        }
    }

    private void notifyUnity(boolean hasPermission) {
        String result = hasPermission ? "true" : "false";
        UnityPlayer.UnitySendMessage("UI_GrowthPopup", "OnOverlayPermissionResult", result);
    }

    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == OVERLAY_PERMISSION_REQ_CODE) {
            if (Settings.canDrawOverlays(activity)) {
                notifyUnity(true);
            } else {
                notifyUnity(false);
            }
        }
    }
}
