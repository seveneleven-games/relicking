using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static Define;

public class UI_IdleProceedPopup : UI_Popup
{
    #region Enum

    enum EGameObjects
    {
    }

    enum EButtons
    {
        ExitIdleButton,
    }

    enum ETexts
    {
        TotalGrowthContent,
    }

    #endregion

    #region StopWatch

    private float startTime;
    private DateTime savedTime;
    private bool stopwatchActive = false;
    private float elapsedTime = 0f;
    private int lastUpdatedTime = 0; // 마지막 ui 업데이트 시간 저장 
    

    private void Update()
    {
        if (stopwatchActive)
        {
            elapsedTime = Time.time - startTime;
            int seconds = (int)elapsedTime;
            if (seconds != lastUpdatedTime) // 초가 변경될 때만 UI 업데이트
            {
                GetText((int)ETexts.TotalGrowthContent).text = FormatTime(seconds);
                lastUpdatedTime = seconds;
            }
        }

        if (Managers.Game.showIdleRewardPopup)
        {
            Managers.Scene.LoadScene(EScene.LobbyScene);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("뒤로 못돌아간다.");
        }
    }

    public void StartStopwatch()
    {
        if (!stopwatchActive)
        {
            stopwatchActive = true;
            startTime = Time.time;
            lastUpdatedTime = -1; // 초 업데이트 강제 실행을 위해 초기화
        }
    }

    public void StopStopwatch()
    {
        stopwatchActive = false;
        elapsedTime = Time.time - startTime;
        int seconds = (int)elapsedTime;
        Managers.Game.idleRewardTime = seconds;
    }

    public void ResetStopwatch()
    {
        elapsedTime = 0;
        lastUpdatedTime = -1; // 초 업데이트 강제 실행을 위해 초기화
        GetText((int)ETexts.TotalGrowthContent).text = "0"; // UI Text를 0으로 초기화
        if (stopwatchActive)
        {
            startTime = Time.time;
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("백으로 갓단다...");
            savedTime = DateTime.UtcNow;
            Debug.Log("Application paused at: " + savedTime);
            unityPluginClass.CallStatic("startForegroundService", unityActivity);
            Debug.Log("포그라운드 서비스 시작");
        }
        else
        {
            unityPluginClass.CallStatic("stopForegroundService", unityActivity);
            Debug.Log("포그라운드 서비스 중지");
            DateTime currentTime = DateTime.UtcNow;
            TimeSpan pauseDuration = currentTime - savedTime;
            Debug.Log("Application resumed, Pause Duration: " + pauseDuration.TotalSeconds + " seconds");
            if (stopwatchActive)
            {
                elapsedTime += (float)pauseDuration.TotalSeconds;
                Debug.Log($"Updated elapsedTime: {elapsedTime}");
                startTime = Time.time - elapsedTime;
                Debug.Log($"Pause duration: {pauseDuration}, New elapsedTime: {elapsedTime}");
                int seconds = (int)elapsedTime;
                GetText((int)ETexts.TotalGrowthContent).text = FormatTime(seconds);
                lastUpdatedTime = seconds;
            }
        }
    }
    

    #endregion

    public void OnDestroy()
    {
        if (Managers.Game != null)
            Managers.Game.OnResourcesChanged -= Refresh;
        
        unityActivity.Dispose();
        unityPluginClass.Dispose();
    }
    
    private AndroidJavaObject unityActivity;
    private AndroidJavaClass unityPluginClass;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Object Bind

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));

        GetButton((int)EButtons.ExitIdleButton).gameObject.BindEvent(OnClickExitIdleButton);

        #endregion
        
        StartStopwatch();
        
        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();
        
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        unityPluginClass = new AndroidJavaClass("com.ssafy.idlearr.UnityPlugin");

        return true;
    }

    void Refresh()
    {
    }

    void OnClickExitIdleButton()
    {
        StopStopwatch();
        Debug.Log("종료하기 Clicked");
        Managers.UI.ShowPopupUI<UI_IdleRewardInfoPopup>();
    }
    
    string FormatTime(int totalSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
        return time.ToString(@"hh\:mm\:ss");
    }
}
