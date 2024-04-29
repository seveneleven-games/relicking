using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RankingPopup : UI_Popup
{
    #region Enum

    enum EGameObjects
    {
        ContentObject,
        RankingList
    }

    enum EButtons
    {
        StageSelectButton,
        MyRankingButton,
    }

    enum ETexts
    {
        StageText,
        MyRank,
        MyNickName,
        MyClass,
        MyDifficulty,
    }

    #endregion

    public void OnDestroy()
    {
        if (Managers.Game != null)
            Managers.Game.OnResourcesChanged -= Refresh;
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        #region Object Bind

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));

        GetButton((int)EButtons.StageSelectButton).gameObject.BindEvent(OnClickStageSelectButton);
        GetButton((int)EButtons.MyRankingButton).gameObject.BindEvent(OnClickRankingDetailButton);

        #endregion

        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();

        return true;
    }

    void Refresh()
    {

    }

    void OnClickStageSelectButton()
    {
        Debug.Log("StageSelect");
    }

    void OnClickRankingDetailButton()
    {
        Debug.Log("RankingDetail");
        Managers.UI.ShowPopupUI<UI_RankingDetailPopup>();
    }
}
