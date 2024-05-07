using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class UI_NodeMapBase : UI_Base
{
    public int ClearedDepth { get; set; } = 0;
    public int BossDepth { get; protected set; }
    public bool[] ClearedNodes { get; protected set; }
    protected int NodeMapNo;
    protected List<NodeData> NodeList;

    public virtual void LineSync()
    {
        Debug.Log("줄 새로 그리고");
    }
    
    public virtual void NodeSync()
    {
        Debug.Log("노드도 새로 그려!");
    }
    
    protected virtual void OnNodeClick(int clickNode)
    {
        gameObject.transform
            .parent.parent
            .GetComponent<UI_NodeMapPopup>()
            .EnterNode(clickNode, NodeList[clickNode].NodeDepth == BossDepth); // 호출 테스트 함수
    }

    protected void Activate(GameObject node)
    {
        Util.FindChild(node, "Deactivated").SetActive(false);
        Util.FindChild(node, "Activated").SetActive(true);
        Util.FindChild(node, "Visited").SetActive(false);
    }
    
    protected void Deactivate(GameObject node)
    {
        Util.FindChild(node, "Deactivated").SetActive(true);
        Util.FindChild(node, "Activated").SetActive(false);
        Util.FindChild(node, "Visited").SetActive(false);
    }
    
    protected void Visit(GameObject node)
    {
        // Debug.Log(node.transform.GetChild(0).name);
        
        Util.FindChild(node, "Deactivated").SetActive(false);
        Util.FindChild(node, "Activated").SetActive(false);
        Util.FindChild(node, "Visited").SetActive(true);
    }
}
