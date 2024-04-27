using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNodeMapScene : UI_Scene
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        Managers.UI.ShowPopupUI<UI_NodeMapPopup>();
        
        return true;
    }
}
