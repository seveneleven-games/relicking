using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BattlePopup : UI_Popup
{
    #region Enum

    enum EGameObjects
    {
        ContentObject,
        StageSelectScrollView,
        StageScrollContentObject,
    }
    
    enum EButtons
    {
        
    }
    
    enum ETexts
    {
        StageNameText
    }
    
    enum EToggles
    {
        
    }
    
    enum EImages
    {
        
    }
    
    #endregion
    
    // 객체 관련 두는 곳

    
    
    public void OnDestroy()
    {
        if(Managers.Game != null)
            Managers.Game.OnResourcesChanged -= Refresh;
    }
    
    // 초기 세팅
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Object Bind

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));
        BindToggle(typeof(EToggles));
        BindImage(typeof(EImages)); 
        
        // 임시
        GetObject((int)EGameObjects.StageSelectScrollView).BindEvent(() =>
        {
            Debug.Log("go Game");
            Managers.Scene.LoadScene(Define.EScene.GameScene);
        });
        
        #endregion
        
        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();
        
        return true;
    }

    // 갱신
    void Refresh()
    {
        
    }
    
    
    
}
