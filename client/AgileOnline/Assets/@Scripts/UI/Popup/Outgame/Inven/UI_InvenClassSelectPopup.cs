using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Util;

[Serializable]
public class ClassSelectDataReq
{
    public int classNo;
}

[Serializable]
public class ClassSelectDataRes
{
    public int status;
    public string message;
    public ClassSelectRes data;
}

[Serializable]
public class ClassSelectRes
{
    public bool result;
}

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

    int TempClassId;

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

        OnClickClassSelectButton(_templateData.SelectedClassId);

        int ClassCount = Managers.Data.PlayerDic.Count;
        for (int i = 0; i < ClassCount; i++)
        {
            int currentClassId = i + 1;
            GameObject ClassObject = Managers.Resource.Instantiate("UI_ClassObject", GetObject((int)EGameObjects.ClassListObject).transform);
            ClassObject.name = $"ClassObject{currentClassId}";
            Sprite spr = Managers.Resource.Load<Sprite>(Managers.Data.PlayerDic[currentClassId].ThumbnailName);
            Util.FindChild<Image>(ClassObject, "ClassImage").sprite = spr;
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
        Managers.Sound.PlayButtonClick();
        Debug.Log("CloseClassSelect");
        Managers.UI.ClosePopupUI(this);
    }

    void OnClickSelectButton()
    {
        Managers.Sound.PlayButtonClick();
        ClassSelect();
    }

    void OnClickClassSelectButton(int num)
    {
        Managers.Sound.PlayButtonClick();
        Debug.Log(num);
        TempClassId = num;
        GetText((int)ETexts.ClassNameText).text = Managers.Data.PlayerDic[num].Name;
        GetText((int)ETexts.ClassDescriptionText).text = Managers.Data.PlayerDic[num].Description;
    }

    void ClassSelect()
    {
        ClassSelectDataReq classSelectDataReq = new()
        {
            classNo = TempClassId,
        };

        string classJsonData = JsonUtility.ToJson(classSelectDataReq);

        StartCoroutine(JWTPostRequest("classes", classJsonData, res =>
        {
            ClassSelectDataRes classSelectDataRes = JsonUtility.FromJson<ClassSelectDataRes>(res);

            if (classSelectDataRes.status == 200)
            {
                _templateData.SelectedClassId = TempClassId;
                Debug.Log($"ClassChangedto{_templateData.SelectedClassId}");
                Managers.UI.ClosePopupUI(this);
            }
        }));
    }
}
