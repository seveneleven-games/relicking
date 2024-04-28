using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SignupInputPopup : UI_Popup
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
        CheckEmailButton,
        NextButton,
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
        EmailInputField,
        PasswordInputField,
        CheckPasswordInputField,
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

        Debug.Log("UI_SignupInputPopup");
        
        #region Object Bind
        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));
        BindToggle(typeof(EToggles));
        BindImage(typeof(EImages)); 
        BindInputField(typeof(EInputFields));
        
        // EmailCheck 버튼
        GetButton((int)EButtons.CheckEmailButton).gameObject.BindEvent(OnClickCheckEmailButton);
        
        // Next 버튼
        GetButton((int)EButtons.NextButton).gameObject.BindEvent(OnClickNextButton);
        
        
        #endregion

        Refresh();
        
        return true;
    }

    // 갱신
    void Refresh()
    {
        // Field 입력내용들 다 날리기
        GetInputField((int)EInputFields.EmailInputField).text = "";
        GetInputField((int)EInputFields.PasswordInputField).text = "";
        GetInputField((int)EInputFields.CheckPasswordInputField).text = "";
    }

    void OnClickCheckEmailButton()
    {
        Debug.Log("OnClickCheckEmailButton");
        // 백으로 보내고 중복이 없다면 버튼 색깔 바꾸던가 하기
    }
    
    void OnClickNextButton()
    {
        
        // isEmail 같은 것 만들어서 모두 충족했을 시에 아래가 실행되도록 하기
        
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_NicknamePopup>();
    }
}
