using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_ButtonAnimation : UI_Base
{

    void Start()
    {
        gameObject.BindEvent(ButtonPointerDownAnimation, type: Define.EUIEvent.PointerDown);
        gameObject.BindEvent(ButtonPointerUpAnimation, type: Define.EUIEvent.PointerUp);
    }

    public void ButtonPointerDownAnimation()
    {
        // 애니메이션 관련
    }

    public void ButtonPointerUpAnimation()
    {
        // 애니메이션 관련
    }
}