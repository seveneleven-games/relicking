using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public GameObject SplashScreen;
    public GameObject mainMenuScreen;
    public GameObject SettingsScreen;
    public GameObject LeaderBoardScreen;
    public GameObject LevelScreen;
    public GameObject ShopScreen;
    public GameObject StatsScreen;
    public GameObject SucessDialog;
    public GameObject GameplayScreen;

    GameObject currentScreen;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Show_mainMenuScreen();

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentScreen.SetActive(false);
            currentScreen.SetActive(true);
        }
    }
    private void DisableAll()
    {
        SplashScreen.SetActive(false);
        mainMenuScreen.SetActive(false);
        SettingsScreen.SetActive(false);
        LeaderBoardScreen.SetActive(false);
        LevelScreen.SetActive(false);
        ShopScreen.SetActive(false);
        StatsScreen.SetActive(false);
        SucessDialog.SetActive(false);
        GameplayScreen.SetActive(false);
    }

    public void Show_SplashScreen() {
        DisableAll();
        SplashScreen.SetActive(true);
        currentScreen = SplashScreen;
    }
    public void Show_mainMenuScreen()
    {
        DisableAll();
        mainMenuScreen.SetActive(true);
        currentScreen = mainMenuScreen;
    }
    public void Show_SettingsScreen()
    {
        DisableAll();
        SettingsScreen.SetActive(true);
        currentScreen = SettingsScreen;
    }
    public void Show_LeaderBoardScreen()
    {
        DisableAll();
        LeaderBoardScreen.SetActive(true);
        currentScreen = LeaderBoardScreen;
    }
    public void Show_LevelScreen()
    {
        DisableAll();
        LevelScreen.SetActive(true);
        currentScreen = LevelScreen;

    }
    public void Show_ShopScreen()
    {
        DisableAll();
        ShopScreen.SetActive(true);
        currentScreen = ShopScreen;
    }
    public void Show_StatsScreen()
    {
        DisableAll();
        StatsScreen.SetActive(true);
        currentScreen = StatsScreen;
    }
    public void Show_SucessDialog()
    {
        DisableAll();
        SucessDialog.SetActive(true);
        currentScreen = SucessDialog;
    }
    public void Show_GameplayScreen()
    {
        DisableAll();
        GameplayScreen.SetActive(true);
        currentScreen = GameplayScreen;

    }



   
}
