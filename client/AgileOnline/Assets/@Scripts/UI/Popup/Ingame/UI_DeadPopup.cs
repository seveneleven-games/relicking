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
        
        Managers.Sound.Stop(Define.ESound.Bgm);
        Managers.Sound.Play(Define.ESound.Effect,"GameOver",0.7f);
        
        BindButton(typeof(Buttons));
        
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(Exit);

        return true;
    }
    
    void Exit()
    {
        Managers.Sound.PlayButtonClick();
        Managers.Scene.LoadScene(Define.EScene.LobbyScene);
        ClosePopupUI();
    }
}
