using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        SceneType = Define.EScene.LobbyScene;

        // UI
        Managers.UI.ShowSceneUI<UI_LobbyScene>();
       // Screen.sleepTimeout = SleepTimeout.SystemSetting;

        

        return true;
    }

    public override void Clear()
    {

    }

   
}
