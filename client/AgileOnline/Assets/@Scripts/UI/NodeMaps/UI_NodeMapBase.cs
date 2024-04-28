using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_NodeMapBase : UI_Base
{
    public int ClearedDepth { get; protected set; } = 0;
    public int BossDepth { get; protected set; }
    public bool[] ClearedNodes { get; protected set; }
    
    public virtual void DataSync()
    {
        
    }
    
    public virtual void OnNodeClick(int clickNode)
    {
        gameObject.transform
            .parent.parent
            .GetComponent<UI_NodeMapPopup>()
            .EnterNode(clickNode); // 호출 테스트 함수
    }
    
    public void Refresh()
    {
        Debug.Log("재활성화 확인!!");
    }
}
