using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DeadPopup : UI_Popup
{

    enum Buttons
    {
        ExitButton
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindButton(typeof(Buttons));
        
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(Exit);

        return true;
    }
    
    void Exit()
    {
        Managers.Scene.LoadScene(Define.EScene.LobbyScene);
        ClosePopupUI();
    }
}
