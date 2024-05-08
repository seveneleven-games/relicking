using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        BGMOFFText,
        SFXONText,
        SFXOFFText
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

    private TemplateData _templateData;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _templateData = Resources.Load<TemplateData>("GameTemplateData");

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindToggle(typeof(Toggles));
        BindImage(typeof(Images));

        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(ShowConfirmPopup);
        GetButton((int)Buttons.CloseButton).gameObject.BindEvent(ClosePopupUI);

        SetRelicInfo();

        //todo(전지환) : 설정 정보에 따라 toggle 싱크 맞춰줘야 함
        GetText((int)Texts.BGMOFFText).gameObject.SetActive(false);
        GetText((int)Texts.SFXOFFText).gameObject.SetActive(false);

        return true;
    }

    void SetRelicInfo()
    {
        int[] relicIds = _templateData.EquipedRelicIds;
        List<int> skillIds = Managers.Object.Player.PlayerSkillList;

        foreach (var skillId in skillIds)
        {
            Debug.Log(skillId);
        }

        for (int i = (int)GameObjects.RelicSlot1; i <= (int)GameObjects.RelicSlot5; i++)
        {
            GameObject relicSlot = GetObject(i);
            int relicIndex = i - (int)GameObjects.RelicSlot1;

            if (relicIndex >= 0 && relicIndex < relicIds.Length && relicIds[relicIndex] > 0)
            {
                string spriteName = Managers.Data.RelicDic[relicIds[relicIndex]].ThumbnailName;
                Sprite relicSprite = Managers.Resource.Load<Sprite>(spriteName);

                Transform relicImageTransform = relicSlot.transform.Find("RelicImage");
                Image relicImage = relicImageTransform.GetComponent<Image>();
                relicImage.sprite = relicSprite;
                relicImage.gameObject.SetActive(true);
            }
        }

        for (int i = (int)GameObjects.SkillSlot1; i <= (int)GameObjects.SkillSlot5; i++)
        {
            GameObject skillSlot = GetObject(i);
            int skillIndex = i - (int)GameObjects.SkillSlot1;

            if (skillIndex >= 0 && skillIndex < skillIds.Count)
            {
                int skillId = skillIds[skillIndex];

                if (skillId == 0)
                {
                    continue;
                }

                string spriteName = Managers.Data.SkillDic[skillId].IconName;
                Sprite skillSprite = Managers.Resource.Load<Sprite>(spriteName);
                
                Transform skillImageTransform = skillSlot.transform.Find("SkillImage");
                Transform maskTransform = skillImageTransform.transform.Find("Mask");
                Transform realTransform = maskTransform.transform.Find("Image");
                Image skillImage = realTransform.GetComponent<Image>();
                skillImage.sprite = skillSprite;
                skillImage.gameObject.SetActive(true);
                
                Transform skillLevelTextTransform = skillSlot.transform.Find("SkillLevelText");
                TextMeshProUGUI skillLevelText = skillLevelTextTransform.GetComponent<TextMeshProUGUI>();
                skillLevelText.text = "Level " + Managers.Data.SkillDic[skillId].SkillId % 10;
                skillLevelText.gameObject.SetActive(true);
            }
        }
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