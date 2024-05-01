using System.Collections;
using TMPro;
using UnityEngine;
using static Define;

public class UI_InGamePopup : UI_Popup
{
    enum GameObjects
    {
        TimerText
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

        return true;
    }

    void OnBackButtonClick()
    {
        Managers.Game.InitializeGameData();
        Managers.Scene.LoadScene(Define.EScene.LobbyScene);
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