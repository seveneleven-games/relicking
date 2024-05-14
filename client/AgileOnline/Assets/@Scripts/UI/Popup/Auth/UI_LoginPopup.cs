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
        // 내가 하면서 느낀건데 되도록이면 타이틀 씬에는 가져오는 것만 이용하는 것이 best인 듯
        Managers.Sound.Play(Define.ESound.Bgm,"Bgm_Login");
        return true;
    }

    // 갱신
    void Refresh()
    {
        
    }

    void OnClickLoginButton()
    {
        // 현재 팝업 닫고 로그인 입력 팝업 열기
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_LoginInputPopup>();
    }

    void OnClickSignupButton()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_SignupInputPopup>();
    }
    
    
}

