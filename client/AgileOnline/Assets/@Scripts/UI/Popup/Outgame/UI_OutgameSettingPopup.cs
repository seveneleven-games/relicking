using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_OutgameSettingPopup : UI_Popup
{
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
    
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindToggle(typeof(Toggles));
        
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(ClosePopupUI);
        
        //todo(전지환) : 설정 정보에 따라 toggle 싱크 맞춰줘야 함
        GetText((int)Texts.BGMOFFText).gameObject.SetActive(false);
        GetText((int)Texts.SFXOFFText).gameObject.SetActive(false);

        return true;
    }
    
    
}
