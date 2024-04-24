using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

public class SceneManagerEx
{
    public BaseScene CurrentScene
    {
        get { return GameObject.FindObjectOfType<BaseScene>(); }
    }

    public void LoadScene(EScene type)
    {
        //Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    private string GetSceneName(EScene type)
    {
        string name = System.Enum.GetName(typeof(EScene), type);
        return name;
    }

    public void Clear()
    {
        //CurrentScene.Clear();
    }
}