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

    private int _wantStage = 1; 
    
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

        UI_StageSelectPopup.OnStageSelected += UpdateStage;
        
        GetRankingInfo();
        Refresh();

        return true;
    }
    
    void OnDestroy()
    {
        UI_StageSelectPopup.OnStageSelected -= UpdateStage;
    }
    
    private void UpdateStage(int stage)
    {
        _wantStage = stage;
        Refresh();
        GetRankingInfo();
    }
    
    void GetRankingInfo()
    {
        StartCoroutine(JWTGetRequest("rankings", res =>
        {
            // json -> 객체로 변환
            _rankingDataRes = JsonUtility.FromJson<RankingDataRes>(res);

            GetText((int)ETexts.StageText).text = $"Stage {_wantStage}";
            
            SetMyInfo();
            GenerateRankingList();
        }));
    }

    // 자기 정보 가져오기
    void SetMyInfo()
    {
        StageRanking currentRanking = GetCurrentStageRanking();
        GetText((int)ETexts.MyRank).text = $"{currentRanking.myRank.rank}";
        GetText((int)ETexts.MyNickName).text = currentRanking.myRank.nickname;
        GetText((int)ETexts.MyClass).text = Managers.Data.PlayerDic[currentRanking.myRank.classNo].Name;
        GetText((int)ETexts.MyDifficulty).text = $"{currentRanking.myRank.difficulty}";
        GetButton((int)EButtons.MyRankingButton).gameObject.BindEvent(() => OnClickRankingDetailButton(currentRanking.myRank));
    }
    
    
    StageRanking GetCurrentStageRanking()
    {
        switch (_wantStage)
        {
            case 1:
                return _rankingDataRes.data.stage1;
            case 2:
                return _rankingDataRes.data.stage2;
            case 3:
                return _rankingDataRes.data.stage3;
            default:
                return _rankingDataRes.data.stage1;
        }
    }
    
    // 랭킹 리스트 만들기
    void GenerateRankingList()
    {
        {
            GameObject container = GetObject((int)EGameObjects.Content);
            container.DestroyChilds();

            StageRanking currentRanking = GetCurrentStageRanking();
            
            for (int i = 0; i < currentRanking.rankList.Count; i++)
            {
                RankingInfo info = currentRanking.rankList[i];
                
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
        Managers.UI.ShowPopupUI<UI_StageSelectPopup>();
        
    }

    void OnClickRankingDetailButton(MyRankingInfo myRankingInfo)
    {
        Debug.Log("RankingDetail");
        UI_RankingDetailPopup popup = Managers.UI.ShowPopupUI<UI_RankingDetailPopup>();
        // 디테일 쪽에 내 랭킹 정보 그대로 보내줘야됨....(정보를 쬐매만 줌.) -> 원하는 스테이지 정보도 같이 보내주기!!
        popup.SetMyRankingInfo(myRankingInfo);
    }
}
