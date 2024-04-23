using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldController : BaseController
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = Define.EObjectType.Env;

        return true;
    }
}
