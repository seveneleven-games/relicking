using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;
using static Define;

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
    public string updatedDate;
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
        
        // 유물 관련
        LevelText1,
        LevelText2,
        LevelText3,
        LevelText4,
        LevelText5,
        LevelText6,
        
        // 스킬 관련
        SkillLevelText1,
        SkillLevelText2,
        SkillLevelText3,
        SkillLevelText4,
        SkillLevelText5,
        SkillLevelText6,
    }

    enum EImages
    {
        ClassImage,
        
        // 유물 바탕화면 관련
        RelicBGImage1,
        RelicBGImage2,
        RelicBGImage3,
        RelicBGImage4,
        RelicBGImage5,
        RelicBGImage6,
        
        // 유물 이미지 관련
        RelicImage1,
        RelicImage2,
        RelicImage3,
        RelicImage4,
        RelicImage5,
        RelicImage6,
        
        // 스킬 관련
        Image1,
        Image2,
        Image3,
        Image4,
        Image5,
        Image6,
    }

    #endregion
    
    // 객체 관련
    private MyRankingInfo _myRankingInfo;

    private RankingInfo _rankingInfo;

    private int _rank;
    
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
        BindImage(typeof(EImages));

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

    public void SetRankingInfo(RankingInfo rankingInfo, int rank)
    {
        _rankingInfo = rankingInfo;
        _recordId = rankingInfo.recordId;
        _rank = rank;
        Init();
        UIRefresh();
    }

    void UIRefresh()
    {
        StartCoroutine(GetDetailRankingInfo(_recordId, () =>
        {
            if (_myRankingInfo != null)
            {
                GetText((int)ETexts.TitleText).text = $"{_myRankingInfo.rank}위 기록";
                GetText((int)ETexts.PlayerName).text = _myRankingInfo.nickname;
                GetText((int)ETexts.DifficultyText).text = $"{_myRankingInfo.difficulty}";
                
                GetImage((int)EImages.ClassImage).sprite =
                    Managers.Resource.Load<Sprite>(Managers.Data.PlayerDic[_myRankingInfo.classNo].ThumbnailName);
            }
            else
            {
                GetText((int)ETexts.TitleText).text = $"{_rank}위 기록";
                GetText((int)ETexts.PlayerName).text = _rankingInfo.nickname;
                GetText((int)ETexts.DifficultyText).text = $"{_rankingInfo.difficulty}";
                GetImage((int)EImages.ClassImage).sprite =
                    Managers.Resource.Load<Sprite>(Managers.Data.PlayerDic[_rankingInfo.classNo].ThumbnailName);
            }

            GetText((int)ETexts.RecordDate).text = _detailRankingDataRes.data.updatedDate;
            GetText((int)ETexts.EliteKillText).text = $"{_detailRankingDataRes.data.eliteKill}";
            GetText((int)ETexts.KillText).text = $"{_detailRankingDataRes.data.normalKill}";
            
            
            // 유물 관련 갱신
            foreach (RelicDetail relicDetail in _detailRankingDataRes.data.relicList)
            {
                switch (relicDetail.slot)
                {
                    case 1:
                        GetText((int)ETexts.LevelText1).text = $"{relicDetail.level}";
                        GetImage((int)EImages.RelicImage1).sprite = Managers.Resource.Load<Sprite>(Managers.Data.RelicDic[relicDetail.relicNo * 10 + relicDetail.level].ThumbnailName);
                        GetImage((int)EImages.RelicBGImage1).color = GetRelicColorByRarity(Managers.Data.RelicDic[relicDetail.relicNo * 10 + relicDetail.level].Rarity);
                        break;
                    case 2:
                        GetText((int)ETexts.LevelText2).text = $"{relicDetail.level}";
                        GetImage((int)EImages.RelicImage2).sprite = Managers.Resource.Load<Sprite>(Managers.Data.RelicDic[relicDetail.relicNo * 10 + relicDetail.level].ThumbnailName);
                        GetImage((int)EImages.RelicBGImage2).color = GetRelicColorByRarity(Managers.Data.RelicDic[relicDetail.relicNo * 10 + relicDetail.level].Rarity);
                        break;
                    case 3:
                        GetText((int)ETexts.LevelText3).text = $"{relicDetail.level}";
                        GetImage((int)EImages.RelicImage3).sprite = Managers.Resource.Load<Sprite>(Managers.Data.RelicDic[relicDetail.relicNo * 10 + relicDetail.level].ThumbnailName);
                        GetImage((int)EImages.RelicBGImage3).color = GetRelicColorByRarity(Managers.Data.RelicDic[relicDetail.relicNo * 10 + relicDetail.level].Rarity);
                        break;
                    case 4:
                        GetText((int)ETexts.LevelText4).text = $"{relicDetail.level}";
                        GetImage((int)EImages.RelicImage4).sprite = Managers.Resource.Load<Sprite>(Managers.Data.RelicDic[relicDetail.relicNo * 10 + relicDetail.level].ThumbnailName);
                        GetImage((int)EImages.RelicBGImage4).color = GetRelicColorByRarity(Managers.Data.RelicDic[relicDetail.relicNo * 10 + relicDetail.level].Rarity);
                        break;
                    case 5:
                        GetText((int)ETexts.LevelText5).text = $"{relicDetail.level}";
                        GetImage((int)EImages.RelicImage5).sprite = Managers.Resource.Load<Sprite>(Managers.Data.RelicDic[relicDetail.relicNo * 10 + relicDetail.level].ThumbnailName);
                        GetImage((int)EImages.RelicBGImage5).color = GetRelicColorByRarity(Managers.Data.RelicDic[relicDetail.relicNo * 10 + relicDetail.level].Rarity);
                        break;
                    case 6:
                        GetText((int)ETexts.LevelText6).text = $"{relicDetail.level}";
                        GetImage((int)EImages.RelicImage6).sprite = Managers.Resource.Load<Sprite>(Managers.Data.RelicDic[relicDetail.relicNo * 10 + relicDetail.level].ThumbnailName);
                        GetImage((int)EImages.RelicBGImage6).color = GetRelicColorByRarity(Managers.Data.RelicDic[relicDetail.relicNo * 10 + relicDetail.level].Rarity);
                        break;
                    
                    default:
                        break;
                }
            }
            
            // 스킬 관련 갱신
            foreach (SkillDetail skillDetail in _detailRankingDataRes.data.skillList)
            {
                switch (skillDetail.slot)
                {
                    case 1:
                        GetText((int)ETexts.SkillLevelText1).text = $"Lv.{skillDetail.level}";
                        GetImage((int)EImages.Image1).sprite = Managers.Resource.Load<Sprite>(Managers.Data.SkillDic[skillDetail.skillNo].IconName);
                        break;
                    case 2:
                        GetText((int)ETexts.SkillLevelText2).text = $"Lv.{skillDetail.level}";
                        GetImage((int)EImages.Image2).sprite = Managers.Resource.Load<Sprite>(Managers.Data.SkillDic[skillDetail.skillNo].IconName);
                        break;
                    case 3:
                        GetText((int)ETexts.SkillLevelText3).text = $"Lv.{skillDetail.level}";
                        GetImage((int)EImages.Image3).sprite = Managers.Resource.Load<Sprite>(Managers.Data.SkillDic[skillDetail.skillNo].IconName);
                        break;
                    case 4:
                        GetText((int)ETexts.SkillLevelText4).text = $"Lv.{skillDetail.level}";
                        GetImage((int)EImages.Image4).sprite = Managers.Resource.Load<Sprite>(Managers.Data.SkillDic[skillDetail.skillNo].IconName);
                        break;
                    case 5:
                        GetText((int)ETexts.SkillLevelText5).text = $"Lv.{skillDetail.level}";
                        GetImage((int)EImages.Image5).sprite = Managers.Resource.Load<Sprite>(Managers.Data.SkillDic[skillDetail.skillNo].IconName);
                        break;
                    case 6:
                        GetText((int)ETexts.SkillLevelText6).text = $"Lv.{skillDetail.level}";
                        GetImage((int)EImages.Image6).sprite = Managers.Resource.Load<Sprite>(Managers.Data.SkillDic[skillDetail.skillNo].IconName);
                        break;
                    
                    default:
                        break;
                }
            }
            
        }));
    }

    Color GetRelicColorByRarity(int rarity)
    {
        switch (rarity)
        {
            case 0: return RelicUIColors.GradeC;
            case 1: return RelicUIColors.GradeB;
            case 2: return RelicUIColors.GradeA;
            case 3: return RelicUIColors.GradeS;
            case 4: return RelicUIColors.GradeSSS;
            default: return Color.white; // 기본 색상 반환
        }
    }
    
    private bool _isRequestingDetail = false;  // 현재 상세 랭킹 정보를 요청 중인지 확인하는 플래그

    // 무한 증폭의 원인이었다!!!!
    IEnumerator currentDetailRequest;
    IEnumerator GetDetailRankingInfo(int recordId, Action onCompleted)
    {
        if (_isRequestingDetail) {
            Debug.LogWarning("Detail ranking request is already in progress.");
            // 기존 요청 중이면 중단
            if (currentDetailRequest != null)
            {
                StopCoroutine(currentDetailRequest);
            }
        }

        _isRequestingDetail = true;  // 요청 시작 플래그 설정

        bool isDone = false;
        currentDetailRequest = JWTGetRequest($"rankings/{recordId}", res =>
        {
            _detailRankingDataRes = JsonUtility.FromJson<DetailRankingDataRes>(res);
            isDone = true;
        });
        StartCoroutine(currentDetailRequest);

        // 요청이 완료될 때까지 대기
        yield return new WaitUntil(() => isDone);

        // 완료 콜백 호출
        onCompleted?.Invoke();

        _isRequestingDetail = false;  // 요청 완료 플래그 해제
    }
    
    // 갱신
    void Refresh()
    {
        
    }

    void OnClickCloseButton()
    {
        Debug.Log("CloseRankingDetail");
        // 현재 팝업을 비활성화
        gameObject.SetActive(false);
    }
}
