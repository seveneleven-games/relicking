using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
public class AndroidPluginManager
{
    private AndroidJavaObject unityActivity;
    private AndroidJavaObject unityContext;
    private AndroidJavaObject permissionHelper;

    // AndroidPluginManager 생성자
    public AndroidPluginManager()
    {
        Initialize();
    }

    // Android 관련 객체 초기화
    private void Initialize()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            unityContext = unityActivity.Call<AndroidJavaObject>("getApplicationContext");
        }
        permissionHelper = new AndroidJavaObject("com.ssafy.idlearr.PermissionIdleHelper", unityContext);
    }

    // 권한 체크 및 요청
    public void CheckAndRequestPermissions()
    {
        bool hasPermissions = permissionHelper.Call<bool>("checkPermissions");
        if (!hasPermissions)
        {
            //permissionHelper.Call("showPermissionModal");
            Debug.Log("허용 받으셔 안받음 !");
        }
        else
        {
            Debug.Log("허용 다받음 !");
        }
    }

    // Android에서 Idle 서비스 시작
    public void StartIdleService()
    {
        using (var idleService = new AndroidJavaClass("com.ssafy.idlearr.IdleService"))
        {
            idleService.CallStatic("startService", unityContext);
        }
    }

    // Android에서 Idle 서비스 중지
    public void StopIdleService()
    {
        using (var idleService = new AndroidJavaClass("com.ssafy.idlearr.IdleService"))
        {
            idleService.CallStatic("stopService", unityContext);
        }
    }
}
