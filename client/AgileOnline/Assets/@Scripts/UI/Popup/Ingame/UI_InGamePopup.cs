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
    private float remainingTime = 10f;

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
        Managers.Game.InitializeGameData();
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
    
}