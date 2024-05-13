using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Util;

[Serializable]
public class ChangePasswordDataReq
{
    public string oldPassword;
    public string newPassword;
    public string newPasswordRe;
}

[Serializable]
public class ChangePasswordDataRes
{
    public int status;
    public string message;
    public bool data;
}


public class UI_ChangePopup : UI_Popup
{
    #region UI 기능 리스트

    // 정보 갱신

    #endregion
    
    #region Enum

    enum EGameObjects
    {
        ChangeNicknameInput,
        OldPasswordInput,
        NewPasswordInput,
        CheckNewPasswordInput,
    }
    
    enum EButtons
    {
        ConfirmButton,
        BackButton,
    }
    
    enum ETexts
    {
        ChangeNicknameGuideText,
        ChangeNicknameResultText,
        ChangeNicknameText,
        
        NewPasswordGuideText,
        OldPasswordResultText,
        NewPasswordResultText,
        CheckNewPasswordResultText,
        OldPasswordText,
        NewPasswordText,
        CheckNewPasswordText,
        
    }
    
    enum EToggles
    {
        
    }
    
    enum EImages
    {
        
    }
    
    enum EInputFields
    {
        ChangeNicknameInputField,
        OldPasswordInputField,
        NewPasswordInputField,
        CheckNewPasswordInputField,
    }
    
    #endregion
    
    
    
    // 객체 관련 두는 곳
    private bool isDuplicateNickname = false;
    private GameObject _logoImage;
    private bool _isChangeNickname = false;
    private bool _isValidatePassword = false;
    
    
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


        // 무조건 시작은 false
        
        // 이메일 중복 체크 결과 텍스트
        GetText((int)ETexts.ChangeNicknameResultText).gameObject.SetActive(false);
        
        // 비밀번호 유효성 결과 텍스트
        GetText((int)ETexts.OldPasswordResultText).gameObject.SetActive(false);
        GetText((int)ETexts.NewPasswordResultText).gameObject.SetActive(false);
        GetText((int)ETexts.CheckNewPasswordResultText).gameObject.SetActive(false);
        
        // 확인 버튼
        GetButton((int)EButtons.ConfirmButton).gameObject.BindEvent(OnClickConfirmButton);
        
        // 뒤로 가기 버튼
        GetButton((int)EButtons.BackButton).gameObject.BindEvent(OnClickBackButton);
        
        
        
        // 신규 비밀번호 및 비밀번호 확인 필드의 값 변화에 대한 이벤트 넣기
        GetInputField((int)EInputFields.OldPasswordInputField).onValueChanged.AddListener(OnChangeOldPassword);
        GetInputField((int)EInputFields.NewPasswordInputField).onValueChanged.AddListener(OnChangeNewPassword);
        GetInputField((int)EInputFields.CheckNewPasswordInputField).onValueChanged
            .AddListener(OnChangeCheckNewPassword);
        
        
        #endregion

        Refresh();
        
        return true;
    }

    // 갱신
    void Refresh()
    {
        // Field 입력내용들 다 날리기
        GetInputField((int)EInputFields.ChangeNicknameInputField).text = "";
        GetInputField((int)EInputFields.OldPasswordInputField).text = "";
        GetInputField((int)EInputFields.NewPasswordInputField).text = "";
        GetInputField((int)EInputFields.CheckNewPasswordInputField).text = "";
    }

    public void SetInfo(bool isChangeNickname)
    {
        _isChangeNickname = isChangeNickname;
        
        #region 닉네임 or 비밀번호

        // 닉네임과 비밀번호 관련 여부에 따른 UI 변경
        
        if (_isChangeNickname)
        {
            // 닉네임 관련 다 킴
            GetText((int)ETexts.ChangeNicknameGuideText).gameObject.SetActive(true);
            
            GetText((int)ETexts.ChangeNicknameText).gameObject.SetActive(true);
            GetObject((int)EGameObjects.ChangeNicknameInput).gameObject.SetActive(true);
            
            // 비밀번호 관련 다 끔
            GetText((int)ETexts.NewPasswordGuideText).gameObject.SetActive(false);
            
            GetText((int)ETexts.OldPasswordText).gameObject.SetActive(false);
            GetObject((int)EGameObjects.OldPasswordInput).gameObject.SetActive(false);
            
            GetText((int)ETexts.NewPasswordText).gameObject.SetActive(false);
            GetObject((int)EGameObjects.NewPasswordInput).gameObject.SetActive(false);
            
            
            GetText((int)ETexts.CheckNewPasswordText).gameObject.SetActive(false);
            GetObject((int)EGameObjects.CheckNewPasswordInput).gameObject.SetActive(false);
            
        }
        else
        {
            //비밀번호 관련 다 킴
            GetText((int)ETexts.NewPasswordGuideText).gameObject.SetActive(true);
            
            GetText((int)ETexts.OldPasswordText).gameObject.SetActive(true);
            GetObject((int)EGameObjects.OldPasswordInput).gameObject.SetActive(true);

            GetText((int)ETexts.NewPasswordText).gameObject.SetActive(true);
            GetObject((int)EGameObjects.NewPasswordInput).gameObject.SetActive(true);

            
            GetText((int)ETexts.CheckNewPasswordText).gameObject.SetActive(true);
            GetObject((int)EGameObjects.CheckNewPasswordInput).gameObject.SetActive(true);

            
            // 닉네임 관련 다 끔
            GetText((int)ETexts.ChangeNicknameGuideText).gameObject.SetActive(false);
            
            GetText((int)ETexts.ChangeNicknameText).gameObject.SetActive(false);
            GetObject((int)EGameObjects.ChangeNicknameInput).gameObject.SetActive(false);


        }
        

        #endregion
        
        
    }
    
    
    void OnClickConfirmButton()
    {
        // 닉네임 변경 관련
        if (_isChangeNickname)
        {
            CheckNickname(() => {
                // 닉네임 중복 검사 후 통과하면 닉네임 변경 요청
                if (!isDuplicateNickname)
                {
                    ChangeNickname();
                }
            });
        }

        // 비밀번호 변경 관련
        else
        {
            ChangePassword();
        }
        
    }

    void OnClickBackButton()
    {
        Managers.UI.ClosePopupUI(this);
    }
    

    #region 닉네임 변경 관련

    void CheckNickname(Action onCompleted)
    {
        StartCoroutine(GetRequest(
            $"members/duplicate-nickname?nickname={GetInputField((int)EInputFields.ChangeNicknameInputField).text}",
            res =>
            {
                NicknameDataRes checkNickname = JsonUtility.FromJson<NicknameDataRes>(res);
                isDuplicateNickname = !checkNickname.data;
                if (isDuplicateNickname)
                {
                    Debug.Log("사용 불가능한 닉네임");
                    GetText((int)ETexts.ChangeNicknameResultText).gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("사용 가능한 닉네임");
                    GetText((int)ETexts.ChangeNicknameResultText).gameObject.SetActive(false);
                }
                onCompleted?.Invoke(); // 콜백 함수 호출
            }));
    }

    void ChangeNickname()
    {
        // 바꿀 닉네임 객체 만들기
        NicknameDataReq nicknameDataReq = new NicknameDataReq
        {
            nickname = GetInputField((int)EInputFields.ChangeNicknameInputField).text
        };
        
        // 객체 -> Json 변환
        string nicknameJsonData = JsonUtility.ToJson(nicknameDataReq);
        
        // 백으로 변경 요청 보내기
        StartCoroutine(JWTPatchRequest("members/nickname", nicknameJsonData, res =>
        {
            NicknameDataRes nicknameDataRes = JsonUtility.FromJson<NicknameDataRes>(res);

            if (nicknameDataRes != null && nicknameDataRes.status == 200)
            {
                Managers.UI.ClosePopupUI(this);
            }
        }));

    }

    #endregion


    #region 비밀번호 변경 관련

    void ChangePassword()
    {
        // 바꿀 비밀번호 객체 만들기
        ChangePasswordDataReq changePasswordDataReq = new ChangePasswordDataReq
        {
            oldPassword = GetInputField((int)EInputFields.OldPasswordInputField).text,
            newPassword = GetInputField((int)EInputFields.NewPasswordInputField).text,
            newPasswordRe = GetInputField((int)EInputFields.CheckNewPasswordInputField).text,
        };
        
        // 객체 -> Json 변환
        string changePasswordJsonData = JsonUtility.ToJson(changePasswordDataReq);
        
        // 백으로 요청 보내기
        StartCoroutine(JWTPatchRequest("members/password", changePasswordJsonData, res =>
        {
            ChangePasswordDataRes changePasswordDataRes = JsonUtility.FromJson<ChangePasswordDataRes>(res);
            
            if (changePasswordDataRes.status == 200)
            {
                Managers.UI.ClosePopupUI(this);
            }
        }));
    }
    

    #endregion

    void OnChangeOldPassword(string value)
    {
        GetText((int)ETexts.OldPasswordResultText).gameObject.SetActive(false);
    }
    
    void OnChangeNewPassword(string value)
    {
        
        _isValidatePassword = ValidatePassword(value);

        // 비밀번호 유효하다면
        if (_isValidatePassword)
        {
            GetText((int)ETexts.NewPasswordResultText).gameObject.SetActive(false);
        }
        
        else
        {
            GetText((int)ETexts.NewPasswordResultText).gameObject.SetActive(true);
        }
        
        RefreshPasswordMatch();
    }

    void OnChangeCheckNewPassword(string value)
    {
        RefreshPasswordMatch();
    }

    void RefreshPasswordMatch()
    {
        bool passwordsMatch = ValidatePasswordsMatch(
            GetInputField((int)EInputFields.NewPasswordInputField).text,
            GetInputField((int)EInputFields.CheckNewPasswordInputField).text
        );

        // 일치 안하면 경고메시지 활성화
        GetText((int)ETexts.CheckNewPasswordResultText).gameObject.SetActive(!passwordsMatch);
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
    
    
    // 비밀번호 일치 검사
    bool ValidatePasswordsMatch(string password, string confirmPassword)
    {
        return password == confirmPassword;
    }
    
}
