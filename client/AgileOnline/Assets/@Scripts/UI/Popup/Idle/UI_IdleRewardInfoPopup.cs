using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static Define;
using static Util;

#region 보상 관련

[Serializable]
public class IdleDataReq
{
    public int lockTime;
}

[Serializable]
public class IdleDataRes
{
    public int status;
    public string message;
    public IdleRewardData data;
}

[Serializable]
public class IdleRewardData
{
    public int earnedGacha;
    public int bonusGacha;
    public int gachaAfterLock;
}

#endregion

public class UI_IdleRewardInfoPopup: UI_Popup
{
     #region Enum

    enum EGameObjects
    {
        TicketCounter,
    }

    enum EButtons
    {
        CloseButton,
    }

    enum ETexts
    {
        TotalIdleContent,
        RewardBonusText,
        Tickets,
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

        GetButton((int)EButtons.CloseButton).gameObject.BindEvent(OnClickCloseButton);

        #endregion

        PostIdleReward();
        
        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();

        return true;
    }

    void PostIdleReward()
    {
        // get 성장 통계
        // StartCoroutine(JWTPostRequest("lock", res =>
        // {
        //     // json -> 객체로 변환
        //     GrowthStaticDataRes growthDataRes = GrowthStaticDataRes.FromJson(res);
        //
        //     // 잘 왔으면 처리
        //     if (growthDataRes.status == 200)
        //     {
        //         // UI 업데이트 해주기 -> 근데.. 이거 화면 들어올때마다 업데이트 하는 메소드를 따로 생각해야할 듯 싶어용
        //         UpdateGrowthUI(growthDataRes.data);
        //     }
        // }));

        IdleDataReq idleDataReq = new IdleDataReq
        {
            lockTime = Managers.Game.idleRewardTime
        };
        
        string idleJsonData = JsonUtility.ToJson(idleDataReq);

        StartCoroutine(JWTPostRequest("lock/end", idleJsonData, res =>
        {
            // json -> 객체로 변환
            IdleDataRes idleDataRes = JsonUtility.FromJson<IdleDataRes>(res);

                
            if (idleDataRes.status == 200)
            {
                UpdateRewardUI(idleDataRes.data);
            }
                
        }));
    }
    
    void UpdateRewardUI(IdleRewardData data)
    {
        GetText((int)ETexts.TotalIdleContent).text = FormatTime(Managers.Game.idleRewardTime);
        GetText((int)ETexts.RewardBonusText).text = $"스트릭 누적 보너스 {data.bonusGacha}%";
        GetText((int)ETexts.Tickets).text = data.earnedGacha.ToString();

        if (data.bonusGacha > 0)
        {
            // 보너스 효과가 0보다 클 때만 보너스를 받는다는 텍스트 활성화
            GetText((int)ETexts.RewardBonusText).gameObject.SetActive(true);
        }
        else
        {
            // 보너스 효과가 0 이하일 때 텍스트 비활성화
            GetText((int)ETexts.RewardBonusText).gameObject.SetActive(false);
        }
    }

    // 갱신
    void Refresh()
    {
    }

    void OnClickCloseButton()
    {
        Debug.Log("종료하기 Clicked");
        Managers.UI.ClosePopupUI(this);
    }
    
    string FormatTime(int totalSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
        return time.ToString(@"hh\:mm\:ss");
    }
}