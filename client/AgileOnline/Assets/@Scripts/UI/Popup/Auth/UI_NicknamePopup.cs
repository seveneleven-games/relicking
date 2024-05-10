using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Util;

// 이거 나중에 닉네임만 변경할 시를 대비해서 남겨둠
// 닉네임 변경인 것 알기위해 isNick 같은거 넘겨주면서 확인.
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

[Serializable]
public class SignupDataReq
{
    public string email;
    public string password;
    public string nickname;
}

[Serializable]
public class SignupDataRes
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
        NicknameResultText,
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
    private string email;
    private string password;
    private bool isDuplicateNickname = false;
    
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
        
        // 중복 체크 결과 텍스트
        GetText((int)ETexts.NicknameResultText).gameObject.SetActive(false);
        
        
        
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

    public void SetInfo(string email, string password)
    {
        this.email = email;
        this.password = password;
    }

    void OnClickConfirmButton()
    {
        CheckNickname(() => {
            // 닉네임 중복 검사 후 회원가입 진행
            if (!isDuplicateNickname)
            {
                // 회원가입 로직
                Signup();
            }
        });
    }

    void CheckNickname(Action onCompleted)
    {
        StartCoroutine(GetRequest(
            $"members/duplicate-nickname?nickname={GetInputField((int)EInputFields.NicknameInputField).text}",
            res =>
            {
                NicknameDataRes checkNickname = JsonUtility.FromJson<NicknameDataRes>(res);
                isDuplicateNickname = !checkNickname.data;
                if (isDuplicateNickname)
                {
                    Debug.Log("사용 불가능한 닉네임");
                    GetText((int)ETexts.NicknameResultText).gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("사용 가능한 닉네임");
                    GetText((int)ETexts.NicknameResultText).gameObject.SetActive(false);
                }
                onCompleted?.Invoke(); // 콜백 함수 호출
            }));
    }

    void Signup()
    {
        // 회원가입 객체 만들기
        SignupDataReq signupData = new SignupDataReq
        {
            email = this.email,
            password = this.password,
            nickname = GetInputField((int)EInputFields.NicknameInputField).text
        };

        // 객체 -> Json으로 변환
        string signupJsonData = JsonUtility.ToJson(signupData);

        // 회원 가입 요청 보내기
        StartCoroutine(PostRequest("members/signup", signupJsonData, res =>
        {
            SignupDataRes signupDataRes = JsonUtility.FromJson<SignupDataRes>(res);
            if (signupDataRes.data)
            {
                Managers.UI.ClosePopupUI(this);
                Managers.UI.ShowPopupUI<UI_LoginInputPopup>();
            }
        }));
        
        
        
        // Managers.UI.ClosePopupUI(this);
        // Managers.UI.ShowPopupUI<UI_LoginInputPopup>();
    }
}
