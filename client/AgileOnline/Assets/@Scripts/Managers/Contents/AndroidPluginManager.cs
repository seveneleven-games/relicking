using System;
using UnityEngine;

public class AndroidPluginManager
{
    private AndroidJavaObject unityActivity;
    private AndroidJavaObject unityContext;
    private AndroidJavaObject permissionHelper;
    private AndroidJavaObject idleService;

    // AndroidPluginManager 생성자
    public AndroidPluginManager()
    {
        Initialize();
    }

    // Android 관련 객체 초기화
    private void Initialize()
    {
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (unityActivity != null)
                {
                    unityContext = unityActivity.Call<AndroidJavaObject>("getApplicationContext");
                    // using (var helperClass = new AndroidJavaClass("com.ssafy.idlearr.PermissionIdleHelper"))
                    // {
                    //     permissionHelper = helperClass.CallStatic<AndroidJavaObject>("getInstance", unityContext);
                    // }
                }
                else
                {
                    Debug.LogError("currentActivity 참조 실패");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("initialize AndroidPluginManager 실패: " + ex.Message);
        }
    }

    public bool AllPermissionFlag { get; private set; } = false;

    // 권한 체크 및 요청
    public void   CheckAndRequestPermissions()
    {
        bool hasPermissions = permissionHelper.Call<bool>("checkPermissions");
        AllPermissionFlag = hasPermissions;
        if (!hasPermissions)
        {
            // 권한 요청 로직을 활성화시키고 싶다면 주석을 해제하세요.
            //permissionHelper.Call("showPermissionModal");
            Debug.Log("ㅎㅎㅎㅎ");
        }
        else
        {
            Debug.Log("ㅋㅋㅋ");
        }
    }

    //Android에서 Idle 서비스 시작
    public void StartIdleService()
    {
        using (var idleService = new AndroidJavaClass("com.ssafy.idlearr.IdleService"))
        {
            idleService.CallStatic("startService", unityContext);
            Debug.Log("서비스 시작");
        }
    }
    
    // Android에서 Idle 서비스 중지
    public void StopIdleService()
    {
        using (var idleService = new AndroidJavaClass("com.ssafy.idlearr.IdleService"))
        {
            idleService.CallStatic("stopService", unityContext);
            Debug.Log("서비스 중지");
        }
    }

    public void onAppBackgrounded()
    {
        using (var idleService = new AndroidJavaClass("com.ssafy.idlearr.IdleService"))
        {
            idleService.CallStatic("onAppBackgrounded", unityContext);
            Debug.Log("백그라운드감지!");
        }
    }
    
}
