using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageInfoItem : UI_Base
{
    #region UI 기능 리스트

    // 정보 갱신
    // StageValueText : 스테이지 이름
    // StageImage : 스테이지 이미지

    #endregion

    #region Enum

    enum EGameObjects
    {
        
    }
    
    enum EButtons
    {
        
    }
    
    enum ETexts
    {
        StageValueText,
    }
    
    enum EToggles
    {
        
    }
    
    enum EImages
    {
        StageImage,
        StageLockImage,
    }
    
    #endregion
    
    // 객체 관련 두는 곳
    StageData _stageData;

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
        
        ClearRewardCompleteInit();
        #endregion
        
        return true;
    }

    public void SetInfo(StageData data)
    {
        _stageData = data;
        // transform.localScale = Vector3.one;
        Refresh();
    }

    void Refresh()
    {
        GetText((int)ETexts.StageValueText).text = $"스테이지{_stageData.StageId}: {_stageData.Name}";
        GetImage((int)EImages.StageImage).sprite = Managers.Resource.Load<Sprite>(_stageData.ThumbnailName);
        if (Managers.Game.DicStageClearInfo.TryGetValue(_stageData.StageId, out StageClearInfo info) == false)
            return;
        
        // 최대 클리어 스테이지
        if (info.MaxDifficulty > 0)
        {
            GetImage((int)EImages.StageLockImage).gameObject.SetActive(false);
            GetImage((int)EImages.StageImage).color = Color.white;
        }
        else
        {
            //게임 처음 시작하고 스테이지창을 오픈 한 경우
            if (info.StageId == 1 && info.MaxDifficulty == 0)
            {
                GetImage((int)EImages.StageLockImage).gameObject.SetActive(false);
                GetImage((int)EImages.StageImage).color = Color.white;
            }
            // 새로운 스테이지
            if (Managers.Game.DicStageClearInfo.TryGetValue(_stageData.StageId - 1, out StageClearInfo PrevInfo) == false)
                return;
            if (PrevInfo.isClear == true)
            {
                GetImage((int)EImages.StageLockImage).gameObject.SetActive(false);
                GetImage((int)EImages.StageImage).color = Color.white;
            }
            
        }
        
    }
    
    void ClearRewardCompleteInit()
    {
        GetImage((int)EImages.StageLockImage).gameObject.SetActive(true);
        GetImage((int)EImages.StageImage).color = Util.HexToColor("6D6D6D");
    }
}
