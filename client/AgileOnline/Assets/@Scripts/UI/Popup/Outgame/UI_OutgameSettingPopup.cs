using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_OutgameSettingPopup : UI_Popup
{
    enum Texts
    {
        BGMONText,
        SFXONText,
        BGMOFFText,
        SFXOFFText,
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
    
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindToggle(typeof(Toggles));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(ClosePopupUI);
        
        GetText((int)Texts.BGMOFFText).gameObject.SetActive(false);
        GetText((int)Texts.SFXOFFText).gameObject.SetActive(false);

        return true;
    }
    
    
}
