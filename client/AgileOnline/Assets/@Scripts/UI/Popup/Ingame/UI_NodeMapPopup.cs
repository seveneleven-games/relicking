using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using Random = UnityEngine.Random;

public class UI_NodeMapPopup : UI_Popup
{
    public TemplateData _templateData;
    
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
        SettingButton
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
        
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(ShowSettingPopup);
        
        Debug.Log("UI_NodeMapScene initialized.");

        return true;
    }

    private void OnEnable()
    {
        if (_nodeMap == null) return;

        // 첫 화면 말고 켜질 때 마다 클리어한 노드를 반영해서 새로 그려야 한다
        _nodeMap.NodeSync();
        _nodeMap.LineSync();
    }


    // 스테이지 정보
    public string _stageNo { get; private set; }
    private string _stageBG;
    private int _nodeMapNo;
    private string _nodeMapName;
    
    // 노드 정보
    private GameObject _nodes;
    private UI_NodeMapBase _nodeMap;
    
    void DataInit()
    {
        // NodeMap 정보를 받아와서 노드맵에 반영한다
        // 반영 정보 : 배경화면, 스테이지 번호, 노드맵 종류(프리팹 이름)
        _templateData = Resources.Load<TemplateData>("GameTemplateData");
        int stageId = _templateData.StageId;

        _stageNo = _templateData.StageId.ToString();
        
        StageData stageData = Managers.Data.StageDic[stageId];
        int[] nodeMaps = stageData.NodeMaps;
        
        _nodeMapNo = nodeMaps[Random.Range(0, nodeMaps.Length)];
        _templateData.TempNodeNum = _nodeMapNo;
        
        _stageBG = Managers.Data.NodeMapDic[_nodeMapNo].BackgroundImage;
        _nodeMapName = Managers.Data.NodeMapDic[_nodeMapNo].PrefabName;
        
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
    
    public event Action<int, bool> OnEnterNode;
    
    public void EnterNode(int clickNode, bool isBossNode)
    {
        // 팝업 닫기
        // 노드 정보 반영시키기
        // 게임 시작하기
        
        /* 설계
         * 1. GameScene에서 UI_NodeMapPopup 활성화
         * 2. EnterNode 실행 시, 노드 번호를 가지고 GameScene의 게임 실행 함수 호출
         * 3. 팝업 닫기 함수 호출
         */
        Debug.Log($"현재 클릭한 노드는 {clickNode}번! 보스노드 여부! : {isBossNode}");
        OnEnterNode?.Invoke(clickNode, isBossNode);
    }

    void ShowSettingPopup()
    {
        Managers.UI.ShowPopupUI<UI_IngameSettingPopup>();
    }
    
    void OnExitGame()
    {
        //todo(전지환) : ExitConfirmPopup에서 실행하기
        PlayerController player = Managers.Object.Player;
        if (player != null)
        {
            player.gameObject.SetActive(false);
            Managers.Object.Player = null;
        }
        StopAllCoroutines();
        CleanupResources();
        Managers.Scene.LoadScene(EScene.LobbyScene);
    }

    public void DataSync(int nodeNo)
    {
        _nodeMap.ClearedNodes[nodeNo] = true;
        _nodeMap.ClearedDepth += 1;
    }
    
    private void CleanupResources()
    {
        // 몬스터와 골드 오브젝트 despawn
        DespawnObjects<MonsterController>("@Monsters");
        DespawnObjects<GoldController>("@Golds");

        // 맵 오브젝트 파괴
        DestroyObjects("@BaseMap");

        // 오브젝트 풀 정리
        Managers.Pool.Clear();
    }

    private void DespawnObjects<T>(string parentName) where T : MonoBehaviour
    {
        GameObject parentObject = GameObject.Find(parentName);
        if (parentObject != null)
        {
            foreach (Transform child in parentObject.transform)
            {
                T component = child.gameObject.GetComponent<T>();
                if (component != null)
                {
                    BaseController baseController = component as BaseController;
                    if (baseController != null)
                        Managers.Object.Despawn(baseController);
                }
            }
        }
    }

    private void DestroyObjects(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            Managers.Resource.Destroy(obj);
        }
    }
}
