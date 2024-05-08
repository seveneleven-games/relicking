using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_ClearPopup : UI_Popup
{
    enum GameObjects
    {
        RelicSlot1,
        RelicSlot2,
        RelicSlot3,
        RelicSlot4,
        RelicSlot5,
        RelicSlot6,
        SkillSlot1,
        SkillSlot2,
        SkillSlot3,
        SkillSlot4,
        SkillSlot5,
        SkillSlot6,
    };

    enum Texts
    {
        StageInfo,
        DifficultyInfo
    }

    enum Buttons
    { }

    enum Toggles
    { }

    enum Images
    {
        BG,
        ClassImage
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindToggle(typeof(Toggles));
        BindImage(typeof(Images));
        
        GetImage((int)Images.BG).gameObject.BindEvent(ClosePopupUI);

        return true;
    }

    public override void ClosePopupUI()
    {
        ExitGame();
        base.ClosePopupUI();
    }
    
    void ExitGame()
    {
        PlayerController player = Managers.Object.Player;
        if (player != null)
        {
            player.gameObject.SetActive(false);
            Managers.Object.Player = null;
        }
        StopAllCoroutines();
        Managers.Object.CleanupResources();
        Managers.Scene.LoadScene(EScene.LobbyScene);
    }
}
