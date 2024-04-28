using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Stage1NodeMap_01 : UI_NodeMapBase
{
    #region UI_NodeMapBase GameObjects enum 작성 순서
    /*
     * 1. 클리어한 노드를 _clearedNodes 배열(bool)로 관리하기 때문에 노드 매핑 순서가 매우 중요
     * : Depth가 낮은 순서로, 왼쪽에서 오른쪽 순서로 작성할 것.
     * => 해당 데이터는 데이터시트에 저장될 것을 기반으로 함
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
    
    //todo : 상위 스크립트를 상속 받아서 노드맵 팝업에서 해당 스크립트로 데이터 받아오는 virtual 함수를 만들어야 할 것 같음

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
        
        return true;
    }
    
    void DataInit()
    {
        BossDepth = 5;
        ClearedNodes = new bool[5];  
    }
    
}
