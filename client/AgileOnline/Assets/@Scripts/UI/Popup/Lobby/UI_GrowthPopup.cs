using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //todo(박설연) : 이걸 오브젝트가 아니라 이미지로 빼서 일주일 단위 스트릭 로직을 추가해야 해여
        StreakGraphContent,
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

        GetButton((int)EButtons.IdleSettingButton).gameObject.BindEvent(OnClickIdleSettingButton);
        GetButton((int)EButtons.StartIdleButton).gameObject.BindEvent(OnClickStartIdleButton);

        #endregion

        GetGrowth();
        
        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();

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
        Debug.Log("데이터 잘 가져왔음 : " + data.continuousLockDate);
        Debug.Log("데이터 잘 가져왔음 : " + data.todayLockTime);
        Debug.Log("데이터 잘 가져왔음 : " + data.totalLockTime);

        GetText((int)ETexts.TotalGrowthContent).text = data.totalLockTime.ToString();
        GetText((int)ETexts.TodayGrowthContent).text = data.todayLockTime.ToString();
        GetText((int)ETexts.OneWeekStreakDays).text = $"{data.continuousLockDate}일";

    }

    void OnClickIdleSettingButton()
    {
        Debug.Log("잠금 앱 설정 버튼 Clicked");
    }

    void OnClickStartIdleButton()
    {
        Debug.Log("성장하러 가기(방치) 버튼 Clicked");
        Managers.Scene.LoadScene(Define.EScene.IdleScene);
    }
    
}