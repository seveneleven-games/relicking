using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;

[Serializable]
public class NicknameDataReq
{
    public string nickname;
    
}

[Serializable]
public class NicknameDataRes
{
    public int status;
    public string message;
    public bool data;
}

public class UI_NicknamePopup : UI_Popup
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
        ConfirmButton,
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
    
    enum EInputFields
    {
        NicknameInputField,
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

        Debug.Log("UI_NicknamePopup");
        
        #region Object Bind
        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));
        BindToggle(typeof(EToggles));
        BindImage(typeof(EImages)); 
        BindInputField(typeof(EInputFields));
        
        // 확인 버튼
        GetButton((int)EButtons.ConfirmButton).gameObject.BindEvent(OnClickConfirmButton);
        
        #endregion

        Refresh();
        
        return true;
    }

    // 갱신
    void Refresh()
    {
        // Field 입력내용들 다 날리기
        GetInputField((int)EInputFields.NicknameInputField).text = "";
    }

    void OnClickConfirmButton()
    {
        
        // 닉네임 객체 만들기
        NicknameDataReq nicknameDataReq = new NicknameDataReq
        {
            nickname = GetInputField((int)EInputFields.NicknameInputField).text,
        };
        
        // 객체 -> Json 변환
        string nicknameJsonData = JsonUtility.ToJson(nicknameDataReq);
        
        // 백으로 요청 보내기
        
        
        
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_LoginInputPopup>();
    }
}
