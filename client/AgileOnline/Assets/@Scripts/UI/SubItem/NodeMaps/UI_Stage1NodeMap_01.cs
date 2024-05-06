using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class UI_Stage1NodeMap_01 : UI_NodeMapBase
{
    #region UI_NodeMapBase enum 작성 순서
    /*
     * 클리어한 노드를 _clearedNodes 배열(bool)로 관리하기 때문에 노드 매핑 순서가 매우 중요
     * : Depth가 낮은 순서로, 왼쪽에서 오른쪽 순서로 작성할 것.
     * => 해당 데이터는 데이터시트에 저장될 것을 기반으로 함
     *
     * Lines도 매핑되는 노드 순서에 맞게 작성 필요
     */
    #endregion
    
    enum GameObjects
    {
        NormalNodeDepth1,
        NormalNodeDepth2,
        EliteNodeDepth3,
        NormalNodeDepth4,
        BossNodeDepth5,
        Line1To2,
        Line2To3,
        Line3To4,
        Line4To5,
    }

    enum Nodes
    {
        NormalNodeDepth1,
        NormalNodeDepth2,
        EliteNodeDepth3,
        NormalNodeDepth4,
        BossNodeDepth5,
    }
    
    enum Lines
    {
        Line1To2,
        Line2To3,
        Line3To4,
        Line4To5,
    }
    
    //todo(전지환) : 상위 스크립트를 상속 받아서 노드맵 팝업에서 해당 스크립트로 데이터 받아오는 virtual 함수를 만들어야 할 것 같음

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        DataInit();

        BindObject(typeof(GameObjects));

        Util.FindChild(GetObject((int)GameObjects.NormalNodeDepth1), "Activated")
            .BindEvent(()=>OnNodeClick((int)GameObjects.NormalNodeDepth1));
        Util.FindChild(GetObject((int)GameObjects.NormalNodeDepth2), "Activated")
            .BindEvent(()=>OnNodeClick((int)GameObjects.NormalNodeDepth2));
        Util.FindChild(GetObject((int)GameObjects.EliteNodeDepth3), "Activated")
            .BindEvent(()=>OnNodeClick((int)GameObjects.EliteNodeDepth3));
        Util.FindChild(GetObject((int)GameObjects.NormalNodeDepth4), "Activated")
            .BindEvent(()=>OnNodeClick((int)GameObjects.NormalNodeDepth4));
        Util.FindChild(GetObject((int)GameObjects.BossNodeDepth5), "Activated")
            .BindEvent(()=>OnNodeClick((int)GameObjects.BossNodeDepth5));
        
        LineSync();
        
        return true;
    }
    
    void DataInit()
    {
        BossDepth = 5;
        ClearedNodes = new bool[5];
        NodeMapNo = 0;
        NodeList = Managers.Data.NodeMapDic[NodeMapNo].NodeList;
    }

    public override void NodeSync()
    {
        Debug.Log($"노드맵 테스트중 Step5. 노드 싱크");
        
        foreach (var node in Enum.GetValues(typeof(Nodes)))
        {
            Debug.Log(GetObject((int)node).name);
            
            if (ClearedNodes[(int)node])
                Visit(GetObject((int)node));
            else if (NodeList[(int)node].NodeDepth <= ClearedDepth)
                Deactivate(GetObject((int)node));
            else if (NodeList[(int)node].NodeDepth == ClearedDepth + 1)
                Activate(GetObject((int)node));
        }

    }

    // 리팩터링 필요! 개같은거!
    public override void LineSync()
    {
        Debug.Log($"노드맵 테스트중 Step6. 라인 싱크");
        
        int index;
        
        foreach (int order in Enum.GetValues(typeof(Lines))) 
        {
            index =  Enum.GetValues(typeof(Nodes)).Length + order;
            if (ClearedNodes[order+1])
            {
                //todo(전지환) : 라인 컴포넌트 토글로 만들어주는게 편할 것 같음 
                Util.FindChild(GetObject(index), "SolidLine").SetActive(true);
                Util.FindChild(GetObject(index), "CircleDottedLine").SetActive(false);
            }
            
        }
    }
    
}
