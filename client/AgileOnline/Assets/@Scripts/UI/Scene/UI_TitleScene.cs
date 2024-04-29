using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_TitleScene : UI_Scene
{
    enum EGameObjects
    {
        StartImage
    }
    
    enum ETexts
    {
        DisplayText
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(EGameObjects));
        BindText(typeof(ETexts));

        // 아무곳이나 누르면 씬 변환하는 객체 생성
        GetObject((int)EGameObjects.StartImage).BindEvent(() =>
        {
            Debug.Log("ChangeScene");
            
            //todo(전지환) : 노드맵 테스트, 머지 전에 EScene.LobbyScene으로 원복하기
            Managers.Scene.LoadScene(EScene.LobbyScene);
        });
        
        GetObject((int)EGameObjects.StartImage).gameObject.SetActive(false);
        GetText((int)ETexts.DisplayText).text = $"";

        StartLoadAssets();
        
        return true;
    }
    
    void StartLoadAssets()
    {
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
        {
            Debug.Log($"{key} {count}/{totalCount}");

            if (count == totalCount)
            {
                Managers.Data.Init();
                
                GetObject((int)EGameObjects.StartImage).gameObject.SetActive(true);
                GetText((int)ETexts.DisplayText).text = "Touch To Start";
            }
        });
    }
}
