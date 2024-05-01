using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InvenEquipPopup : UI_Popup
{
    #region Enum
    enum EGameObjects
    {
        ContentObjet,
    }

    enum EButtons
    {
        ButtonCancel,
        EquipButton1,
        EquipButton2,
        EquipButton3,
        EquipButton4,
        EquipButton5,
        EquipButton6,
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

        GetButton((int)EButtons.ButtonCancel).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)EButtons.EquipButton1).gameObject.BindEvent(() => OnClickEquipButton(1));
        GetButton((int)EButtons.EquipButton2).gameObject.BindEvent(() => OnClickEquipButton(2));
        GetButton((int)EButtons.EquipButton3).gameObject.BindEvent(() => OnClickEquipButton(3));
        GetButton((int)EButtons.EquipButton4).gameObject.BindEvent(() => OnClickEquipButton(4));
        GetButton((int)EButtons.EquipButton5).gameObject.BindEvent(() => OnClickEquipButton(5));
        GetButton((int)EButtons.EquipButton6).gameObject.BindEvent(() => OnClickEquipButton(6));

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
        Debug.Log("CloseEquipPopup");
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickEquipButton(int number)
    {
        Debug.Log($"Button number {number}");
        Managers.UI.ClosePopupUI(this);
    }
}
