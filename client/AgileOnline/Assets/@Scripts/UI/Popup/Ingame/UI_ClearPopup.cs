using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_ClearPopup : UI_Popup
{
    private TemplateData _templateData;
    private PlayerController _player;
    
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
        StageInfo,
        DifficultyInfo
    }

    enum Buttons
    { }

    enum Toggles
    { }

    enum Images
    {
        BG,
        ClassImage
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.Sound.Play(ESound.Effect,"GameClear");
        
        _templateData = Resources.Load<TemplateData>("GameTemplateData");
        Debug.Log("받아오니?" + _templateData);
        _player = Managers.Object.Player;
        
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindToggle(typeof(Toggles));
        BindImage(typeof(Images));
        
        GetImage((int)Images.BG).gameObject.BindEvent(ClosePopupUI);

        GetText((int)Texts.StageInfo).text = _templateData.StageId.ToString();
        GetText((int)Texts.DifficultyInfo).text = _templateData.Difficulty.ToString();
        
        
        //todo(전지환) : 클래스 이미지 스프라이트 추가되면 이미지 바꿔줄 것.
        GetImage((int)Images.ClassImage).sprite = Managers.Resource.Load<Sprite>(
            Managers.Data.PlayerDic[_templateData.SelectedClassId].ThumbnailName);

        for (int i = 0; i < 6; i++)
        {
            Debug.Log((_templateData));
            Debug.Log(_templateData.EquipedRelicIds);
            int relicId = _templateData.EquipedRelicIds[i];

            if (relicId == 0) continue;
            
            Image relicImage = Util.FindChild<Image>(GetObject(i), "RelicImage");
            GameObject levelTextParent = Util.FindChild(GetObject(i), "RelicLevelText");

            relicImage.gameObject.SetActive(true);
            relicImage.sprite = Managers.Resource.Load<Sprite>(
                Managers.Data.RelicDic[relicId].ThumbnailName);
            
            levelTextParent.SetActive(true);
            Util.FindChild<TMP_Text>(levelTextParent, "LevelText").text = (relicId % 10).ToString();
        }

        for (int i = 6; i < 12; i++)
        {
            int skillId = _player.PlayerSkillList[i - 6];

            if (skillId == 0) continue;

            Image skillImage = Util.FindChild<Image>(GetObject(i), "Image", true);
            GameObject levelTextParent = Util.FindChild(GetObject(i), "SkillLevelText");
            
            skillImage.sprite = Managers.Resource.Load<Sprite>(
                Managers.Data.SkillDic[skillId].IconName);
            skillImage.color = new Color(1, 1, 1, 1);
            
            levelTextParent.SetActive(true);
            Util.FindChild<TMP_Text>(levelTextParent, "LevelText").text = (skillId % 10).ToString();

        }
        
        return true;
    }

    public override void ClosePopupUI()
    {
        Managers.Sound.PlayButtonClick();
        ExitGame();
        base.ClosePopupUI();
    }
    
    void ExitGame()
    {
        PlayerController player = Managers.Object.Player;
        if (player != null)
        {
            player.gameObject.SetActive(false);
            Managers.Object.Player = null;
        }
        StopAllCoroutines();
        Managers.Object.CleanupResources();
        Managers.Scene.LoadScene(EScene.LobbyScene);
    }
}
