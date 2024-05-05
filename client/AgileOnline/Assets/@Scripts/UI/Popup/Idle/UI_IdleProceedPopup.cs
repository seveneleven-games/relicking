using System.Collections;
using TMPro;
using UnityEngine;
using static Define;

public class UI_IdleProceedPopup : UI_Popup
    {
        #region Enum
        
            enum EGameObjects
            {
                //todo(박설연) : 이걸 오브젝트가 아니라 이미지로 빼서 일주일 단위 스트릭 로직을 추가해야 해여
                StreakGraphContent,
            }
        
            enum EButtons
            {
                ExitIdleButton,
            }
        
            enum ETexts
            {
                TotalGrowthContent,
            }
        
            #endregion
        
            public void OnDestroy()
            {
                if (Managers.Game != null)
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
        
                GetButton((int)EButtons.ExitIdleButton).gameObject.BindEvent(OnClickExitIdleButton);
        
                #endregion
        
                Managers.Game.OnResourcesChanged += Refresh;
                Refresh();
        
                return true;
            }
        
            // 갱신
            void Refresh()
            {
        
            }
        
            void OnClickExitIdleButton()
            {
                Debug.Log("종료하기 Clicked");
                Managers.Scene.LoadScene(EScene.LobbyScene);
            }
            
    }
