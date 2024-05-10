using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using Object = UnityEngine.Object;

public class UI_TitleScene : UI_Scene
{
    #region Enum

    enum EGameObjects
    {
        LoadingSlide
    }
    
    enum EButtons
    {
        StartButton
    }
    
    
    enum ETexts
    {
        StartText
    }

    #endregion

    bool isPreload = false;
    
    // 로그인 관련 팝업 작성
    UI_LoginPopup _loginPopupUI;
    private GameObject _loadingSlide;
    private float _loadingValue = 0f;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));

        _loadingSlide = GetObject((int)EGameObjects.LoadingSlide);
        
        // 아무곳이나 누르면 씬 변환하는 버튼 생성
        GetButton((int)EButtons.StartButton).gameObject.BindEvent(() =>
        {
            if (isPreload)
            {

                Managers.UI.ShowPopupUI<UI_LoginPopup>();
                
                // 팝업이 열릴 때 시작 관련 버튼과 텍스트를 비활성화
                GetButton((int)EButtons.StartButton).gameObject.SetActive(false);
                GetText((int)ETexts.StartText).text = "";
                
                // Debug.Log("ChangeScene");
                // Managers.Scene.LoadScene(EScene.LobbyScene);
            }
        });
        GetButton((int)EButtons.StartButton).gameObject.SetActive(false);
        GetText((int)ETexts.StartText).text = $"";
        
        

        StartLoadAssets();
        
        return true;
    }

    // 얜 뭐하는 애니?
    private void Awake()
    {
        Init();
    }

    void StartLoadAssets()
    {
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
        {
            //로딩바 적용

            _loadingValue = _loadingSlide.GetComponent<Slider>().value = (1000 * count / totalCount) / 10f;
            Util.FindChild<TMP_Text>(_loadingSlide, "Text").text = $"Loading...{_loadingValue}%";
            
            Debug.Log($"{key} {count}/{totalCount}");
            
            Debug.Log("잘 되고 있음!!");
            
            if (count == totalCount)
            {
                Debug.Log("카운트랑 토탈 카운트가 같음!!");
                
                isPreload = true;
                Managers.Data.Init();
                Managers.Game.Init();
                
                _loadingSlide.SetActive(false);
                GetButton((int)EButtons.StartButton).gameObject.SetActive(true);
                GetText((int)ETexts.StartText).text = "터치하여 시작하기";
            }
        });
    }
}
