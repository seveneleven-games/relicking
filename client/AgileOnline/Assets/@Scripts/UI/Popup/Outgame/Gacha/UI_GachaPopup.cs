using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;


#region 가챠 뽑기 관련

[Serializable]
public class GachaDataReq
{
    public int gachaNum;
}

[Serializable]
public class GachaDataRes
{
    public int status;
    public string message;
    public List<GachaRelic> data;
}

[Serializable]
public class GachaRelic
{
    public int relicNo;
    public int level;
    public bool levelUpYn;
    public bool newYn;
}

#endregion


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
        // ContentObjet,
    }

    enum EButtons
    {
        DetailInfoButton,
        GachaOneButton,
        GachaTenButton,
        UpButton,
        DownButton,
        NoUpButton,
        NoDownButton,
        
    }

    enum ETexts
    {
        GachaTitle,
        Tickets,
        GachaOneText,
        GachaTenText,
        TicketOneNumber,
        TicketTenNumber
    }

    #endregion

    // 객체 관련 두는 곳
    private int _gachaFold;
    
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

        _gachaFold = 1;
        
        GetButton((int)EButtons.UpButton).gameObject.BindEvent(OnClickUpButton);
        GetButton((int)EButtons.DownButton).gameObject.BindEvent(OnClickDownButton);
        
        // 맨 처음에 백 통신을 통해 티켓정보 가져오기 
        GetTicket();
        
        // 티켓 변화가 있으면 갱신시켜라! (사실 여기서만 변화가 있기 때문에 이 방식 안 써도 되긴 함.)
        Managers.Game.OnResourcesChanged += Refresh;
        
        Refresh();

        return true;
    }

    

    void OnClickUpButton()
    {
        Managers.Sound.PlayButtonClick();
        _gachaFold = 2;
        GachaInfoRefresh();
    }

    void OnClickDownButton()
    {
        Managers.Sound.PlayButtonClick();
        _gachaFold = 1;
        GachaInfoRefresh();
    }
    
    void GetTicket()
    {
        // 티켓 조회 테스트
        StartCoroutine(JWTGetRequest("gacha", res =>
        {
            // json -> 객체로 변환
            TicketDataRes ticketDataRes = JsonUtility.FromJson<TicketDataRes>(res);
            Managers.Game.Ticket = ticketDataRes.data.gacha; 
        }));
    }
    
    // 갱신
    void Refresh()
    {

        TicketInfoRefresh();
        GachaInfoRefresh();

    }


    void TicketInfoRefresh()
    {
        // 티켓 UI정보 갱신
        GetText((int)ETexts.Tickets).text = $"{Managers.Game.Ticket}";
    }
    
    void GachaInfoRefresh()
    {
        if (_gachaFold == 1)
        {
            GetButton((int)EButtons.UpButton).gameObject.SetActive(true);
            GetButton((int)EButtons.NoUpButton).gameObject.SetActive(false);
            GetButton((int)EButtons.NoDownButton).gameObject.SetActive(true);
            GetButton((int)EButtons.DownButton).gameObject.SetActive(false);
            GetText((int)ETexts.GachaOneText).text = "1회 뽑기";
            GetText((int)ETexts.GachaTenText).text = "10회 뽑기";
            GetText((int)ETexts.TicketOneNumber).text = "-1";
            GetText((int)ETexts.TicketTenNumber).text = "-10";
           
        }

        else if (_gachaFold == 2)
        {
            GetButton((int)EButtons.UpButton).gameObject.SetActive(false);
            GetButton((int)EButtons.NoUpButton).gameObject.SetActive(true);
            GetButton((int)EButtons.NoDownButton).gameObject.SetActive(false);
            GetButton((int)EButtons.DownButton).gameObject.SetActive(true);
            GetText((int)ETexts.GachaOneText).text = "50회 뽑기";
            GetText((int)ETexts.GachaTenText).text = "100회 뽑기";
            GetText((int)ETexts.TicketOneNumber).text = "-50";
            GetText((int)ETexts.TicketTenNumber).text = "-100";
        }
    }
    
    void OnClickDetailInfoButton()
    {
        Managers.Sound.PlayButtonClick();
        Debug.Log("InfoButtonClicked");
        Managers.UI.ShowPopupUI<UI_GachaInfoPopup>();
    }

    void OnClickGachaOneButton()
    {
        Managers.Sound.PlayButtonClick();
        Debug.Log("GachaOne");

        if (_gachaFold == 1)
        {
            SendGachaRequest(1);
        }
        else if (_gachaFold == 2)
        {
            SendGachaRequest(50);
        }
    }

    void OnClickGachaTenButton()
    {
        Managers.Sound.PlayButtonClick();
        Debug.Log("GachaTen");
        if (_gachaFold == 1)
        {
            SendGachaRequest(10);
        }
        else if (_gachaFold == 2)
        {
            SendGachaRequest(100);
        }
    }

    // 가쟈 요청하기
    void SendGachaRequest(int gachaNum)
    {
        // 만약 티켓이 부족하다면?
        if (Managers.Game.Ticket - gachaNum < 0)
        {
            // 팝업에서 Toast 방식으로 변환
            Managers.UI.ShowToast("티켓이 부족합니다.");
            // Managers.UI.ShowPopupUI<UI_GachaNoTicketPopup>();
        }
        else
        {
            // 가챠 요청 객체 만들기
            GachaDataReq gachaDataReq = new GachaDataReq
            {
                gachaNum = gachaNum
            };
        
            // 객체 -> Json 변환
            string gachaJsonData = JsonUtility.ToJson(gachaDataReq);

            StartCoroutine(JWTPostRequest("gacha", gachaJsonData, res =>
            {
                // json -> 객체로 변환
                GachaDataRes gachaDataRes = JsonUtility.FromJson<GachaDataRes>(res);

                
                if (gachaDataRes.status == 200)
                {
                    Transform UI_root = gameObject.transform.parent; // @UI_Root
                    FindChild(UI_root.gameObject, "UI_LobbyScene").SetActive(false);
                    FindChild(UI_root.gameObject, "UI_GachaPopup").SetActive(false);
                    ShowGachaResultPopup(gachaDataRes.data);
                    
                    // 성공했으면 빼주기 (자동 갱신을 위하여)
                    Managers.Game.Ticket -= gachaNum;
                }
                
            }));
        }
    }

    // 가챠 결과 창 보여주기
    void ShowGachaResultPopup(List<GachaRelic> gachaRelics)
    {
        UI_GachaResultPopup popup = Managers.UI.ShowPopupUI<UI_GachaResultPopup>();
        
        // 띄운 팝업 함수 이용. (가챠된 유물 정보 팝업 창으로 보내주기)
        popup.SetRelicsData(gachaRelics);
        
    }
    
    
}
