using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNodeMapScene : BaseScene
{
    private UI_NodeMapPopup _nodeMap;
    private int _nodeNo;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        _nodeMap = Managers.UI.ShowPopupUI<UI_NodeMapPopup>();
        _nodeMap.OnEnterNode += GameStart;
        
        return true;
    }

    void GameStart(int nodeNo)
    {
        // 여기에 팝업 닫는 함수 있어야 할 것.
        _nodeMap.ClosePopupUI();
        Debug.Log($"{nodeNo}번 노드 진입! 게임 시작!!");
        Managers.Scene.LoadScene(Define.EScene.GameScene);
    }
    
    public override void Clear()
    {

    }
}
