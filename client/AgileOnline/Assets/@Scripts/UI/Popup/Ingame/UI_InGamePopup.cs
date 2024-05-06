using System.Collections;
using TMPro;
using UnityEngine;
using static Define;

public class UI_InGamePopup : UI_Popup
{
    enum GameObjects
    {
        TimerText,
        RemainGold
    }

    enum Buttons
    {
        BackButton
    }

    private TextMeshProUGUI timerText;
    private float remainingTime = 60f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(GameObjects));
        BindButton(typeof(Buttons));
        GetButton((int)Buttons.BackButton).gameObject.BindEvent(OnBackButtonClick);

        timerText = GetText((int)GameObjects.TimerText).GetComponent<TextMeshProUGUI>();
        StartCoroutine(UpdateTimer());

        PlayerController pc = Managers.Object.Player;
        pc.UpdateRemainGoldText();

        return true;
    }

    public void UpdateRemainGoldText(int gold)
    {
        TextMeshProUGUI component = GetText((int)GameObjects.RemainGold).GetComponent<TextMeshProUGUI>();
        component.text = gold.ToString();
    }

    void OnBackButtonClick()
    {
        PlayerController player = Managers.Object.Player;
        if (player != null)
        {
            player.gameObject.SetActive(false);
            Managers.Object.Player = null;
        }
        StopAllCoroutines();
        // 리소스 정리
        CleanupResources();
        
        Managers.Scene.LoadScene(EScene.LobbyScene);
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