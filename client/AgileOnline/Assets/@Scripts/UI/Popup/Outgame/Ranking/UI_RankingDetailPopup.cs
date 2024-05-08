using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;

#region 랭킹 상세 조회 (DetailRankingDataRes)

[Serializable]
public class DetailRankingDataRes
{
    public int status;
    public string message;
    public RankingDetail data;
}

[Serializable]
public class RankingDetail
{
    public int eliteKill;
    public int normalKill;
    public List<RelicDetail> relicList;
    public List<SkillDetail> skillList;
}

[Serializable]
public class RelicDetail
{
    public int relicNo;
    public int level; 
    public int slot; 
}

[Serializable]
public class SkillDetail
{
    public int skillNo;
    public int level; 
    public int slot; 
}

#endregion

public class UI_RankingDetailPopup : UI_Popup
{
    #region Enum

    enum EGameObjects
    {
        ContentObject,
        RelicListObject,
        SkillListObject,
    }

    enum EButtons
    {
        ButtonBG,
        CloseButton,
    }

    enum ETexts
    {
        TitleText,
        PlayerName,
        RecordDate,
        DifficultyText,
        EliteKillText,
        KillText,
    }

    enum EImages
    {
        ClassImage,
    }

    #endregion
    
    // 객체 관련
    private MyRankingInfo _myRankingInfo;

    private int _recordId; // 이걸 둔 이유는 내 랭크 조회와 다른 사람 랭크 조회로 함수가 둘로 나눠짐을 방지하기 위해.
    
    private DetailRankingDataRes _detailRankingDataRes;
    
    // 초기 세팅
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Object Bind

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));

        GetButton((int)EButtons.ButtonBG).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)EButtons.CloseButton).gameObject.BindEvent(OnClickCloseButton);


        #endregion

        Refresh();

        return true;
    }


    public void SetMyRankingInfo(MyRankingInfo myRankingInfo)
    {
        _myRankingInfo = myRankingInfo;
        _recordId = myRankingInfo.recordId;
        Init();
        UIRefresh();

    }

    void UIRefresh()
    {
        GetDetailRankingInfo(_recordId);
        
        // 여기부터 해야됨!!!!
        
    }
    
    void GetDetailRankingInfo(int recordId)
    {
        StartCoroutine(JWTGetRequest($"rankings/{recordId}", res =>
        {
            // json -> 객체로 변환
            _detailRankingDataRes = JsonUtility.FromJson<DetailRankingDataRes>(res);


        }));
    }
    
    
    // 갱신
    void Refresh()
    {
        
    }

    void OnClickCloseButton()
    {
        Debug.Log("CloseRankingDetail");
        Managers.UI.ClosePopupUI(this);
    }
}
