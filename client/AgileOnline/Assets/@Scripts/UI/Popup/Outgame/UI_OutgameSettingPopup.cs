using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_OutgameSettingPopup : UI_Popup
{
    enum GameObjects
    {
        RelicSlot1,
        RelicSlot2,
        RelicSlot3,
        RelicSlot4,
        RelicSlot5,
        RelicSlot6
    };

    enum Texts
    {
        BGMONText,
        BGMOFFText,
        SFXONText,
        SFXOFFText
    }

    enum Buttons
    {
        CloseButton
    }

    enum Toggles
    {
        BGMSoundToggle,
        SFXSoundToggle,
    }

    enum Images
    {
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
        
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(ClosePopupUI);
        
        //todo(전지환) : 설정 정보에 따라 toggle 싱크 맞춰줘야 함
        GetText((int)Texts.BGMOFFText).gameObject.SetActive(false);
        GetText((int)Texts.SFXOFFText).gameObject.SetActive(false);

        return true;
    }
    
    
}
