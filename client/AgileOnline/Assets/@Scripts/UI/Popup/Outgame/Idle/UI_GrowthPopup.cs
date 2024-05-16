using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        //IdleSettingButton,
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
        OneWeekStreakGraph,
        StreakGraphContent,
    }

    #endregion

    public void OnDestroy()
    {
        if (Managers.Game != null)
            Managers.Game.OnResourcesChanged -= Refresh;
    }
    
    
    private static AndroidJavaObject plugin;


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

        //GetButton((int)EButtons.IdleSettingButton).gameObject.BindEvent(OnClickIdleSettingButton);
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
        
        if (plugin == null)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                plugin = new AndroidJavaObject("com.ssafy.idlearr.OverlayPermissionHelper", currentActivity);
            }
        }

        return true;
    }
    
    public void CheckAndRequestOverlayPermission()
    {
        plugin.Call("checkAndRequestOverlayPermission");
    }

    // 권한 결과를 처리하는 메서드 (안드로이드에서 Unity로 호출)
    public void OnOverlayPermissionResult(string result)
    {
        if (result == "true")
        {
            Debug.Log("오버레이 권한이 허용되었습니다.");
            // 권한이 허용되었을 때 특정 씬으로 이동
            Managers.Scene.LoadScene(Define.EScene.IdleScene);
        }
        else
        {
            Debug.Log("오버레이 권한이 거부되었습니다.");
            // 권한이 거부되었을 때 사용자에게 알림
        }
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

            MakeGraph(data.continuousLockDate);
        }
        else
        {
            // 스트릭 일수가 0 이하일 때 텍스트 비활성화
            GetText((int)ETexts.StreakBonusText).gameObject.SetActive(false);
            GetImage((int)EImages.StreakGraphContent).gameObject.SetActive(false);
        }
    }

    void MakeGraph(int data)
    {
        
        if (data < 7)
        {
            Image component = GetImage((int)EImages.OneWeekStreakGraph).GetComponent<Image>();
            float mywidth = component.rectTransform.rect.width - 40;
            
            float segmentWidth = mywidth / 7.0f;
            float childWidth = segmentWidth * data;
            
            
            // RectTransform 가져오기
            Image currentImage = GetImage((int)EImages.StreakGraphContent).GetComponent<Image>();

            // 현재 높이 유지
            float currentHeight = currentImage.rectTransform.sizeDelta.y;

            // 너비 설정
            currentImage.rectTransform.sizeDelta = new Vector2(childWidth, currentHeight);
            
            GetImage((int)EImages.StreakGraphContent).gameObject.SetActive(true);
            

        }
        else
        {
            Image component = GetImage((int)EImages.OneWeekStreakGraph).GetComponent<Image>();
            float mywidth = component.rectTransform.rect.width - 40;
            
            Image currentImage = GetImage((int)EImages.StreakGraphContent).GetComponent<Image>();
            float currentHeight = currentImage.rectTransform.sizeDelta.y;
            currentImage.rectTransform.sizeDelta = new Vector2(mywidth, currentHeight);
            
            GetImage((int)EImages.StreakGraphContent).gameObject.SetActive(true);
        }
    }

    // void OnClickIdleSettingButton()
    // {
    //     Debug.Log("잠금 앱 설정 버튼 Clicked");
    //     Managers.UI.ShowPopupUI<UI_ToBeContinuedPopup>();
    // }

    
    void OnClickStartIdleButton()
    {
        Managers.Sound.PlayButtonClick();
        Debug.Log("성장하러 가기(방치) 버튼 Clicked");

        CheckAndRequestOverlayPermission();
    }

    string FormatTime(int totalSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
        return time.ToString(@"hh\:mm\:ss");
    }
    
}