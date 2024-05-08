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
        Class,
        Difficulty
    }
    
    enum EToggles
    {
        
    }
    
    enum EImages
    {
        
    }
    
    #endregion
    
    
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
        #endregion
        
        
        return true;
    }

    // UI_RankingPopup에서 호출할 것임.
    public void SetInfo(RankingInfo rankingInfo, int rank)
    {
        GetText((int)ETexts.Rank).text = $"{rank}";
        GetText((int)ETexts.NickName).text = rankingInfo.nickname;
        GetText((int)ETexts.Class).text = $"{rankingInfo.classNo}"; // 임시
        GetText((int)ETexts.Difficulty).text = $"{rankingInfo.difficulty}";
        
    }
    
    
}
