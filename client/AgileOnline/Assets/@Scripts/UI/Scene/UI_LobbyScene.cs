using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_LobbyScene : UI_Scene
{
    enum GameObjects
    {
        StartButton
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));
        
        GetObject((int)GameObjects.StartButton).BindEvent(() =>
        {
            Debug.Log("StartButton clicked!");
            Managers.Scene.LoadScene(EScene.GameScene);
        });
        
        GetObject((int)GameObjects.StartButton).gameObject.SetActive(true);

        Debug.Log("UI_LobbyScene initialized.");

        return true;
    }
}