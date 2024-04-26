using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_NodeMapScene : UI_Scene
{
    enum GameObjects
    {
        Line1To2,
        Line2To3,
        Line3To4,
        Line4To5,
        BossNodeDepth5,
        NormalNodeDepth4,
        EliteNodeDepth3,
        NormalNodeDepth2,
        NormalNodeDepth1
    };

    enum Texts
    {
        StageNo
    }

    enum Buttons
    {
        BackButton
    }

    enum Toggles { }

    enum Images
    {
        OriginImage
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        // Stage 정보를 받아와서 노드맵에 반영한다
        // 반영 정보 : 배경화면, 스테이지 번호, 
        
        //
        // GetObject((int)GameObjects.StartButton).BindEvent(() =>
        // {
        //     Debug.Log("StartButton clicked!");
        //     Managers.Scene.LoadScene(EScene.GameScene);
        // });
        //
        // GetObject((int)GameObjects.StartButton).gameObject.SetActive(true);
        //
        // Debug.Log("UI_LobbyScene initialized.");

        return true;
    }
}
