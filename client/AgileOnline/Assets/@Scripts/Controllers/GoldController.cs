using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class GoldController : BaseController
{
    public int GoldValue { get; private set; }
    public string PrefabName { get; private set; }
    public string Name { get; private set; }
    public int DropGold { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = Define.EObjectType.Env;

        return true;
    }

    public void InitGold(int templateId)
    {
        GoldData data = Managers.Data.GoldDic[Managers.Data.MonsterDic[templateId].DropGold];
        
        GoldValue = data.DropGold;
        PrefabName = data.PrefabName;
        Name = data.Name;
        DropGold = data.DropGold;
    }
}