using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_IdleScene : UI_Scene
{
    #region Enum

    enum EGameObjects
    {
        GrowthBG
    }
    
    enum EButtons
    {
        
    }
    
    
    enum ETexts
    {
        
    }
    
    enum EImages
    {
        OriginImage,
    }


    #endregion

    bool isPreload = false;
    
    // 방치 관련 팝업 작성
    UI_IdleProceedPopup _idleProceedPopupUI;
    

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        
        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));
        BindImage(typeof(EImages)); 
        
        
        Image originImage = GetImage((int)EImages.OriginImage); // 이미지 사용
        
        _idleProceedPopupUI = Managers.UI.ShowPopupUI<UI_IdleProceedPopup>();
        
        

        
        return true;
    }

    
    private void Awake()
    {
        Init();
    }
    
}