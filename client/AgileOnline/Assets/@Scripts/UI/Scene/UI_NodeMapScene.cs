using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_NodeMapScene : UI_Scene
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
        
        Debug.Log("UI_NodeMapScene initialized.");

        return true;
    }

    // 스테이지 정보
    private string _stageNo;
    private string _stageBG;
    private string _nodeMapName;
    
    // 노드 정보
    private GameObject _nodes;
    private GameObject _nodeMap;
    
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
        _nodeMap = Managers.Resource.Instantiate(_nodeMapName, _nodes.transform) ;
        _nodeMap.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
        _nodes.GetComponent<ScrollRect>().content = _nodeMap.GetComponent<RectTransform>();
    }
}
