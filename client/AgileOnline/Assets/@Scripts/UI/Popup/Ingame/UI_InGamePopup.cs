using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_InGamePopup : UI_Popup
{
    enum GameObjects
    {
        TimerText,
        RemainGold,
        BossSlider,
        BossName
    }

    enum Buttons
    {
        SettingButton
    }

    private TextMeshProUGUI _timerText;
    private TextMeshProUGUI _bossName;
    private float _remainingTime = 15f;
    private TemplateData _templateData;


    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        popupType = GameScene.IsBossNode ? PopupType.InGameBoss : PopupType.InGame;
        _templateData = Resources.Load<TemplateData>("GameTemplateData");
        
        BindText(typeof(GameObjects));
        BindButton(typeof(Buttons));
        Bind<Slider>(typeof(GameObjects));
        Slider bossSlider = Get<Slider>((int)GameObjects.BossSlider);
        bossSlider.gameObject.SetActive(false);
        CheckBossNode();
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(ShowSettingPopup);

        _timerText = GetText((int)GameObjects.TimerText).GetComponent<TextMeshProUGUI>();
        StartCoroutine(UpdateTimer());

        PlayerController pc = Managers.Object.Player;
        pc.UpdateRemainGoldText();
        
        return true;
    }

    public void CheckBossNode()
    {
        bool isBossNode = GameScene.IsBossNode;

        Slider bossSlider = Get<Slider>((int)GameObjects.BossSlider);
        
        bossSlider.gameObject.SetActive(isBossNode);
        if (isBossNode)
        {
            _bossName = GetText((int)GameObjects.BossName).GetComponent<TextMeshProUGUI>();
            switch (Managers.Data.StageDic[_templateData.StageId].Name)
            {
                case "고대 숲":
                    _bossName.text = "깊은 숲의 그롬쉬";
                    break;
                case "설원":
                    _bossName.text = "혹한의 크루스";
                    break;
                case "지하 세계":
                    _bossName.text = "용암 지대의 론트";
                    break;
            }
            _remainingTime = 60f;
            Managers.Sound.Play(Define.ESound.Bgm, "Bgm_InGameBoss");
        }
    }

    public void UpdateBossHealth(float currentHealth, float maxHealth)
    {
        Slider bossSlider = Get<Slider>((int)GameObjects.BossSlider);
        bossSlider.value = currentHealth / maxHealth;
    }

    public void UpdateRemainGoldText(int gold)
    {
        TextMeshProUGUI component = GetText((int)GameObjects.RemainGold).GetComponent<TextMeshProUGUI>();
        component.text = gold.ToString();
    }

    void ShowSettingPopup()
    {
        Time.timeScale = 0;
        Managers.UI.ShowPopupUI<UI_IngameSettingPopup>();
    }

    private IEnumerator UpdateTimer()
    {
        while (_remainingTime > 1f)
        {
            _timerText.text = Mathf.FloorToInt(_remainingTime).ToString();
            _remainingTime -= Time.deltaTime;
            yield return null;
        }

        _timerText.text = "0";
    }

    private void CleanupResources()
    {
        // 몬스터와 골드 오브젝트 despawn
        DespawnObjects<MonsterController>("@Monsters");
        DespawnObjects<GoldController>("@Golds");

        // 맵 오브젝트 파괴
        DestroyObjects("@BaseMap");

        // 오브젝트 풀 정리
        Managers.Pool.Clear();
    }

    private void DespawnObjects<T>(string parentName) where T : MonoBehaviour
    {
        GameObject parentObject = GameObject.Find(parentName);
        if (parentObject != null)
        {
            foreach (Transform child in parentObject.transform)
            {
                T component = child.gameObject.GetComponent<T>();
                if (component != null)
                {
                    BaseController baseController = component as BaseController;
                    if (baseController != null)
                        Managers.Object.Despawn(baseController);
                }
            }
        }
    }

    private void DestroyObjects(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            Managers.Resource.Destroy(obj);
        }
    }
}