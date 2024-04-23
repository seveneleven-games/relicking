using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldController : BaseController
{
    public int GoldValue { get; private set; }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = Define.EObjectType.Env;

        return true;
    }
    
    public void InitGold(Data.MonsterData data)
    {
        GoldValue = data.DropGold;
    }
}
