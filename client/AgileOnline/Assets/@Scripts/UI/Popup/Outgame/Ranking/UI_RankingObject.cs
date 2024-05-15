using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RankingObject : UI_Base
{
    #region Enum

    enum EGameObjects
    {
        
    }
    
    enum EButtons
    {
        RankingDetailButton
    }
    
    enum ETexts
    {
        Rank,
        NickName,
        Difficulty
    }
    
    enum EToggles
    {
        
    }
    
    enum EImages
    {
        
    }
    
    #endregion
    
    // 객체 관련
    
    // Show를 하면 새로 생성을 하는 것이어서 계속 증폭하는 현상이 발생해서 이렇게 키고 끄는 형식으로 변경!!!
    private UI_RankingDetailPopup _uiRankingDetailPopup; 
    
    private void Awake()
    {
        Init();
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
        BindToggle(typeof(EToggles));
        BindImage(typeof(EImages)); 
        
        
        // 계속 생성되는 것을 방지하기 위해
        _uiRankingDetailPopup = Managers.UI.ShowPopupUI<UI_RankingDetailPopup>();
        _uiRankingDetailPopup.gameObject.SetActive(false);
        
        #endregion
        
        
        return true;
    }

    // UI_RankingPopup에서 호출할 것임.
    public void SetInfo(RankingInfo rankingInfo, int rank)
    {
        GetText((int)ETexts.Rank).text = $"{rank}";
        GetText((int)ETexts.NickName).text = rankingInfo.nickname;
        GetText((int)ETexts.Difficulty).text = $"{rankingInfo.difficulty}";
        GetButton((int)EButtons.RankingDetailButton).onClick.AddListener(()=> OnClickRankingDetailButton(rankingInfo, rank));
    }

    void OnClickRankingDetailButton(RankingInfo rankingInfo, int rank)
    {
        Managers.Sound.PlayButtonClick();
        _uiRankingDetailPopup.gameObject.SetActive(true);
        _uiRankingDetailPopup.SetRankingInfo(rankingInfo, rank);
    }
    
}
