using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStoreScene : BaseScene
{
    private UI_StorePopup _store;
    
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        _store = Managers.UI.ShowPopupUI<UI_StorePopup>();
        
        return true;
    }

    public override void Clear()
    {
        
    }
}
