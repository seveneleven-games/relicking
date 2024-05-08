using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LobbyScene : UI_Scene
{
    #region Enum

    enum EGameObjects
    {
        // 네브바 쪽
        MenuToggleGroup,
        CheckInventoryBgImage,
        CheckInventoryImage,
        CheckGachaBgImage,
        CheckGachaImage,
        CheckBattleBgImage,
        CheckBattleImage,
        CheckGrowthBgImage,
        CheckGrowthImage,
        CheckRankingBgImage,
        CheckRankingImage,
        
    }
    
    enum EButtons
    {
        SettingButton
    }
    
    enum ETexts
    {
        InventoryToggleText,
        CheckInventoryToggleText, // 솔직히 색만 바뀌는데 필요하려나?
        GachaToggleText,
        CheckGachaToggleText,
        BattleToggleText,
        CheckBattleToggleText,
        GrowthToggleText,
        CheckGrowthToggleText,
        RankingToggleText,
        CheckRankingToggleText,
        TicketValueText
    }
    
    enum EToggles
    {
        InventoryToggle,
        GachaToggle,
        BattleToggle,
        GrowthToggle,
        RankingToggle,
        
    }
    
    enum EImages
    {
        Backgroundimage,
    }
    
    #endregion
    
    // 객체 관련 두는 곳
    bool _isSelectedInventory = false;
    bool _isSelectedGacha = false;
    bool _isSelectedBattle = false;
    bool _isSelectedGrowth = false;
    bool _isSelectedRanking = false;

    
    UI_BattlePopup _battlePopupUI;
    UI_GachaPopup _gachaPopupUI;
    UI_InvenPopup _invenPopupUI;
    UI_RankingPopup _rankingPopupUI;
    UI_GrowthPopup _growthPopupUI;
    
    
    public void OnDestroy()
    {
        StopAllCoroutines();
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
        
        // 토글 클릭 시 행동 (이쪽이 네비게이션 바 부분)
        
        GetToggle((int)EToggles.InventoryToggle).gameObject.BindEvent(OnClickInventoryToggle);
        GetToggle((int)EToggles.GachaToggle).gameObject.BindEvent(OnClickGachaToggle);
        GetToggle((int)EToggles.BattleToggle).gameObject.BindEvent(OnClickBattleToggle);
        GetToggle((int)EToggles.GrowthToggle).gameObject.BindEvent(OnClickGrowthToggle);
        GetToggle((int)EToggles.RankingToggle).gameObject.BindEvent(OnClickRankingToggle);
        
        _battlePopupUI = Managers.UI.ShowPopupUI<UI_BattlePopup>();
        _gachaPopupUI = Managers.UI.ShowPopupUI<UI_GachaPopup>();
        _invenPopupUI = Managers.UI.ShowPopupUI<UI_InvenPopup>();
        _rankingPopupUI = Managers.UI.ShowPopupUI<UI_RankingPopup>();
        _growthPopupUI = Managers.UI.ShowPopupUI<UI_GrowthPopup>();
        
        TogglesInit();


        if (Managers.Game.showIdleRewardPopup)
        {
            GetToggle((int)EToggles.GrowthToggle).gameObject.GetComponent<Toggle>().isOn = true;
            OnClickGrowthToggle();
            Managers.Game.showIdleRewardPopup = false;
        }
        else
        {
            // 맨 처음은 배틀로
            GetToggle((int)EToggles.BattleToggle).gameObject.GetComponent<Toggle>().isOn = true;
            OnClickBattleToggle();
        }
        
        
        // 가챠
        //GetToggle((int)EToggles.GachaToggle).gameObject.GetComponent<Toggle>().isOn = true;
        //OnClickGachaToggle();
        
        // 설정 버튼
        GetButton((int)EButtons.SettingButton).gameObject.BindEvent(ShowSettingPopup);

        #endregion

        Managers.Game.OnResourcesChanged += Refresh;
        Refresh();
        
        return true;
    }
    
    void ShowSettingPopup()
    {
        Managers.UI.ShowPopupUI<UI_OutgameSettingPopup>();
    }
    

    // 갱신
    void Refresh()
    {
        // GetText((int)ETexts.TicketValueText).text = Managers.Game.Ticket.ToString();
        
        // 토글 선택 시 리프레시 버그 대응
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetObject((int)EGameObjects.MenuToggleGroup).GetComponent<RectTransform>());
    }
    
    // 토글 초기화
    void TogglesInit()
    {
        // 팝업 초기화
        _battlePopupUI.gameObject.SetActive(false);
        _gachaPopupUI.gameObject.SetActive(false);
        _invenPopupUI.gameObject.SetActive(false);
        _rankingPopupUI.gameObject.SetActive(false);
        _growthPopupUI.gameObject.SetActive(false);
        
        // 선택여부 초기화
        _isSelectedInventory = false;
        _isSelectedGacha = false;
        _isSelectedBattle = false;
        _isSelectedGrowth = false;
        _isSelectedRanking = false;
        
        // 팝업 버튼 초기화 ( check안한 상태로 )
        GetObject((int)EGameObjects.CheckInventoryBgImage).SetActive(false);
        GetObject((int)EGameObjects.CheckInventoryImage).SetActive(false);
        GetObject((int)EGameObjects.CheckGachaBgImage).SetActive(false);
        GetObject((int)EGameObjects.CheckGachaImage).SetActive(false);
        GetObject((int)EGameObjects.CheckBattleBgImage).SetActive(false);
        GetObject((int)EGameObjects.CheckBattleImage).SetActive(false);
        GetObject((int)EGameObjects.CheckGrowthBgImage).SetActive(false);
        GetObject((int)EGameObjects.CheckGrowthImage).SetActive(false);
        GetObject((int)EGameObjects.CheckRankingBgImage).SetActive(false);
        GetObject((int)EGameObjects.CheckRankingImage).SetActive(false);
        
        // 팝업 버튼 사이즈 및 위치 초기화 (check 안한 상태로) -> 필요 없어짐 (툴에서 따로 처리)
        
        // 팝업버튼 밑의 글씨 비활성화
        GetText((int)ETexts.InventoryToggleText).gameObject.SetActive(false);
        GetText((int)ETexts.GachaToggleText).gameObject.SetActive(false);
        GetText((int)ETexts.BattleToggleText).gameObject.SetActive(false);
        GetText((int)ETexts.GrowthToggleText).gameObject.SetActive(false);
        GetText((int)ETexts.RankingToggleText).gameObject.SetActive(false);
    }
    
    
    void ShowUI(GameObject contentPopup, Toggle toggle, TMP_Text text, GameObject obj1, GameObject obj2, float duration = 0.1f)
    {
        TogglesInit();
        
        contentPopup.SetActive(true);
        // toggle.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 150); // 차후 수정 필요!!
        text.gameObject.SetActive(true);
        obj1.SetActive(true);
        obj2.SetActive(true);
        
        Refresh();
    }
    
    void OnClickInventoryToggle()
    {
        GetImage((int)EImages.Backgroundimage).color = Util.HexToColor("525DAD"); // 배경 색상 변경 (변화 필요)
        if (_isSelectedInventory == true) // 활성화 후 토글 클릭 방지
            return;
        
        // ShowUI
        ShowUI(_invenPopupUI.gameObject,
            GetToggle((int)EToggles.InventoryToggle),
            GetText((int)ETexts.InventoryToggleText),
            GetObject((int)EGameObjects.CheckInventoryBgImage),
            GetObject((int)EGameObjects.CheckInventoryImage));
        _isSelectedInventory = true;
    }
    void OnClickGachaToggle()
    {
        GetImage((int)EImages.Backgroundimage).color = Util.HexToColor("525DAD"); // 배경 색상 변경
        if (_isSelectedGacha == true) // 활성화 후 토글 클릭 방지
            return;
        ShowUI(_gachaPopupUI.gameObject, 
            GetToggle((int)EToggles.GachaToggle), 
            GetText((int)ETexts.CheckGachaToggleText), 
            GetObject((int)EGameObjects.CheckGachaBgImage),
            GetObject((int)EGameObjects.CheckGachaImage));
        _isSelectedGacha = true;
    }
    void OnClickBattleToggle()
    {
        GetImage((int)EImages.Backgroundimage).color = Util.HexToColor("1F5FA0"); // 배경 색상 변경
        if (_isSelectedBattle == true) // 활성화 후 토글 클릭 방지
            return;
        
        ShowUI(_battlePopupUI.gameObject, 
            GetToggle((int)EToggles.BattleToggle), 
            GetText((int)ETexts.CheckBattleToggleText),
            GetObject((int)EGameObjects.CheckBattleBgImage), 
            GetObject((int)EGameObjects.CheckBattleImage));
        _isSelectedBattle = true;
    }
    void OnClickGrowthToggle()
    {
        GetImage((int)EImages.Backgroundimage).color = Util.HexToColor("C48152"); // 배경 색상 변경 
        if (_isSelectedGrowth == true) // 활성화 후 토글 클릭 방지
            return;
        
        // ShowUI
        ShowUI(_growthPopupUI.gameObject,
            GetToggle((int)EToggles.GrowthToggle),
            GetText((int)ETexts.CheckGrowthToggleText),
            GetObject((int)EGameObjects.CheckGrowthBgImage),
            GetObject((int)EGameObjects.CheckGrowthImage));
        _isSelectedGrowth = true;
    }
    void OnClickRankingToggle()
    {
        GetImage((int)EImages.Backgroundimage).color = Util.HexToColor("525DAD"); // 배경 색상 변경 (변화필요)
        if (_isSelectedRanking == true) // 활성화 후 토글 클릭 방지
            return;
        
        // ShowUI
        ShowUI(_rankingPopupUI.gameObject,
            GetToggle((int)EToggles.RankingToggle),
            GetText((int)ETexts.CheckRankingToggleText),
            GetObject((int)EGameObjects.CheckRankingBgImage),
            GetObject((int)EGameObjects.CheckRankingImage));
        _isSelectedRanking = true;
    }
    
    
    
}
