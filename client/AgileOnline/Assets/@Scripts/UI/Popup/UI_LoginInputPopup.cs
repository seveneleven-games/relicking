using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using static Util;
public class UI_LoginInputPopup : UI_Popup
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
        KakaoLoginButton,
        LoginButton,
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
    }
    
    #endregion
    
    // 객체 관련 두는 곳
    
    // Test를 위한 사용자 변수
    private string email = "user1@ssafy.com";
    private string password = "1234";
    

    private void Awake()
    {
        Init();
    }

    // 초기 세팅
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Debug.Log("UI_LoginInputPopup");
        
        #region Object Bind
        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));
        BindToggle(typeof(EToggles));
        BindImage(typeof(EImages)); 
        BindInputField(typeof(EInputFields));
        
        
        // Kakao 로그인 버튼
        GetButton((int)EButtons.KakaoLoginButton).gameObject.BindEvent(OnClickKakaoLoginButton);
        
        // 로그인하기 버튼
        GetButton((int)EButtons.LoginButton).gameObject.BindEvent(OnClickLoginButton);

        #endregion

        Refresh();
        
        return true;
    }

    // 갱신
    void Refresh()
    {
        if (_init == false)
            return;

        #region 초기화

        // Field 입력내용들 다 날리기
        GetInputField((int)EInputFields.EmailInputField).text = "";
        GetInputField((int)EInputFields.PasswordInputField).text = "";

        #endregion
    }

    void OnClickKakaoLoginButton()
    {
        Debug.Log("OnClickKakaoLoginButton");
        
        // 웹통신 테스트 -> 성공
        StartCoroutine(GetRequest("test/login"));

    }

    void OnClickLoginButton()
    {
        // CT에서 로그인 되는지 Test하기 -> 성공!!!
        // if (GetInputField((int)EInputFields.EmailInputField).text == email && GetInputField((int)EInputFields.PasswordInputField).text == password)
        // {
        //     Debug.Log("로그인 성공");
        // }
        // else
        // {
        //     Debug.Log("로그인 실패");
        // }
        
        // 백으로 요청 보내기
        
        // 성공시 로비로 가기
        Managers.Scene.LoadScene(EScene.LobbyScene);
    }
    


    
}
