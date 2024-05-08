using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using static Util;


[Serializable]
public class LoginDataReq
{
    public string email;
    public string password;
}

#region LoginDataRes

[Serializable]
public class LoginDataRes
{
    public int status;
    public string message;
    public UserRes data;
}

[Serializable]
public class UserRes
{
    public string accessToken;
    public string refreshToken;
    public int memberId;
    public string nickname;
    public StageRes stageRes;
    public int currentClassNo;
    public List<RelicList> relicList;
}

[Serializable]
public class StageRes
{
    public int stage1;
    public int stage2;
    public int stage3;
}

[Serializable]
public class RelicList
{
    public int slot;
    public int relicNo;
    public int level;
}


#endregion

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
        BackButton,
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

    private TemplateData _templateData;
    
    private void Awake()
    {
        Init();
    }

    // 초기 세팅
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        _templateData = Resources.Load<TemplateData>("GameTemplateData");

        Debug.Log("UI_LoginInputPopup");
        
        #region Object Bind
        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));
        BindToggle(typeof(EToggles));
        BindImage(typeof(EImages)); 
        BindInputField(typeof(EInputFields));
        
        // 뒤로가기 버튼
        GetButton((int)EButtons.BackButton).gameObject.BindEvent(OnClickBackButton);
        
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

    void OnClickBackButton()
    {
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_LoginPopup>();
    }
    
    void OnClickKakaoLoginButton()
    {
        Debug.Log("OnClickKakaoLoginButton");
        
        // 웹통신 테스트 -> 성공
        StartCoroutine(GetRequest("test/login", data =>
        {
            Debug.Log("test해봅시다!!! " + data);
        }));

    }

    void OnClickLoginButton()
    {
        
        // 로그인 객체 만들기
        LoginDataReq loginDataReq = new LoginDataReq
        {
            email = GetInputField((int)EInputFields.EmailInputField).text,
            password = GetInputField((int)EInputFields.PasswordInputField).text,
        };
        
        // 객체 -> Json 변환
        string loginJsonData = JsonUtility.ToJson(loginDataReq);
        
        // 백으로 요청 보내기
        StartCoroutine(PostRequest("members/login", loginJsonData, res =>
        {

            // json -> 객체로 변환
            LoginDataRes loginDataRes = JsonUtility.FromJson<LoginDataRes>(res);
            
            // 성공시 로비로 가기
            if (loginDataRes.data != null && loginDataRes.data.accessToken != null)
            {
                // 토큰들 저장하기
                Managers.Game._gameData.accessToken = loginDataRes.data.accessToken;
                Managers.Game._gameData.refreshToken = loginDataRes.data.refreshToken;
                
                // 스테이지별 난이도 정보 업데이트
                Managers.Game.UpdateStageClearInfo(loginDataRes.data.stageRes);
                _templateData.SelectedClassId = loginDataRes.data.currentClassNo;
                loginDataRes.data.relicList.ForEach(relic =>
                {
                    int index = relic.slot - 1;
                    if (index >= 0 && index < _templateData.EquipedRelicIds.Length)
                    {
                        _templateData.EquipedRelicIds[index] = relic.relicNo * 10 + relic.level - 10;
                    }
                });

                foreach (int relicId in _templateData.EquipedRelicIds)
                {
                    Debug.Log("relic id is : " + relicId);
                }

                Managers.Scene.LoadScene(EScene.LobbyScene);
            }
            
        }));
        
        // 임시 성공하든 안하든 로비로
        // Managers.Scene.LoadScene(EScene.LobbyScene);
        
    }
    


    
}
