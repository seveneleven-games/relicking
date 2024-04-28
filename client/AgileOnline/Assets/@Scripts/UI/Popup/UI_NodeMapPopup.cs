using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_NodeMapPopup : UI_Popup
{
    enum GameObjects
    {
        Nodes
    };

    enum Texts
    {
        StageNo
    }

    enum Buttons
    {
        BackButton
    }

    enum Toggles { }

    enum Images
    {
        NodeMapBG
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        DataInit();
        
        GetButton((int)Buttons.BackButton).gameObject.BindEvent(OnBackButtonClick);
        
        Debug.Log("UI_NodeMapScene initialized.");

        return true;
    }

    // 스테이지 정보
    private string _stageNo;
    private string _stageBG;
    private string _nodeMapName;
    
    // 노드 정보
    private GameObject _nodes;
    private UI_NodeMapBase _nodeMap;
    
    void DataInit()
    {
        // NodeMap 정보를 받아와서 노드맵에 반영한다
        // 반영 정보 : 배경화면, 스테이지 번호, 노드맵 종류(프리팹 이름)
        //todo : 데이터 시트에서 스테이지 정보 긁어오기 (데이터 긁어오기 전에, 템플릿ID 랜덤으로 돌려서 노드맵 이름 요청해와야겠다)
        
        _stageNo = STAGE_NO.ToString();
        _stageBG = STAGE_BG_NAME;
        _nodeMapName = STAGE_NODEMAP_NAME;
        
        GetImage((int)Images.NodeMapBG).sprite = Managers.Resource.Load<Sprite>(_stageBG);
        GetText((int)Texts.StageNo).text = _stageNo;
        
        // 노드맵 프리팹 적용하기
        
        // 초기화
        _nodes = GetObject((int)GameObjects.Nodes);
        _nodes.DestroyChilds();
        
        //적용
        _nodeMap = Managers.Resource.Instantiate(_nodeMapName, _nodes.transform).GetComponent<UI_NodeMapBase>() ;
        _nodeMap.gameObject.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
        _nodes.GetComponent<ScrollRect>().content = _nodeMap.GetComponent<RectTransform>();
    }
    
    private void OnEnable()
    {
        if (_nodeMap == null) return;
        
        // 첫 화면 말고 켜질 때 마다 클리어한 노드를 반영해서 새로 그려야 한다
        _nodeMap.Refresh();
    }

    public event Action<int> OnEnterNode;
    
    public void EnterNode(int clickNode)
    {
        // 팝업 닫기
        // 노드 정보 반영시키기
        // 게임 시작하기
        
        /* 설계
         * 1. GameScene에서 UI_NodeMapPopup 활성화
         * 2. EnterNode 실행 시, 노드 번호를 가지고 GameScene의 게임 실행 함수 호출
         * 3. 팝업 닫기 함수 호출
         */
        Debug.Log($"현재 클릭한 노드는 {clickNode}번!");
        if (OnEnterNode != null) OnEnterNode.Invoke(clickNode);
    }

    void OnBackButtonClick()
    {
        //todo : 뒤로가기 버튼 확인 모달 띄우는게 좋지 않을까? ex) 스테이지를 포기하고 로비로 나가시겠습니까?
        
        Managers.Scene.LoadScene(EScene.LobbyScene);
    }
    
}
