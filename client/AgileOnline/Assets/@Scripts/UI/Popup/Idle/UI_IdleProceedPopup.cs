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
    private float savedTime;
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
                Debug.Log(seconds);
                GetText((int)ETexts.TotalGrowthContent).text = FormatTime(seconds);
                lastUpdatedTime = seconds;
            }
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
        // 여기서 멈추면 
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
            savedTime = Time.time;
        }
        else if (stopwatchActive)
        {
            // 앱이 포어그라운드로 돌아왔을 때
            startTime += (Time.time - savedTime);
        }
    }

    #endregion

    public void OnDestroy()
    {
        if (Managers.Game != null)
            Managers.Game.OnResourcesChanged -= Refresh;
    }

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

        StartStopwatch();
        
        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();

        return true;
    }

    // 갱신
    void Refresh()
    {
    }

    void OnClickExitIdleButton()
    {
        StopStopwatch();
        //ResetStopwatch();
        
        Debug.Log("종료하기 Clicked");
        Managers.Game.showIdleRewardPopup = true;
        Managers.Scene.LoadScene(EScene.LobbyScene);
    }
    
    string FormatTime(int totalSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
        return time.ToString(@"hh\:mm\:ss");
    }
}