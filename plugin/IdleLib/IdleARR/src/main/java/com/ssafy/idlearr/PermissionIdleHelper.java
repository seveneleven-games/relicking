//package com.ssafy.idlearr;
//
//import android.app.AlertDialog;
//import android.app.AppOpsManager;
//import android.content.Context;
//import android.content.Intent;
//import android.graphics.Color;
//import android.net.Uri;
//import android.os.Build;
//import android.os.Process;
//import android.provider.Settings;
//import android.widget.Button;
//import android.widget.LinearLayout;
//import com.google.android.material.textview.MaterialTextView;
//
//public class PermissionIdleHelper {
//    private static PermissionIdleHelper instance;
//    private Context context;
//
//    private PermissionIdleHelper(Context context) {
//        this.context = context;
//    }
//
//    // 싱글톤으로 생성
//    public static synchronized PermissionIdleHelper getInstance(Context context) {
//        if (instance == null) {
//            instance = new PermissionIdleHelper(context);
//        }
//        return instance;
//    }
//
//    // 유니티 호출 메소드 : 허용 두가지 체크
//    public boolean checkPermissions() {
//        boolean overlayAllowed = Settings.canDrawOverlays(context);
//        return overlayAllowed;
////        boolean usageAllowed = checkUsageStatsPermission();
////        return overlayAllowed && usageAllowed;
//    }
//
//    private boolean checkUsageStatsPermission() {
//        AppOpsManager appOps = (AppOpsManager) context.getSystemService(Context.APP_OPS_SERVICE);
//        int mode = appOps.unsafeCheckOpNoThrow(AppOpsManager.OPSTR_GET_USAGE_STATS, Process.myUid(), context.getPackageName());
//        return mode == AppOpsManager.MODE_ALLOWED;
//    }
//
//    public void showPermissionModal() {
//        boolean overlayPermissionGranted = Settings.canDrawOverlays(context);
//       // boolean usageStatsPermissionGranted = checkUsageStatsPermission();
//
//        if (!overlayPermissionGranted) {
//            showOverlayPermissionDialog();
//        }
////        else if (!usageStatsPermissionGranted) {
////            showUsageAccessPermissionDialog();
////        }
//    }
//
//
//    // 여기도 지금 에러 처리용으로 주석처리 해둿어요 권한부터 체크하려고...
//    private void showOverlayPermissionDialog() {
//        LinearLayout layout = createLayout();
//        MaterialTextView titleTextView = createTitleTextView(layout, "사용자 권한 필요");
//        MaterialTextView messageTextView = createMessageTextView(layout, titleTextView.getId(), "이 앱은 다른 앱 위에 표시될 수 있는 권한이 필요합니다. 설정으로 이동하여 권한을 허용해주세요.");
//        Button confirmButton = createButton(layout, messageTextView.getId(), "설정", Settings.ACTION_MANAGE_OVERLAY_PERMISSION);
//        Button cancelButton = createButton(layout, messageTextView.getId(), "취소", null);
//
//        showDialog(layout);
//    }
//
//    private void showUsageAccessPermissionDialog() {
////        LinearLayout layout = createLayout();
////        MaterialTextView titleTextView = createTitleTextView(layout, "사용 정보 접근 권한 필요");
////        MaterialTextView messageTextView = createMessageTextView(layout, titleTextView.getId(), "이 앱은 다른 앱의 사용 정보에 접근할 수 있는 권한이 필요합니다. 설정 화면으로 이동하여 권한을 허용해 주세요.");
////        Button confirmButton = createButton(layout, messageTextView.getId(), "설정", Settings.ACTION_USAGE_ACCESS_SETTINGS);
////        Button cancelButton = createButton(layout, messageTextView.getId(), "취소", null);
////
////        showDialog(layout);
//    }
//
//    private LinearLayout createLayout() {
//        LinearLayout layout = new LinearLayout(context);
//        layout.setOrientation(LinearLayout.VERTICAL);
//        layout.setLayoutParams(new LinearLayout.LayoutParams(
//                LinearLayout.LayoutParams.MATCH_PARENT,
//                LinearLayout.LayoutParams.WRAP_CONTENT));
//        layout.setPadding(24, 24, 24, 24);
//        return layout;
//    }
//
//    private MaterialTextView createTitleTextView(LinearLayout layout, String text) {
//        MaterialTextView textView = new MaterialTextView(context);
//        textView.setLayoutParams(new LinearLayout.LayoutParams(
//                LinearLayout.LayoutParams.MATCH_PARENT,
//                LinearLayout.LayoutParams.WRAP_CONTENT));
//        textView.setText(text);
//        textView.setTextAppearance(context, androidx.appcompat.R.style.TextAppearance_AppCompat_Headline);
//        layout.addView(textView);
//        return textView;
//    }
//
//    private MaterialTextView createMessageTextView(LinearLayout layout, int aboveViewId, String text) {
//        MaterialTextView textView = new MaterialTextView(context);
//        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(
//                LinearLayout.LayoutParams.MATCH_PARENT,
//                LinearLayout.LayoutParams.WRAP_CONTENT);
//        layoutParams.topMargin = 16;
//        textView.setLayoutParams(layoutParams);
//        textView.setText(text);
//        textView.setTextAppearance(context, androidx.appcompat.R.style.TextAppearance_AppCompat_Large);
//        layout.addView(textView);
//        return textView;
//    }
//
//    private Button createButton(LinearLayout layout, int aboveViewId, String text, String action) {
//        Button button = new Button(context);
//        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(
//                LinearLayout.LayoutParams.WRAP_CONTENT,
//                LinearLayout.LayoutParams.WRAP_CONTENT);
//        layoutParams.topMargin = 24;
//        button.setLayoutParams(layoutParams);
//        button.setText(text);
//        button.setTextColor(Color.parseColor("#3F51B5"));
//        button.setTextAppearance(context, androidx.appcompat.R.style.TextAppearance_AppCompat_Button);
//        button.setOnClickListener(v -> {
//            if (action != null) {
//                Intent intent = new Intent(action, Uri.parse("package:" + context.getPackageName()));
//                context.startActivity(intent);
//            }
//        });
//        layout.addView(button);
//        return button;
//    }
//
//    private void showDialog(LinearLayout layout) {
//        AlertDialog.Builder builder = new AlertDialog.Builder(context);
//        builder.setView(layout);
//        AlertDialog dialog = builder.create();
//        dialog.show();
//    }
//}
