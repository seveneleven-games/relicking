using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindToggle(typeof(Toggles));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(ClosePopupUI);
        
        GetText((int)Texts.BGMOFFText).gameObject.SetActive(false);
        GetText((int)Texts.SFXOFFText).gameObject.SetActive(false);

        // 로그아웃 버튼
        GetButton((int)Buttons.LogoutButton).gameObject.BindEvent(OnClickLogoutButton);
        
        // 닉네임 변경 버튼
        GetButton((int)Buttons.ChangeNicknameButton).gameObject.BindEvent(OnClickChangeNicknameButton);
        
        // 비밀번호 변경 버튼
        GetButton((int)Buttons.ChangePasswordButton).gameObject.BindEvent(OnClickChangePasswordButton);
        
        return true;
    }

    void OnClickLogoutButton()
    {
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
        UI_ChangePopup popup = Managers.UI.ShowPopupUI<UI_ChangePopup>();
        popup.SetInfo(true);
    }
    
    void OnClickChangePasswordButton()
    {
        UI_ChangePopup popup = Managers.UI.ShowPopupUI<UI_ChangePopup>();
        popup.SetInfo(false);
    }
}
