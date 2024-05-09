using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_StageSelectPopup : UI_Popup
{
    #region Enum
    enum EGameObjects
    {
        
    }
    
    enum EButtons
    {
        StageButton1,
        StageButton2,
        StageButton3,
    }
    
    enum ETexts
    {
        
    }
    
    enum EToggles
    {
        
    }
    
    enum EImages
    {
        
    }
    
    #endregion

    // 객체 관련
    
    public static event Action<int> OnStageSelected;
    
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
        
        GetButton((int)EButtons.StageButton1).gameObject.BindEvent(OnClickStageButton1);
        GetButton((int)EButtons.StageButton2).gameObject.BindEvent(OnClickStageButton2);
        GetButton((int)EButtons.StageButton3).gameObject.BindEvent(OnClickStageButton3);
        
        
        #endregion

        
        return true;
    }
    
    
    void OnClickStageButton1()
    {
        Debug.Log("1번 선택");
        OnStageSelected?.Invoke(1);
        Managers.UI.ClosePopupUI(this);
    }
        
    void OnClickStageButton2()
    {
        Debug.Log("2번 선택");
        OnStageSelected?.Invoke(2);
        Managers.UI.ClosePopupUI(this);
    }
        
    void OnClickStageButton3()
    {
        Debug.Log("3번 선택");
        OnStageSelected?.Invoke(3);
        Managers.UI.ClosePopupUI(this);
    }
    
}
