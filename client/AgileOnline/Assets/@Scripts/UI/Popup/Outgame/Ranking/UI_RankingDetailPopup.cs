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

    // 이 부분 버튼을 통해서 눌렀을 때 recordId 값을 전달해주는 식으로 구현하기
    void GetDetailRankingInfo(int recordId)
    {
        StartCoroutine(JWTGetRequest($"rankings/{recordId}", res =>
        {
            // json -> 객체로 변환
            DetailRankingDataRes detailRankingDataRes = JsonUtility.FromJson<DetailRankingDataRes>(res);
            
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
