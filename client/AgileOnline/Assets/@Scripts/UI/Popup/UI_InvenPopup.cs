using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InvenPopup : UI_Popup
{
    #region Enum

    enum EGameObjects
    {
        ContentObjet,
        ToggleGroup,
        RelicObject,
        StatObject,
    }

    enum EButtons
    {
        RelicClassButton,
        StatClassButton,
        RelicButton,
    }

    enum ETexts
    {
        
    }

    enum EToggles
    {
        EquipToggle,
        StatToggle,
    }

    enum EImages
    {
        EquipToggleBGImage,
        StatToggleBGImage,
    }

    #endregion

    bool _isSelectedEquip = false;
    bool _isSelectedStat = false;

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
        BindToggle(typeof(EToggles));
        BindImage(typeof(EImages));

        #endregion

        GetToggle((int)EToggles.EquipToggle).gameObject.BindEvent(OnClickEquipToggle);
        GetToggle((int)EToggles.StatToggle).gameObject.BindEvent(OnClickStatToggle);
        GetButton((int)EButtons.RelicClassButton).gameObject.BindEvent(OnClickClassSelectButton);
        GetButton((int)EButtons.StatClassButton).gameObject.BindEvent(OnClickClassSelectButton);
        GetButton((int)EButtons.RelicButton).gameObject.BindEvent(OnClickRelicInfoButton);

        ToggleInit();
        OnClickEquipToggle();

        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();

        return true;
    }

    // 갱신
    void Refresh()
    {

    }

    void ToggleInit()
    {
        // 선택여부 초기화
        _isSelectedEquip = false;
        _isSelectedStat = false;

        // 팝업버튼 초기화
        GetObject((int)EGameObjects.RelicObject).SetActive(false);
        GetObject((int)EGameObjects.StatObject).SetActive(false);

        GetImage((int)EImages.EquipToggleBGImage).color = Util.HexToColor("B9A691");
        GetImage((int)EImages.StatToggleBGImage).color = Util.HexToColor("B9A691");
    }

    void OnClickEquipToggle()
    {
        ToggleInit();
        GetImage((int)EImages.EquipToggleBGImage).color = Util.HexToColor("B38C61");
        if (_isSelectedEquip == true)
            return;

        GetObject((int)EGameObjects.RelicObject).SetActive(true);
        _isSelectedEquip = true;
    }

    void OnClickStatToggle()
    {
        ToggleInit();
        GetImage((int)EImages.StatToggleBGImage).color = Util.HexToColor("B38C61");
        if (_isSelectedStat == true)
            return;

        GetObject((int)EGameObjects.StatObject).SetActive(true);
        _isSelectedStat= true;
    }

    void OnClickClassSelectButton()
    {
        Managers.UI.ShowPopupUI<UI_InvenClassSelectPopup>();
    }

    void OnClickRelicInfoButton()
    {
        Managers.UI.ShowPopupUI<UI_InvenRelicInfoPopup>();
    }
}
