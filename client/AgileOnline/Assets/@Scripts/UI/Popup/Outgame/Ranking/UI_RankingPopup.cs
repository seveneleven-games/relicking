using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;



#region 랭킹 조회 (RankingDataRes)

[Serializable]
public class RankingDataRes
{
    public int status;
    public string message;
    public AllRankingData data;
}

[Serializable]
public class AllRankingData
{
    public Dictionary<Stage, StageRanking> stages = new Dictionary<Stage, StageRanking>();
}

public enum Stage
{
    Stage1,
    Stage2,
    Stage3
}

[Serializable]
public class StageRanking
{
    public MyRankingInfo myRank;
    public List<RankingInfo> rankList;
}

[Serializable]
public class MyRankingInfo
{
    public int recordId;
    public int rank;
    public string nickname;
    public int classNo;
    public string difficulty;
}

[Serializable]
public class RankingInfo
{
    public int recordId;
    public string nickname;
    public int classNo;
    public string difficulty;
}

#endregion

public class UI_RankingPopup : UI_Popup
{
    
    [SerializeField]
    GameObject RankingObject;
    
    #region Enum

    enum EGameObjects
    {
        ContentObject,
        Content,
    }

    enum EButtons
    {
        StageSelectButton,
        MyRankingButton,
    }

    enum ETexts
    {
        StageText,
        MyRank,
        MyNickName,
        MyClass,
        MyDifficulty,
    }

    #endregion
    
    // 객체관련 두는 곳
    RankingDataRes _rankingDataRes;

    private Stage currentStage = Stage.Stage1; // 임시
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Object Bind

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));

        GetButton((int)EButtons.StageSelectButton).gameObject.BindEvent(OnClickStageSelectButton);
        GetButton((int)EButtons.MyRankingButton).gameObject.BindEvent(OnClickRankingDetailButton);

        #endregion

        GetRankingInfo();
        Refresh();

        return true;
    }

    void GetRankingInfo()
    {
        StartCoroutine(JWTGetRequest("rankings", res =>
        {
            // json -> 객체로 변환
            _rankingDataRes = JsonUtility.FromJson<RankingDataRes>(res);
        }));
    }

    // 랭킹 리스트 만들기
    void GenerateRankingList()
    {
        if (_rankingDataRes.data.stages.TryGetValue(currentStage, out StageRanking stageRanking))
        {

            GameObject container = GetObject((int)EGameObjects.Content);
            container.DestroyChilds();
            
            
            foreach (RankingInfo info in stageRanking.rankList)
            {
                // 가챠부분 보기.
            }
        }
        else
        {
            Debug.LogError("Selected stage does not exist in the data.");
        }
    }

    void Refresh()
    {

    }

    void OnClickStageSelectButton()
    {
        Debug.Log("StageSelect");
    }

    void OnClickRankingDetailButton()
    {
        Debug.Log("RankingDetail");
        Managers.UI.ShowPopupUI<UI_RankingDetailPopup>();
    }
}
