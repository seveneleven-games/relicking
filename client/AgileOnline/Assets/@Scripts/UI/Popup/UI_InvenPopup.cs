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
        RelicListObject,
        StatListObject,
    }

    enum EButtons
    {
        ClassButton,
        RelicButton,
    }

    enum ETexts
    {
        MaxHealthText,
        DamageText,
        SpeedText,
        CoinBonusText,
        CriticalRateText,
        CriticalDamageText,
        CoolDownText,
    }

    enum EToggles
    {
        RelicToggle,
        StatToggle,
    }

    enum EImages
    {
        RelicToggleBGImage,
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

        GetToggle((int)EToggles.RelicToggle).gameObject.BindEvent(OnClickRelicToggle);
        GetToggle((int)EToggles.StatToggle).gameObject.BindEvent(OnClickStatToggle);
        GetButton((int)EButtons.ClassButton).gameObject.BindEvent(OnClickClassSelectButton);
        GetButton((int)EButtons.RelicButton).gameObject.BindEvent(OnClickRelicInfoButton);

        ToggleInit();
        OnClickRelicToggle();

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
        GetObject((int)EGameObjects.RelicListObject).SetActive(false);
        GetObject((int)EGameObjects.StatListObject).SetActive(false);

        GetImage((int)EImages.RelicToggleBGImage).color = Util.HexToColor("B9A691");
        GetImage((int)EImages.StatToggleBGImage).color = Util.HexToColor("B9A691");
    }

    void OnClickRelicToggle()
    {
        ToggleInit();
        GetImage((int)EImages.RelicToggleBGImage).color = Util.HexToColor("B38C61");
        if (_isSelectedEquip == true)
            return;

        GetObject((int)EGameObjects.RelicListObject).SetActive(true);
        _isSelectedEquip = true;
    }

    void OnClickStatToggle()
    {
        ToggleInit();
        GetImage((int)EImages.StatToggleBGImage).color = Util.HexToColor("B38C61");
        if (_isSelectedStat == true)
            return;

        GetObject((int)EGameObjects.StatListObject).SetActive(true);
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
