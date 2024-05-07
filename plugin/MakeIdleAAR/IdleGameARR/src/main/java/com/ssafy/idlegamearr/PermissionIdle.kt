package com.ssafy.idlegamearr

import android.app.AlertDialog
import android.app.AppOpsManager
import android.content.Context
import android.content.Intent
import android.net.Uri
import android.os.Build
import  android.os.Process
import android.provider.Settings
import android.view.LayoutInflater
import android.widget.Button
import androidx.annotation.RequiresApi

class PermissionIdleHelper (private val context : Context){
    // 두가지 권한을 모두 확인하는 메소드 -> 각각 확인하지 않나여?
    @RequiresApi(Build.VERSION_CODES.Q)
    fun checkPermissions(): Boolean{
        val overlayAllowed = Settings.canDrawOverlays(context)
        val usageAllowed = checkUsageStatsPermission()
        return overlayAllowed && usageAllowed
    }

    // 사용 정보 접근 허용
    @RequiresApi(Build.VERSION_CODES.Q)
    private fun checkUsageStatsPermission(): Boolean {
        val appOps = context.getSystemService(Context.APP_OPS_SERVICE) as AppOpsManager
        val mode = appOps.unsafeCheckOpNoThrow(AppOpsManager.OPSTR_GET_USAGE_STATS, Process.myUid(), context.packageName)
        return mode == AppOpsManager.MODE_ALLOWED
    }

    @RequiresApi(Build.VERSION_CODES.Q)
    fun showPermissionModal(){
        // 권한 상태 미리 확인
        val overlayPermissionGranted = Settings.canDrawOverlays(context)
        val usageStatsPermissionGranted = checkUsageStatsPermission()

        // 첫 번째 권한이 필요한 경우
        if (!overlayPermissionGranted) {
            showOverlayPermissionDialog(usageStatsPermissionGranted)
        } else if (!usageStatsPermissionGranted) {  // 두 번째 권한만 필요한 경우
            showUsageAccessPermissionDialog()
        }
    }

    fun showOverlayPermissionDialog(usageStatsPermissionGranted: Boolean) {
        val inflater = context.getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater
        val view = inflater.inflate(R.layout.view_modal, null)

        val dialog = AlertDialog.Builder(context)
            .setView(view)
            .create()

        // 설정 버튼에 리스너 설정
        val confirmButton = view.findViewById<Button>(R.id.secondConfirmButton)
        confirmButton.setOnClickListener {
            // 다른 앱 위에 표시 권한 설정 페이지로 이동
            val intent = Intent(Settings.ACTION_MANAGE_OVERLAY_PERMISSION, Uri.parse("package:" + context.packageName))
            context.startActivity(intent)
            dialog.dismiss() // 다이얼로그 닫기
        }

        dialog.setOnDismissListener {
            // 첫 번째 권한 요청에 대한 모달이 닫힌 후, 두 번째 권한이 필요한 경우 요청 진행
            if (!usageStatsPermissionGranted) {
                showUsageAccessPermissionDialog()
            }
        }

        dialog.show()
    }

    // 먼가 xml 코드 id 이름이 바뀐듯..
    fun showUsageAccessPermissionDialog() {
        // 사용정보 접근 허용 모달 생성 코드
        // 인텐트로 설정 화면 열기
        // 두 번째 권한 요청 다이얼로그 생성
        val inflater = context.getSystemService(Context.LAYOUT_INFLATER_SERVICE) as LayoutInflater
        val view = inflater.inflate(R.layout.view_second_modal, null)

        val dialog = AlertDialog.Builder(context)
            .setView(view)
            .create()

        // 설정 버튼에 리스너 설정
        val confirmButton = view.findViewById<Button>(R.id.confirmButton)
        confirmButton.setOnClickListener {
            // 사용 정보 접근 권한 설정 페이지로 이동
            val intent = Intent(Settings.ACTION_USAGE_ACCESS_SETTINGS)
            context.startActivity(intent)
            dialog.dismiss() // 다이얼로그 닫기
        }

        dialog.show()
    }


}