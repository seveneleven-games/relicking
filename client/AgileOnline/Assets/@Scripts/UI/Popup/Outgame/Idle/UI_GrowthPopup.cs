using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;

[Serializable]
public class GrowthStaticDataRes
{
    public int status;
    public string message;
    public GrowthStaticRes data;

    // JSON에서 객체로 변환하는 팩토리 메서드 추가했음
    public static GrowthStaticDataRes FromJson(string jsonString)
    {
        var res = JsonUtility.FromJson<GrowthStaticDataRes>(jsonString);
        // 여기에 추가적인 데이터 처리하기 -> 스트릭 관련 
        res.data = GrowthStaticRes.ProcessData(res.data);
        return res;
    }
}

[Serializable]
public class GrowthStaticRes
{
    public int totalLockTime;
    public int continuousLockDate;
    public int todayLockTime;

    // 데이터 처리를 위한 메서드 추가했음
    public static GrowthStaticRes ProcessData(GrowthStaticRes data)
    {
        // 데이터 가공용 ( 스트릭이나 시간 섬세하게 ) 처리 로직
        if (data != null)
        {
            // 여기서 데이터 바꿔주는 로직 추가할 예정 (스트릭, 시간 단위) 
        }

        return data;
    }
}

public class UI_GrowthPopup : UI_Popup
{
    #region Enum

    enum EGameObjects
    {
    }

    enum EButtons
    {
        IdleSettingButton,
        StartIdleButton,
    }

    enum ETexts
    {
        GrowthTitle,
        TotalGrowthContent,
        TodayGrowthContent,
        OneWeekStreakDays,
        StreakBonusText,
    }

    enum EImages
    {
        //todo(박설연) : 이걸 오브젝트가 아니라 이미지로 빼서 일주일 단위 스트릭 로직을 추가해야 해여
        StreakGraphContent,
    }

    #endregion

    public void OnDestroy()
    {
        if (Managers.Game != null)
            Managers.Game.OnResourcesChanged -= Refresh;
    }
    
    
    private AndroidJavaObject UnityInstance;
    private AndroidJavaObject UnityActivity;

    // 초기 세팅
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Object Bind

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));
        BindImage(typeof(EImages));

        GetButton((int)EButtons.IdleSettingButton).gameObject.BindEvent(OnClickIdleSettingButton);
        GetButton((int)EButtons.StartIdleButton).gameObject.BindEvent(OnClickStartIdleButton);
        
        // 처음엔 전부 비활성화 상태로
        GetText((int)ETexts.TotalGrowthContent).gameObject.SetActive(false);
        GetText((int)ETexts.TodayGrowthContent).gameObject.SetActive(false);
        GetText((int)ETexts.OneWeekStreakDays).gameObject.SetActive(false);
        GetText((int)ETexts.StreakBonusText).gameObject.SetActive(false);
        GetImage((int)EImages.StreakGraphContent).gameObject.SetActive(false);
        

        #endregion

        GetGrowth();

        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();
        
  
        // AndroidJavaClass helperClass = new AndroidJavaClass("com.ssafy.idlearr.PermissionIdleHelper");
        // AndroidJavaObject activity = new AndroidJavaObject("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        // AndroidJavaObject instance = helperClass.CallStatic<AndroidJavaObject>("getInstance", activity);
        //
        // if (instance != null)
        // {
        //     Debug.Log("fdfjsakjdsafj러아럼ㄴ");
        // }
       
       // Managers.Android.CheckAndRequestPermissions();

        return true;
    }

    void GetGrowth()
    {
        // get 성장 통계
        StartCoroutine(JWTGetRequest("lock", res =>
        {
            // json -> 객체로 변환
            GrowthStaticDataRes growthDataRes = GrowthStaticDataRes.FromJson(res);

            // 잘 왔으면 처리
            if (growthDataRes.status == 200)
            {
                // UI 업데이트 해주기 -> 근데.. 이거 화면 들어올때마다 업데이트 하는 메소드를 따로 생각해야할 듯 싶어용
                UpdateGrowthUI(growthDataRes.data);
            }
        }));
    }

    // 갱신
    void Refresh()
    {
    }

    void UpdateGrowthUI(GrowthStaticRes data)
    {
        
        GetText((int)ETexts.TotalGrowthContent).text = FormatTime(data.totalLockTime);
        GetText((int)ETexts.TodayGrowthContent).text = FormatTime(data.todayLockTime);
        GetText((int)ETexts.OneWeekStreakDays).text = $"{data.continuousLockDate}일";
        
        GetText((int)ETexts.TotalGrowthContent).gameObject.SetActive(true);
        GetText((int)ETexts.TodayGrowthContent).gameObject.SetActive(true);
        GetText((int)ETexts.OneWeekStreakDays).gameObject.SetActive(true);

        if (data.continuousLockDate > 0)
        {
            // 스트릭 일수가 0보다 클 때만 보너스를 받는다는 텍스트 활성화
            GetText((int)ETexts.StreakBonusText).text = $"스트릭 누적 보너스 +{data.continuousLockDate}";
            GetText((int)ETexts.StreakBonusText).gameObject.SetActive(true);
        }
        else
        {
            // 스트릭 일수가 0 이하일 때 텍스트 비활성화
            GetText((int)ETexts.StreakBonusText).gameObject.SetActive(false);
            GetImage((int)EImages.StreakGraphContent).gameObject.SetActive(false);
        }
    }

    void OnClickIdleSettingButton()
    {
        Debug.Log("잠금 앱 설정 버튼 Clicked");
        Managers.UI.ShowPopupUI<UI_ToBeContinuedPopup>();
    }

    void OnClickStartIdleButton()
    {
        Debug.Log("성장하러 가기(방치) 버튼 Clicked");
        // 여기서 플러그인 허용 요청 -> 요청이 모두 잘 오면 화면 이동하기
        // if (Managers.Android.AllPermissionFlag)
        // {
        //     Debug.Log("허용 잘받앗다 더블체크");
        // }
        // else
        // {
        //     Debug.Log("잘 받긴햇는데 허용은 아님요 ");
        // }

        // if (UnityInstance == null)
        // {
        //     Debug.Log("널이다");
        // }
        // Debug.Log(UnityInstance.ToString());
        
        Managers.Scene.LoadScene(Define.EScene.IdleScene);
    }

    string FormatTime(int totalSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
        return time.ToString(@"hh\:mm\:ss");
    }
    
}