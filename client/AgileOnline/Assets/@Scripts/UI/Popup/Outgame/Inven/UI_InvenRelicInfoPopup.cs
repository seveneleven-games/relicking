using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Util;

public class UI_InvenRelicInfoPopup : UI_Popup
{
    #region Enum
    enum EGameObjects
    {
        ContentObjet,
        //RelicExpSliderObject,
    }

    enum EButtons
    {
        ButtonBG,
        ButtonEquip,
        ButtonUnequip,
        ButtonCancel,
    }

    enum ETexts
    {
        RelicLevelText,
        RelicNameText,
        RelicDescriptionText,
        RelicRarityText,
    }

    enum EImages
    {
        RelicImage
    }

    TemplateData _templateData;

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
        GetButton((int)EButtons.ButtonEquip).gameObject.BindEvent(OnClickEquipButton);
        GetButton((int)EButtons.ButtonUnequip).gameObject.BindEvent(OnClickUnequipButton);

        _templateData = Resources.Load<TemplateData>("GameTemplateData");

        RelicData relicData = Managers.Data.RelicDic[_templateData.SelectedRelicId];

        switch (relicData.Rarity)
        {
            case 0:
                GetText((int)ETexts.RelicRarityText).text = "C";
                GetText((int)ETexts.RelicRarityText).color = Util.HexToColor("696969");
                break;
            case 1:
                GetText((int)ETexts.RelicRarityText).text = "B";
                GetText((int)ETexts.RelicRarityText).color = Util.HexToColor("2641CB");
                break;
            case 2:
                GetText((int)ETexts.RelicRarityText).text = "A";
                GetText((int)ETexts.RelicRarityText).color = Util.HexToColor("A507CC");
                break;
            case 3:
                GetText((int)ETexts.RelicRarityText).text = "S";
                GetText((int)ETexts.RelicRarityText).color = Util.HexToColor("FBFF31");
                break;
            case 4:
                GetText((int)ETexts.RelicRarityText).text = "SSS";
                GetText((int)ETexts.RelicRarityText).color = Util.HexToColor("FF0000");
                break;
        }
        GetText((int)ETexts.RelicNameText).text = relicData.Name;
        GetText((int)ETexts.RelicDescriptionText).text = relicData.Description + RelicDetailDescription(relicData);
        string levelText = _templateData.SelectedRelicId % 10 == 9 ? "Max" : (_templateData.SelectedRelicId % 10).ToString();
        GetText((int)ETexts.RelicLevelText).text = "Lv." + levelText;
        Sprite spr = Managers.Resource.Load<Sprite>(relicData.ThumbnailName);
        GetImage((int)EImages.RelicImage).sprite = spr;

        bool isEquip = IsRelicEquiped(_templateData.SelectedRelicId, _templateData.EquipedRelicIds);
        GetButton((int)EButtons.ButtonUnequip).gameObject.SetActive(isEquip);
        
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
        Debug.Log("CloseRelicDetail");
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickEquipButton()
    {
        Debug.Log("EquipClicked");
        Managers.UI.ClosePopupUI(this);
        Managers.UI.ShowPopupUI<UI_InvenEquipPopup>();
    }

    void OnClickUnequipButton()
    {
        int EquipedIndex = -1;
        for (int i = 0; i < _templateData.EquipedRelicIds.Length; i++)
        {
            if (_templateData.SelectedRelicId == _templateData.EquipedRelicIds[i])
            {
                EquipedIndex = i;
                break;
            }
        }
        if ( EquipedIndex >= 0)
        {
            Debug.Log($"Call UnequipRelic Function");
            UnEquipRelic(EquipedIndex);
            //_templateData.SetRelicAt(EquipedIndex, 0);
            //Debug.Log("[" + string.Join(", ", _templateData.EquipedRelicIds) + "]");
        }
    }

    public bool IsRelicEquiped(int relicId, int[] relicIds)
    {
        bool result = false;
        for (int i = 0; i < relicIds.Length; i++)
        {
            if (relicId == relicIds[i])
            {
                result = true;
                break;
            }
        }

        return result;
    }

    public string RelicDetailDescription(RelicData data)
    {
        string result = "";
        if (data.Atk > 0)
        {
            result += $"\n공격력 +{data.Atk}";
        }
        if (data.MaxHp > 0)
        {
            result += $"\n최대체력 +{data.MaxHp}";
        }
        if (data.CoolTime > 0)
        {
            result += $"\n쿨타임 {data.CoolTime}% 감소";
        }
        if (data.Speed > 0)
        {
            result += $"\n이동속도 +{data.Speed}";
        }

        return result;
    }

    void UnEquipRelic(int number)
    {
        RelicDataReq relicDataReq = new()
        {
            slot = number + 1,
            relicNo = 0,
        };

        string relicJsonData = JsonUtility.ToJson(relicDataReq);

        StartCoroutine(JWTPostRequest("relics", relicJsonData, res =>
        {
            RelicDataRes relicDataRes = JsonUtility.FromJson<RelicDataRes>(res);

            if (relicDataRes.status == 200)
            {
                _templateData.SetRelicAt(number, 0);
                Debug.Log("[" + string.Join(", ", _templateData.EquipedRelicIds) + "]");
                Managers.UI.ClosePopupUI(this);
            }
        }));
    }
}
