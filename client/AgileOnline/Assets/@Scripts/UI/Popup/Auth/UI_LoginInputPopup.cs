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
    public StageRes stageData;
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
    private GameObject _logoImage;
    
    private void Awake()
    {
        Init();
        _logoImage = GameObject.Find("UI_TitleScene/LogoImage");
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
        
        // InputField 이벤트 바인딩
        var emailInput = GetInputField((int)EInputFields.EmailInputField);
        var passwordInput = GetInputField((int)EInputFields.PasswordInputField);
        emailInput.onSelect.AddListener((_) => OnInputFieldSelected());
        passwordInput.onSelect.AddListener((_) => OnInputFieldSelected());
        emailInput.onDeselect.AddListener((_) => OnInputFieldDeselected());
        passwordInput.onDeselect.AddListener((_) => OnInputFieldDeselected());
        
        
        // 뒤로가기 버튼
        GetButton((int)EButtons.BackButton).gameObject.BindEvent(OnClickBackButton);
        
        // 로그인하기 버튼
        GetButton((int)EButtons.LoginButton).gameObject.BindEvent(OnClickLoginButton);
        GetButton((int)EButtons.LoginButton).gameObject.SetActive(true);
        
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
                Managers.Game.UpdateStageClearInfo(loginDataRes.data.stageData);
                _templateData.SelectedClassId = loginDataRes.data.currentClassNo;
                loginDataRes.data.relicList.ForEach(relic =>
                {
                    int index = relic.slot - 1;
                    if (index >= 0 && index < _templateData.EquipedRelicIds.Length)
                    {
                        _templateData.EquipedRelicIds[index] = relic.relicNo * 10 + relic.level;
                    }
                });

                foreach (int relicId in _templateData.EquipedRelicIds)
                {
                    Debug.Log("relic id is : " + relicId);
                }

                Managers.Scene.LoadScene(EScene.LobbyScene);
            }
            
        }));
    }

    // 입력 필드가 선택될 때 호출
    void OnInputFieldSelected()
    {
        _logoImage.SetActive(false);
    }

    // 입력 필드 선택이 해제될 때 호출
    void OnInputFieldDeselected()
    {
        _logoImage.SetActive(true);
    }
    


    
}
