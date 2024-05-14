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
        BossSlider
    }

    enum Buttons
    {
        SettingButton
    }

    private TextMeshProUGUI timerText;
    private float remainingTime = 30f;


    private void OnEnable()
    {
        // 인게임 사운드 넣기
        Managers.Sound.Play(Define.ESound.Bgm,"Bgm_InGame");
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(GameObjects));
        BindButton(typeof(Buttons));
        Bind<Slider>(typeof(GameObjects));
        Slider bossSlider = Get<Slider>((int)GameObjects.BossSlider);
        bossSlider.gameObject.SetActive(false);
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(ShowSettingPopup);

        timerText = GetText((int)GameObjects.TimerText).GetComponent<TextMeshProUGUI>();
        StartCoroutine(UpdateTimer());

        PlayerController pc = Managers.Object.Player;
        pc.UpdateRemainGoldText();

        return true;
    }
    
    public void CheckBossNode()
    {
        bool isBossNode = GameScene.IsBossNode; 

        Slider bossSlider = Get<Slider>((int)GameObjects.BossSlider);

        if (bossSlider != null)
        {
            bossSlider.gameObject.SetActive(isBossNode);
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
        while (remainingTime > 1f)
        {
            timerText.text = Mathf.FloorToInt(remainingTime).ToString();
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        timerText.text = "0";
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