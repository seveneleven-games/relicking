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

        GetText((int)ETexts.RelicNameText).text = relicData.Name;
        GetText((int)ETexts.RelicDescriptionText).text = relicData.Description + $"\n 공격력 +{relicData.Atk} 쿨타임 {relicData.CoolTime}%감소 \n최대체력 +{relicData.MaxHp} 이동속도 0.{relicData.Speed} 증가";
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
