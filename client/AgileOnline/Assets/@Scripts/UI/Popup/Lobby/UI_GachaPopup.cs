using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;


[Serializable]
public class TicketDataRes
{
    public int status;
    public string message;
    public TicketRes data;
}

[Serializable]
public class TicketRes
{
    public int gacha;
}


public class UI_GachaPopup : UI_Popup
{
    #region Enum

    enum EGameObjects
    {
        ContentObjet,
    }

    enum EButtons
    {
        DetailInfoButton,
        GachaOneButton,
        GachaTenButton,
    }

    enum ETexts
    {
        GachaTitle,
        Tickets,
    }

    #endregion

    // 객체 관련 두는 곳
    private int _ticketNum;
    
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

        GetButton((int)EButtons.DetailInfoButton).gameObject.BindEvent(OnClickDetailInfoButton);
        GetButton((int)EButtons.GachaOneButton).gameObject.BindEvent(OnClickGachaOneButton);
        GetButton((int)EButtons.GachaTenButton).gameObject.BindEvent(OnClickGachaTenButton);

        #endregion
        
        // 맨 처음에 백 통신을 통해 티켓정보 가져오기 
        
        // 티켓 변화가 있으면 갱신
        Managers.Game.OnResourcesChanged += Refresh;
        
        Refresh();

        return true;
    }

    // 갱신
    void Refresh()
    {
        // 티켓 정보 가져오기
        _ticketNum = Managers.Game.Ticket;

        TicketInfoRefresh();

    }


    void TicketInfoRefresh()
    {
        // 티켓 UI정보 갱신
        GetText((int)ETexts.Tickets).text = $"{_ticketNum}";
    }
    
    void OnClickDetailInfoButton()
    {
        Debug.Log("InfoButtonClicked");
        Managers.UI.ShowPopupUI<UI_GachaInfoPopup>();
    }

    void OnClickGachaOneButton()
    {
        Debug.Log("GachaOne");
        
        // 백엔드 통신이 정상적으로 완료되었을 시 티켓 타감하기
        
        // 티켓 조회 테스트
        JWTGetRequest("gacha", res =>
        {
            // json -> 객체로 변환
            TicketDataRes ticketDataRes = JsonUtility.FromJson<TicketDataRes>(res);
            _ticketNum = ticketDataRes.data.gacha;
        });

    }

    void OnClickGachaTenButton()
    {
        Debug.Log("GachaTen");
        //Managers.UI.ShowPopupUI<UI_GachaNoTicketPopup>();
    }
}
