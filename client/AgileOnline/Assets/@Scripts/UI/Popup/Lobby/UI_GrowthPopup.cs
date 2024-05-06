using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GrowthPopup : UI_Popup
{
    #region Enum

    enum EGameObjects
    {
        //todo(박설연) : 이걸 오브젝트가 아니라 이미지로 빼서 일주일 단위 스트릭 로직을 추가해야 해여
        StreakGraphContent,
    }

    enum EButtons
    {
       IdleSettingButton,
       StartIdleButton,
    }

    enum ETexts
    {
        GrowthTitle,
        TotalGrowthContent,
        TodayGrowthContent,
        OneWeekStreakDays,
        StreakBonusText,
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

        GetButton((int)EButtons.IdleSettingButton).gameObject.BindEvent(OnClickIdleSettingButton);
        GetButton((int)EButtons.StartIdleButton).gameObject.BindEvent(OnClickStartIdleButton);

        #endregion

        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();

        return true;
    }

    // 갱신
    void Refresh()
    {

    }

    void OnClickIdleSettingButton()
    {
        Debug.Log("잠금 앱 설정 버튼 Clicked");
    }

    void OnClickStartIdleButton()
    {
        Debug.Log("성장하러 가기(방치) 버튼 Clicked");
        Managers.Scene.LoadScene(Define.EScene.IdleScene);
    }
    
}