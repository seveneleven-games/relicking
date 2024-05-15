using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LoginPopup : UI_Popup
{

    #region UI 기능 리스트

    // 정보 갱신

    #endregion
    
    #region Enum

    enum EGameObjects
    {
        
    }
    
    enum EButtons
    {
        LoginButton,
        SignupButton
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
    
    // 객체 관련 두는 곳
    
    

    private void Awake()
    {
        Init();
    }

    // 초기 세팅
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Debug.Log("UI_LoginPopup");
        
        #region Object Bind
        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));
        BindToggle(typeof(EToggles));
        BindImage(typeof(EImages)); 
        
        // 버튼 기능
        GetButton((int)EButtons.LoginButton).gameObject.BindEvent(OnClickLoginButton);
        GetButton((int)EButtons.SignupButton).gameObject.BindEvent(OnClickSignupButton);
        

        #endregion

        Refresh();
        return true;
    }

    // 갱신
    void Refresh()
    {
        
    }

    void OnClickLoginButton()
    {
        Managers.Sound.PlayButtonClick();
        // 현재 팝업 닫고 로그인 입력 팝업 열기
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_LoginInputPopup>();
    }

    void OnClickSignupButton()
    {
        Managers.Sound.PlayButtonClick();
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_SignupInputPopup>();
    }
    
    
}

