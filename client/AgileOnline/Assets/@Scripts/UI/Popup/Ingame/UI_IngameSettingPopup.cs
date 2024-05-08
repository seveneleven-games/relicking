using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_IngameSettingPopup : UI_Popup
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
        BGMONText,
        SFXONText,
        BGMOFFText,
        SFXOFFText,
    }

    enum Buttons
    {
        CloseButton,
        ExitButton
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
        
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(ShowConfirmPopup);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(ClosePopupUI);

        GetText((int)Texts.BGMOFFText).gameObject.SetActive(false);
        GetText((int)Texts.SFXOFFText).gameObject.SetActive(false);

        //todo(전지환) : 설정 정보에 따라 toggle 싱크 맞춰줘야 함

        return true;
    }

    void ShowConfirmPopup()
    {
        Managers.UI.ShowPopupUI<UI_GameExitConfirmPopup>();
    }

    public override void ClosePopupUI()
    {
        Time.timeScale = 1;
        base.ClosePopupUI();
    }

    //todo(전지환) : 유물 정보 가져와서 싱크 맞춰주기
    //todo(전지환) : player 정보 가져와서 스킬 맞춰주기
    
}
