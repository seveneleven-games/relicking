using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public class UI_GameExitConfirmPopup : UI_Popup
{
    enum Buttons
    {
        CloseButton,
        ExitButton
    }

    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(ExitGame);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(ClosePopupUI);
        
        return true;
    }
    
    void ExitGame()
    {
        Managers.Sound.PlayButtonClick();
        PlayerController player = Managers.Object.Player;
        if (player != null)
        {
            player.gameObject.SetActive(false);
            Managers.Object.Player = null;
        }
        StopAllCoroutines();
        Managers.Object.CleanupResources();
        Time.timeScale = 1;
        Managers.Scene.LoadScene(EScene.LobbyScene);
    }
    
}
