using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GachaPopup : UI_Popup
{
    #region Enum

    enum EGameObjects
    {
        ContentObjet,
    }

    enum EButtons
    {
        DetailInfoButton,
        GachaOneButton,
        GachaTenButton,
    }

    enum ETexts
    {
        GachaTitle,
        Tickets,
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

        GetButton((int)EButtons.DetailInfoButton).gameObject.BindEvent(OnClickDetailInfoButton);
        GetButton((int)EButtons.GachaOneButton).gameObject.BindEvent(OnClickGachaOneButton);
        GetButton((int)EButtons.GachaTenButton).gameObject.BindEvent(OnClickGachaTenButton);

        #endregion

        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();

        return true;
    }

    // 갱신
    void Refresh()
    {

    }

    void OnClickDetailInfoButton()
    {
        Debug.Log("InfoButtonClicked");
        Managers.UI.ShowPopupUI<UI_GachaInfoPopup>();
    }

    void OnClickGachaOneButton()
    {
        Debug.Log("GachaOne");
    }

    void OnClickGachaTenButton()
    {
        Debug.Log("GachaTen");
        //Managers.UI.ShowPopupUI<UI_GachaNoTicketPopup>();
    }
}
