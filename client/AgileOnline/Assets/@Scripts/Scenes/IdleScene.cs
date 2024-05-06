using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleScene : BaseScene
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.EScene.IdleScene;
        
        // UI
        Managers.UI.ShowSceneUI<UI_IdleScene>();
        //Screen.sleepTimeout = SleepTimeout.SystemSetting;
        
        
        return true;
    }

    public override void Clear()
    {

    }
}