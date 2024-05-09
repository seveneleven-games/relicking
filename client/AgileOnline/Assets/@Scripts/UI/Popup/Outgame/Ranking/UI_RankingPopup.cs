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
    public StageRanking stage1;
    public StageRanking stage2;
    public StageRanking stage3;
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

    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Object Bind

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));

        GetButton((int)EButtons.StageSelectButton).gameObject.BindEvent(OnClickStageSelectButton);

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
            SetMyInfo();
            GenerateRankingList();
        }));
    }

    // 자기 정보 가져오기
    void SetMyInfo()
    {
        GetText((int)ETexts.MyRank).text = $"{_rankingDataRes.data.stage1.myRank.rank}";
        GetText((int)ETexts.MyNickName).text = _rankingDataRes.data.stage1.myRank.nickname;
        GetText((int)ETexts.MyClass).text = Managers.Data.PlayerDic[_rankingDataRes.data.stage1.myRank.classNo].Name;
        GetText((int)ETexts.MyDifficulty).text = $"{_rankingDataRes.data.stage1.myRank.difficulty}";
        GetButton((int)EButtons.MyRankingButton).gameObject.BindEvent(() => OnClickRankingDetailButton(_rankingDataRes.data.stage1.myRank));
    }
    
    
    // 랭킹 리스트 만들기
    void GenerateRankingList()
    {
        {
            GameObject container = GetObject((int)EGameObjects.Content);
            container.DestroyChilds();

            StageRanking stageRanking = _rankingDataRes.data.stage1;
            
            for (int i = 0; i < stageRanking.rankList.Count; i++)
            {
                RankingInfo info = stageRanking.rankList[i];
                
                int rank = i + 1; // 등수 만들어주기.
                
                UI_RankingObject item = Managers.Resource.Instantiate("UI_RankingObject", pooling: true)
                    .GetOrAddComponent<UI_RankingObject>();
                item.transform.SetParent(container.transform);
                
                item.GetComponent<UI_RankingObject>().SetInfo(info, rank);
            }
        }
    }

    void Refresh()
    {

    }

    void OnClickStageSelectButton()
    {
        Debug.Log("StageSelect");
    }

    void OnClickRankingDetailButton(MyRankingInfo myRankingInfo)
    {
        Debug.Log("RankingDetail");
        UI_RankingDetailPopup popup = Managers.UI.ShowPopupUI<UI_RankingDetailPopup>();
        // 디테일 쪽에 내 랭킹 정보 그대로 보내줘야됨....(정보를 쬐매만 줌.)
        popup.SetMyRankingInfo(myRankingInfo);
    }
}
