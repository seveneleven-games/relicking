using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;



public class UI_GachaResultPopup : UI_Popup
{
    
    // 사용할 프리팹 가져오기
    [SerializeField]
    GameObject UI_GachaRelicObject;
    
    #region Enum
    enum EGameObjects
    {
        OpenContentObject, // 결과 이전 창
        ContentObject, // 결과창
        GachaResultListObject, // 위치 필요
    }

    enum EButtons
    {
        SkipButton,
        CloseButton,
    }

    enum ETexts
    {

    }

    #endregion

    // 객체 관련 쓰는 곳
    private List<GachaRelic> _relics;

    private Canvas _canvas;
    

    // 초기 세팅
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Object Bind

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        
        // 결과창 관련
        GetObject((int)EGameObjects.OpenContentObject).SetActive(true);
        GetObject((int)EGameObjects.ContentObject).SetActive(false);
        
        // skip 버튼
        GetButton((int)EButtons.SkipButton).gameObject.BindEvent(OnClickSkipButton);
        
        // Close 버튼
        GetButton((int)EButtons.CloseButton).gameObject.BindEvent(OnClickCloseButton);


        #region Camera
        
        // 캔버스 가져오기
        _canvas = GetComponent<Canvas>();
        
        // Main 카메라 가져오기
        Camera mainCamera = Camera.main;
        
        if (mainCamera != null)
        {
            // Main Camera를 Render Camera로 설정합니다.
            _canvas.worldCamera = mainCamera;
        }
        
        #endregion
        
        #endregion
        
        Refresh();

        return true;
    }

    // 보여질 유물 정보 리스트가져오기
    public void SetRelicsData(List<GachaRelic> gachaRelics)
    {
        _relics = gachaRelics;
        
        Init(); // 이걸 넣어주는 이유는 Bind를 안해서 못 가져옴...
        UIRefresh();
    }

    
    void UIRefresh()
    {
        
        if (_relics != null && _relics.Count > 0)
        {
            foreach (GachaRelic relic in _relics)
            {
                #region 내방식
                // 내 방식
                // GameObject item = Instantiate(UI_GachaRelicObject,
                //     GetObject((int)EGameObjects.GachaResultListObject).transform);

                #endregion
                
                #region 루키스 방식
            
                // 루키스 방식
                GameObject container = GetObject((int)EGameObjects.GachaResultListObject);
                container.DestroyChilds();
                
                UI_GachaRelicObject item = Managers.Resource.Instantiate("UI_GachaRelicObject", pooling: true)
                    .GetOrAddComponent<UI_GachaRelicObject>();
                
                item.transform.SetParent(container.transform);
                #endregion
                // Todo -> 여기까진 맞게 한 듯 (이 이후부터 다르게 해줘야 될 듯!!)
                // 각 아이템들의 정보 설정해주기
                item.GetComponent<UI_GachaRelicObject>().SetInfo(relic);
                
                // 레어리티 유니크 이상 있는지 여부 찾기 -> 있으면 상자 색 황금으로 바꾸기
                
            }    
        }
        else
        {
            Debug.Log("처리할 유물이 없습니다.");
        }
        
    }
    
    // 갱신
    void Refresh()
    {

    }

    void OnClickSkipButton()
    {
        // 뽑기 연출을 스킵하고 결과 보여주기
        GetObject((int)EGameObjects.OpenContentObject).gameObject.SetActive(false);
        GetObject((int)EGameObjects.ContentObject).gameObject.SetActive(true);
        _canvas.worldCamera = null;
    }
    
    void OnClickCloseButton()
    {
        Debug.Log("CloseGachaResultPopup");
        Transform UI_root = gameObject.transform.parent; // @UI_Root
        FindChild(UI_root.gameObject, "UI_LobbyScene").SetActive(true);
        FindChild(UI_root.gameObject, "UI_GachaPopup").SetActive(true);
        Managers.UI.ClosePopupUI(this);
    }


    #region 애니메이션 이벤트, 파티클 이벤트

    [SerializeField] 
    private GameObject _particle;

    public void PlayParticle()
    {
        _particle.SetActive(true);
        StartCoroutine(CoSkil());
    }

    IEnumerator CoSkil()
    {
        yield return new WaitForSeconds(3f);
        OnClickSkipButton();
    }

    #endregion
}
