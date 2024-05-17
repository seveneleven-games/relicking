using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;

[Serializable]
public class RelicDataReq
{
    public int slot;
    public int relicNo;
}

[Serializable]
public class RelicDataRes
{
    public int status;
    public string message;
    public RelicRes data;
}

[Serializable]
public class RelicRes
{
    public bool result;
}

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

    TemplateData _templateData;

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
        GetButton((int)EButtons.EquipButton1).gameObject.BindEvent(() => OnClickEquipButton(0));
        GetButton((int)EButtons.EquipButton2).gameObject.BindEvent(() => OnClickEquipButton(1));
        GetButton((int)EButtons.EquipButton3).gameObject.BindEvent(() => OnClickEquipButton(2));
        GetButton((int)EButtons.EquipButton4).gameObject.BindEvent(() => OnClickEquipButton(3));
        GetButton((int)EButtons.EquipButton5).gameObject.BindEvent(() => OnClickEquipButton(4));
        GetButton((int)EButtons.EquipButton6).gameObject.BindEvent(() => OnClickEquipButton(5));

        _templateData = Resources.Load<TemplateData>("GameTemplateData");

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
        Managers.Sound.PlayButtonClick();
        Debug.Log("CloseEquipPopup");
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickEquipButton(int number)
    {
        EquipRelic(number);
    }

    void EquipRelic(int number)
    {
        RelicDataReq relicDataReq = new()
        {
            slot = number + 1,
            relicNo = _templateData.SelectedRelicId / 10,
        };

        string relicJsonData = JsonUtility.ToJson(relicDataReq);

        StartCoroutine(JWTPostRequest("relics", relicJsonData, res =>
        {
            RelicDataRes relicDataRes = JsonUtility.FromJson<RelicDataRes>(res);
 
            if (relicDataRes.status == 200)
            {
                Managers.Sound.Play(Define.ESound.Effect,"Equip_Inventory");
                _templateData.SetRelicAt(number, _templateData.SelectedRelicId);
                Debug.Log($"Button number {number}");
                Debug.Log("[" + string.Join(", ", _templateData.EquipedRelicIds) + "]");
                Managers.UI.ClosePopupUI(this);
            }
        }));
    }
}
