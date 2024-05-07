using System;
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

    // 이거 죽이면 에러 뜸.
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
        
        // 이건 잠겨있을 경우를 위한 것임.. (스테이지 잠금이 있을 시..)
        GetImage((int)EImages.StageLockImage).gameObject.SetActive(true);
        GetImage((int)EImages.StageImage).color = Util.HexToColor("6D6D6D");
        
        #endregion
        
        return true;
    }

    public void SetInfo(StageData data)
    {
        _stageData = data;
        transform.localScale = Vector3.one;
        Refresh();
    }

    void Refresh()
    {
        GetText((int)ETexts.StageValueText).text = $"스테이지{_stageData.StageId}: {_stageData.Name}";
        GetImage((int)EImages.StageImage).sprite = Managers.Resource.Load<Sprite>(_stageData.ThumbnailName);
    }
    
}
