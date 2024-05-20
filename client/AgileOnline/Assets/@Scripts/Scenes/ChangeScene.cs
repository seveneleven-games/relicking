using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : UI_Popup
{
    public float transitionTime = 1f;

    #region Enum
    enum EGameObjects
    {
        DownScene,
    }

    enum EImages
    {
        
    }
    #endregion
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Object Bind
        
        BindObject(typeof(EGameObjects));
        
        GetObject((int)EGameObjects.DownScene).SetActive(false);
        
        #endregion
        
        return true;
    }
    
    // 씬 전환 함수
    public void LoadScene(Define.EScene eScene)
    {
        StartCoroutine(Transition(eScene));
    }

    IEnumerator Transition(Define.EScene eScene)
    {
        
        GetObject((int)EGameObjects.DownScene).SetActive(true);
        // 애니메이션 재생 시간만큼 대기합니다.
        yield return new WaitForSeconds(transitionTime);

        // 씬 로드
        Managers.Scene.LoadScene(eScene);
    }
}
