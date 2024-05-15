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
                //Debug.Log(seconds);
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

    // 버튼 누르면 스톱워치 정지 -> 모달에 현재 시간 저장해서 보여줌 
    public void StopStopwatch()
    {
        stopwatchActive = false;
        elapsedTime = Time.time - startTime;
        int seconds = (int)elapsedTime;
        // 여기서 멈추면 저장
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
            // 앱이 백그라운드로 갈 때
            Debug.Log("백으로 갓단다...");
            savedTime = DateTime.UtcNow;
            Debug.Log("Application paused at: " + savedTime);
            
            // Android의 startForegroundService 메서드를 호출합니다.
            unityPluginClass.CallStatic("startForegroundService", unityActivity);
            Debug.Log("야플러그인시작이야");
        }
        else
        {
            unityPluginClass.CallStatic("stopForegroundService", unityActivity);
            Debug.Log("플러그인끝");
            DateTime currentTime = DateTime.UtcNow;
            TimeSpan pauseDuration = currentTime - savedTime;
            Debug.Log("Application resumed, Pause Duration: " + pauseDuration.TotalSeconds + " seconds");
            // 앱이 포어그라운드로 돌아왔을 때
            if (stopwatchActive)
            {
                elapsedTime += (float)pauseDuration.TotalSeconds;
                Debug.Log($"Updated elapsedTime: {elapsedTime}");
                startTime = Time.time - elapsedTime; // 백그라운드에서 보낸 시간만큼 elapsedTime에 추가 후 startTime 조정
                Debug.Log($"Pause duration: {pauseDuration}, New elapsedTime: {elapsedTime}");
                int seconds = (int)elapsedTime;
                GetText((int)ETexts.TotalGrowthContent).text = FormatTime(seconds); // UI 업데이트
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

    // 초기 세팅
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
        
        // 안드로이드 서비스 플러그인 실행해주기 
       // Managers.Android.StartIdleService();
        StartStopwatch();
        
        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();
        
        // 화면 안꺼지게
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        
        // AndroidJavaClass를 사용하여 UnityPlayer 클래스에 접근하고, 현재 액티비티를 가져옵니다.
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        // UnityPlugin 클래스에 접근합니다.
        unityPluginClass = new AndroidJavaClass("com.ssafy.idlearr.UnityPlugin");

        

        return true;
    }

    // 갱신
    void Refresh()
    {
    }

    void OnClickExitIdleButton()
    {
        Managers.Sound.Play(Define.ESound.Effect, "LockResult");
        StopStopwatch();
        Debug.Log("종료하기 Clicked");
      //  Managers.Android.StopIdleService();
        
        Managers.UI.ShowPopupUI<UI_IdleRewardInfoPopup>();
        //Managers.Scene.LoadScene(EScene.LobbyScene);
    }
    
    string FormatTime(int totalSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
        return time.ToString(@"hh\:mm\:ss");
    }
}