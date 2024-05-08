using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InvenClassSelectPopup : UI_Popup
{
    #region Enum
    enum EGameObjects
    {
        ContentObjet,
        ClassListObject,
    }

    enum EButtons
    {
        ButtonBG,
        ButtonCancel,
        ButtonSelect,
    }

    enum ETexts
    {
        ClassNameText,
        ClassDescriptionText,
    }

    #endregion

    public TemplateData _templateData;

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

        GetButton((int)EButtons.ButtonBG).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)EButtons.ButtonCancel).gameObject.BindEvent(OnClickCloseButton);
        GetButton((int)EButtons.ButtonSelect).gameObject.BindEvent(OnClickSelectButton);

        _templateData = Resources.Load<TemplateData>("GameTemplateData");

        GetText((int)ETexts.ClassNameText).text = Managers.Data.PlayerDic[_templateData.SelectedClassId].Name;
        GetText((int)ETexts.ClassDescriptionText).text = $"{_templateData.SelectedClassId} description";


        int ClassCount = Managers.Data.PlayerDic.Count;
        for (int i = 0; i < ClassCount; i++)
        {
            int currentClassId = i + 1;
            GameObject ClassObject = Managers.Resource.Instantiate("UI_ClassObject", GetObject((int)EGameObjects.ClassListObject).transform);
            ClassObject.name = $"ClassObject{currentClassId}";
            ClassObject.BindEvent(() => OnClickClassSelectButton(currentClassId));
        }

        #endregion

        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();

        return true;
    }

    // 갱신
    void Refresh()
    {

    }

    void OnClickCloseButton()
    {
        Debug.Log("CloseClassSelect");
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickSelectButton()
    {
        Debug.Log($"ClassChangedto{_templateData.SelectedClassId}");
        OnClickCloseButton();
    }

    void OnClickClassSelectButton(int num)
    {
        Debug.Log(num);
        _templateData.SelectedClassId = num;
        GetText((int)ETexts.ClassNameText).text = Managers.Data.PlayerDic[num].Name;
        GetText((int)ETexts.ClassDescriptionText).text = $"{num} description";
    }
}
