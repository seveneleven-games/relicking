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
        if (originImage != null)
        {
            // 예를 들어 이미지를 설정하거나 다른 작업을 수행
            originImage.color = Color.red; // 색상 변경 예시
        }
        
        _idleProceedPopupUI = Managers.UI.ShowPopupUI<UI_IdleProceedPopup>();
        
        

        
        return true;
    }

    // 얜 뭐하는 애니?
    private void Awake()
    {
        Init();
    }
    
}