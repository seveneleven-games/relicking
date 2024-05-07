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
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        ShowIdleRewardDialog();

        return true;
    }

    public override void Clear()
    {

    }

    void ShowIdleRewardDialog()
    {
        if (Managers.Game.showIdleRewardPopup)
        {
            //그 리워드 관련 팝업으로 바꿔주기
            Managers.UI.ShowPopupUI<UI_ToBeContinuedPopup>();
            Managers.Game.showIdleRewardPopup = false;
        }
    }
}
