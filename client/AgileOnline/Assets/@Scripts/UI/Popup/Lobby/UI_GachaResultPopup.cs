using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UI_GachaResultPopup : UI_Popup
{
    
    // 사용할 프리팹 가져오기
    [SerializeField]
    GameObject UI_GachaRelicObject;
    
    
    #region Enum
    enum EGameObjects
    {
        GachaResultListObject, // 위치 필요
    }

    enum EButtons
    {
        CloseButton,
    }

    enum ETexts
    {

    }

    #endregion

    // 객체 관련 쓰는 곳
    private List<GachaRelic> _relics;
    

    // 초기 세팅
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Object Bind

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));

        GetButton((int)EButtons.CloseButton).gameObject.BindEvent(OnClickCloseButton);
       
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

    void OnClickCloseButton()
    {
        Debug.Log("CloseGachaResultPopup");
        Managers.UI.ClosePopupUI(this);
    }
}
