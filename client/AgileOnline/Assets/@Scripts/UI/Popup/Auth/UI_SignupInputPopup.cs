using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;
using System.Text.RegularExpressions;

[Serializable]
public class CheckEmailRes
{
    public int status;
    public string message;
    public bool data;
}


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
        NoNextButton,
    }
    
    enum ETexts
    {
        EmailForm,
        PasswordForm,
        CheckPasswordForm,
    }
    
    enum EToggles
    {
        
    }
    
    enum EImages
    {
        CheckImage,
    }

    enum EInputFields
    {
        EmailInputField,
        PasswordInputField,
        CheckPasswordInputField,
    }
    
    #endregion
    
    // 객체 관련 두는 곳
    public string email = "";

    public string password = "";
    
    bool isValidatePassword = false;

    bool isValidateEmail = false;
    
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
        GetButton((int)EButtons.NextButton).gameObject.SetActive(false);
        GetButton((int)EButtons.NextButton).gameObject.BindEvent(OnClickNextButton);
        
        // NoNext 버튼
        GetButton((int)EButtons.NoNextButton).gameObject.SetActive(true);
        
        // 이메일 필드 값 변화에 대한 이벤트 넣기
        GetInputField((int)EInputFields.EmailInputField).onValueChanged.AddListener(OnChangeEmail);
        
        // 비밀번호 및 비밀번호 확인 필드의 값 변화에 대한 이벤트 넣기
        GetInputField((int)EInputFields.PasswordInputField).onValueChanged.AddListener(OnChangePassword);
        GetInputField((int)EInputFields.CheckPasswordInputField).onValueChanged.AddListener(OnChangeCheckPassword);
        
        
        // 형식 경고 관련 텍스트
        GetText((int)ETexts.EmailForm).gameObject.SetActive(false);
        GetText((int)ETexts.PasswordForm).gameObject.SetActive(false);
        GetText((int)ETexts.CheckPasswordForm).gameObject.SetActive(false);
        
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

        if (!ValidateEmail(GetInputField((int)EInputFields.EmailInputField).text))
        {
            // 유효하지 않다면
            return;
        }
        
        // 이메일 중복 확인 요청 보내기
        StartCoroutine(GetRequest(
            $"members/duplicate-email?email={GetInputField((int)EInputFields.EmailInputField).text}",
            res =>
            {
                // json -> 객체로 변환
                CheckEmailRes checkEmail = JsonUtility.FromJson<CheckEmailRes>(res);
                
                // true면 (사용가능한 이메일)
                if (checkEmail.data)
                {
                    GetImage((int)EImages.CheckImage).color = Util.HexToColor("27AE24"); // 체크 버튼 색깔 변경.
                    isValidateEmail = true;
                    CheckMatch();
                }
                // false면 (중복된 이메일)
                else
                    GetImage((int)EImages.CheckImage).color = Util.HexToColor("525252"); // 체크 버튼 색깔 변경.
                    
            }));
    }
    
    // 이메일 유효성 검사 정규표현식
    private const string emailPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
    
    void OnChangeEmail(string value)
    {
        // 이메일이 유효하지 않다면
        if (!ValidateEmail(value))
        {
            isValidateEmail = false;
            GetImage((int)EImages.CheckImage).color = Util.HexToColor("525252"); // 체크 버튼 색깔 변경.
            GetText((int)ETexts.EmailForm).gameObject.SetActive(true);
        }
        
        else
        {
            GetText((int)ETexts.EmailForm).gameObject.SetActive(false);
        }
        
        CheckMatch();
    }
    
    // 이메일 유효성 검사
    bool ValidateEmail(string email)
    {
        // 이메일이 정규표현식에 부합하는지 확인
        return Regex.IsMatch(email, emailPattern);
    }
    
    
    void OnChangePassword(string value)
    {
        isValidatePassword = ValidatePassword(value);

        // 비밀번호 유효하다면
        if (isValidatePassword)
        {
            GetText((int)ETexts.PasswordForm).gameObject.SetActive(false);
        }
        
        else
        {
            GetText((int)ETexts.PasswordForm).gameObject.SetActive(true);
        }
        
        CheckMatch();
    }
    
    void OnChangeCheckPassword(string value)
    {
        CheckMatch();
    }

    
    
    // 비밀번호 유효성 검사
    bool ValidatePassword(string password)
    {
        if (password.Length < 8 || password.Length > 16)
        {
            return false; // 길이 체크
        }

        bool hasLetter = false; // 영문자 존재 여부
        bool hasDigit = false;  // 숫자 존재 여부
        bool hasSpecialChar = false; // 특수문자 존재 여부

        foreach (char c in password)
        {
            if (char.IsLetter(c)) hasLetter = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (!char.IsWhiteSpace(c) && !char.IsControl(c)) hasSpecialChar = true;
        
            // 모든 조건을 만족하면 더 이상 검사할 필요가 없음
            if (hasLetter && hasDigit && hasSpecialChar)
            {
                return true;
            }
        }

        // 세 조건 중 하나라도 충족하지 않으면 false 반환
        return hasLetter && hasDigit && hasSpecialChar;
    }

    
    void CheckMatch()
    {
        string password = GetInputField((int)EInputFields.PasswordInputField).text;
        string confirmPassword = GetInputField((int)EInputFields.CheckPasswordInputField).text;
        bool passwordsMatch = password == confirmPassword;

        // 비밀번호 일치 여부에 따른 UI 업데이트
        GetText((int)ETexts.CheckPasswordForm).gameObject.SetActive(!passwordsMatch);

        // 모든 조건이 충족되는지 확인
        bool canProceed = passwordsMatch && isValidatePassword && isValidateEmail;
        GetButton((int)EButtons.NextButton).gameObject.SetActive(canProceed);
        GetButton((int)EButtons.NoNextButton).gameObject.SetActive(!canProceed);
    }
    
    void OnClickNextButton()
    {
        email = GetInputField((int)EInputFields.EmailInputField).text;
        password = GetInputField((int)EInputFields.PasswordInputField).text;
        
        Managers.UI.ClosePopupUI(this);
        
        var nicknamePopup = Managers.UI.ShowPopupUI<UI_NicknamePopup>();
        nicknamePopup.SetInfo(email, password);  // 닉네임 팝업으로 데이터 전달
        
        
    }
}

