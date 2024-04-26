using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InvenRelicInfoPopup : UI_Popup
{
    #region Enum
    enum EGameObjects
    {
        ContentObjet,
    }

    enum EButtons
    {
        ButtonBG,
        ButtonEquip,
        ButtonCancel,
    }

    enum ETexts
    {
        LevelText,
        RelicNameText,
        RelicDescriptionText,
    }

    enum EImages
    {

    }

    #endregion

    public void OnDestroy()
    {
        if (Managers.Game != null)
            Managers.Game.OnResourcesChanged -= Refresh;
    }

    // 초기 세팅
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Object Bind

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));
        BindImage(typeof(EImages));

        GetButton((int)EButtons.ButtonBG).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)EButtons.ButtonCancel).gameObject.BindEvent(OnClickCloseButton);


        #endregion

        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();

        return true;
    }

    // 갱신
    void Refresh()
    {

    }

    void OnClickCloseButton()
    {
        Debug.Log("CloseGachaInfo");
        Managers.UI.ClosePopupUI(this);
    }
}
