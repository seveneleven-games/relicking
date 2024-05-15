using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Util;

[Serializable]
public class LogoutDataRes
{
    public int status;
    public string message;
    public bool data;
}


public class UI_OutgameSettingPopup : UI_Popup
{
    enum Texts
    {
        BGMONText,
        SFXONText,
        BGMOFFText,
        SFXOFFText,
    }

    enum Buttons
    {
        CloseButton,
        LogoutButton,
        ChangeNicknameButton,
        ChangePasswordButton,
        
    }

    enum Toggles
    {
        BGMSoundToggle,
        SFXSoundToggle,
    }

    enum Images
    {
        BGMCheckmark,
        SFXCheckmark,
    }
    
    // 객체 관련 두는 곳
    private bool _isSelectedBGMSound;
    private bool _isSelectedSFXSound;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindToggle(typeof(Toggles));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(ClosePopupUI);

        _isSelectedBGMSound = Managers.Game.BGMOn;
        _isSelectedSFXSound = Managers.Game.EffectSoundOn;
        
        // 토글 텍스트 초기 설정
        TogglesInit();
        
        // 토글 버튼
        GetToggle((int)Toggles.BGMSoundToggle).gameObject.BindEvent(OnClickBGMSoundToggle);
        GetToggle((int)Toggles.SFXSoundToggle).gameObject.BindEvent(OnClickSFXSoundToggle);
        
        // 로그아웃 버튼
        GetButton((int)Buttons.LogoutButton).gameObject.BindEvent(OnClickLogoutButton);
        
        // 닉네임 변경 버튼
        GetButton((int)Buttons.ChangeNicknameButton).gameObject.BindEvent(OnClickChangeNicknameButton);
        
        // 비밀번호 변경 버튼
        GetButton((int)Buttons.ChangePasswordButton).gameObject.BindEvent(OnClickChangePasswordButton);
        
        
        return true;
    }

    #region 토글 초기화

    void TogglesInit()
    {
        // BGM 토글 설정
        Toggle bgmToggle = GetToggle((int)Toggles.BGMSoundToggle);
        bgmToggle.isOn = _isSelectedBGMSound;
        GetText((int)Texts.BGMOFFText).gameObject.SetActive(!_isSelectedBGMSound);
        GetText((int)Texts.BGMONText).gameObject.SetActive(_isSelectedBGMSound);

        // SFX 토글 설정
        Toggle sfxToggle = GetToggle((int)Toggles.SFXSoundToggle);
        sfxToggle.isOn = _isSelectedSFXSound;
        GetText((int)Texts.SFXOFFText).gameObject.SetActive(!_isSelectedSFXSound);
        GetText((int)Texts.SFXONText).gameObject.SetActive(_isSelectedSFXSound);
    }
    
    #endregion
    
    
    
    void OnClickBGMSoundToggle()
    {
        Managers.Sound.PlayButtonClick();
        _isSelectedBGMSound = !_isSelectedBGMSound;

        if (_isSelectedBGMSound)
        {
            Managers.Game.BGMOn = true;
            GetText((int)Texts.BGMOFFText).gameObject.SetActive(false);
            GetText((int)Texts.BGMONText).gameObject.SetActive(true);
            
        }

        else
        {
            Managers.Game.BGMOn = false;
            GetText((int)Texts.BGMOFFText).gameObject.SetActive(true);
            GetText((int)Texts.BGMONText).gameObject.SetActive(false);
        }
    }

    void OnClickSFXSoundToggle()
    {
        Managers.Sound.PlayButtonClick();
        _isSelectedSFXSound = !_isSelectedSFXSound;

        if (_isSelectedSFXSound)
        {
            Managers.Game.EffectSoundOn = true;
            GetText((int)Texts.SFXOFFText).gameObject.SetActive(false);
            GetText((int)Texts.SFXONText).gameObject.SetActive(true);
        }

        else
        {
            Managers.Game.EffectSoundOn = false;
            GetText((int)Texts.SFXOFFText).gameObject.SetActive(true);
            GetText((int)Texts.SFXONText).gameObject.SetActive(false);
        }
    }
    
    
    void OnClickLogoutButton()
    {
        Managers.Sound.PlayButtonClick();
        StartCoroutine(JWTDeleteRequest("members/logout", res =>
        {
            // json -> 객체로 변환
            LogoutDataRes logoutDataRes = JsonUtility.FromJson<LogoutDataRes>(res);

            // 로그아웃 요청 성공시에 맨 처음으로 보내고 토큰 지우기
            if (logoutDataRes.data == true)
            {
                SceneManager.LoadScene("NewTitleScene");
                Managers.Game.AccessToken = "";
                Managers.Game.RefreshToken = "";
            }
        }));
    }
    
    void OnClickChangeNicknameButton()
    {
        Managers.Sound.PlayButtonClick();
        UI_ChangePopup popup = Managers.UI.ShowPopupUI<UI_ChangePopup>();
        popup.SetInfo(true);
    }
    
    void OnClickChangePasswordButton()
    {
        Managers.Sound.PlayButtonClick();
        UI_ChangePopup popup = Managers.UI.ShowPopupUI<UI_ChangePopup>();
        popup.SetInfo(false);
    }
}
