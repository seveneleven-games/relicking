using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Util;
using static Extension;

[Serializable]
public class InventoryDataRes
{
    public int status;
    public string message;
    public InventoryData data;
}

[Serializable]
public class InventoryRelicDataRes
{
    public int relicNo;
    public int level;
    public int exp;
    public int slot;
}

[Serializable]
public class InventoryData
{
    public int currentClassNo;
    public List<InventoryRelicDataRes> myRelicList;
}

public class UI_InvenPopup : UI_Popup
{
    #region Enum
    
    enum EGameObjects
    {
        ContentObject,
        ToggleGroup,
        StatToggleObject,
        RelicToggleObject,
        RelicListObject,
    }
    
    enum EButtons
    {
        ClassButton,
        EquipedRelicButton1,
        EquipedRelicButton2,
        EquipedRelicButton3,
        EquipedRelicButton4,
        EquipedRelicButton5,
        EquipedRelicButton6,
    }
    
    enum ETexts
    {
        MaxHealthText,
        DamageText,
        SpeedText,
        CoinBonusText,
        CriticalRateText,
        CriticalDamageText,
        CoolDownText,
    }
    
    enum EToggles
    {
        RelicToggle,
        StatToggle,
    }
    
    enum EImages
    {
        RelicToggleBGImage,
        StatToggleBGImage,
        EquipedRelicImage1,
        EquipedRelicImage2,
        EquipedRelicImage3,
        EquipedRelicImage4,
        EquipedRelicImage5,
        EquipedRelicImage6,
        ClassImage,
    }
    
    #endregion
    
    private TemplateData _templateData;
    
    public void OnDestroy()
    {
        // if (Managers.Game != null)
            // Managers.Game.OnResourcesChanged -= Refresh;

        if (_templateData != null)
        {
            _templateData.OnPlayerStatusChagned -= SetClassDetailStatus;
            _templateData.OnEquipedRelicIdsChanged -= SetEquipedRelicImages;
            _templateData.OnEquipedRelicIdsChanged -= SetEquiped;
        }
        
    }
    
    // 초기 세팅
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
    
        _templateData = Resources.Load<TemplateData>("GameTemplateData");
        
        #region Object Bind
    
        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));
        BindToggle(typeof(EToggles));
        BindImage(typeof(EImages));
    
        //토글 상호작용 바인딩
        GetToggle((int)EToggles.RelicToggle).gameObject.BindEvent(OnClickRelicToggle);
        GetToggle((int)EToggles.StatToggle).gameObject.BindEvent(OnClickStatToggle);
    
        //장착 정보창 관련 상호작용 바인딩
        GetButton((int)EButtons.ClassButton).gameObject.BindEvent(OnClickClassSelectButton);
        GetButton((int)EButtons.EquipedRelicButton1).gameObject.BindEvent(() => OnClickEquipedRelicButton(0));
        GetButton((int)EButtons.EquipedRelicButton2).gameObject.BindEvent(() => OnClickEquipedRelicButton(1));
        GetButton((int)EButtons.EquipedRelicButton3).gameObject.BindEvent(() => OnClickEquipedRelicButton(2));
        GetButton((int)EButtons.EquipedRelicButton4).gameObject.BindEvent(() => OnClickEquipedRelicButton(3));
        GetButton((int)EButtons.EquipedRelicButton5).gameObject.BindEvent(() => OnClickEquipedRelicButton(4));
        GetButton((int)EButtons.EquipedRelicButton6).gameObject.BindEvent(() => OnClickEquipedRelicButton(5));
        
        #endregion
        
        OnClickRelicToggle();
        GetRelicInfo();
        
    
        // 티켓 변화가 있을 때 마다 리프레시 해주어야 하는 이유?
        //  -> 가챠 후 보유 장비 정보가 바뀌기 때문
        //  하지만, 로직이 바뀔 예정
        // Managers.Game.OnResourcesChanged += Refresh;
        
        // 플레이어 스탯 변화 이벤트
        _templateData.OnPlayerStatusChagned += SetClassDetailStatus;
        
        // 클래스 변화 이벤트
        _templateData.OnSelectedClassIdChanged += SetClassImage;
        
        // 장착 장비 변화 이벤트
        _templateData.OnEquipedRelicIdsChanged += SetEquipedRelicImages;
        _templateData.OnEquipedRelicIdsChanged += SetEquiped;
    
        return true;
    }
    
    // 갱신
    // void Refresh()
    // {
    //     //todo(전지환) : 해당 함수가 필요한지 검토 후 삭제
    // }
    
    
    // 요주의 함수
    private void OnEnable()
    {
        if (!_init) return;
        
        GetRelicInfo();
    }
    
    bool _isSelectedEquip = false;
    bool _isSelectedStat = false;
    void ToggleInit()
    {
        // 선택여부 초기화
        _isSelectedEquip = false;
        _isSelectedStat = false;
    
        // 팝업버튼 초기화
        GetObject((int)EGameObjects.RelicToggleObject).SetActive(false);
        GetObject((int)EGameObjects.StatToggleObject).SetActive(false);
    
        GetImage((int)EImages.RelicToggleBGImage).color = Util.HexToColor("B9A691");
        GetImage((int)EImages.StatToggleBGImage).color = Util.HexToColor("B9A691");
    }
    
    void OnClickRelicToggle()
    {
        Managers.Sound.PlayButtonClick();
        ToggleInit();
        GetImage((int)EImages.RelicToggleBGImage).color = Util.HexToColor("B38C61");
        if (_isSelectedEquip == true)
            return;
    
        GetObject((int)EGameObjects.RelicToggleObject).SetActive(true);
        _isSelectedEquip = true;
    }
    
    void OnClickStatToggle()
    {
        Managers.Sound.PlayButtonClick();
        ToggleInit();
        GetImage((int)EImages.StatToggleBGImage).color = Util.HexToColor("B38C61");
        if (_isSelectedStat == true)
            return;
    
        GetObject((int)EGameObjects.StatToggleObject).SetActive(true);
        _isSelectedStat = true;
    }
    
    void OnClickClassSelectButton()
    {
        Managers.Sound.PlayButtonClick();
        Managers.UI.ShowPopupUI<UI_InvenClassSelectPopup>();
    }
    
    void OnClickRelicInfoButton(int num)
    {
        Managers.Sound.PlayButtonClick();
        _templateData.SelectedRelicId = num;
        Managers.UI.ShowPopupUI<UI_InvenRelicInfoPopup>();
    }
    
    void OnClickEquipedRelicButton(int num)
    {
        if (_templateData.EquipedRelicIds[num] == 0)
            return;
        Managers.Sound.PlayButtonClick();
        OnClickRelicInfoButton(_templateData.EquipedRelicIds[num]);
    }
    
    void SetClassDetailStatus(int num, int[] nums)
    {
        int MaxHp = Managers.Data.PlayerDic[num].MaxHp;
        int Atk = Managers.Data.PlayerDic[num].Atk;
        float Speed = Managers.Data.PlayerDic[num].Speed;
        float CoinBonus = Managers.Data.PlayerDic[num].ExtraGold;
        float CritRate = Managers.Data.PlayerDic[num].CritRate;
        float CritDmgRate = Managers.Data.PlayerDic[num].CritDmgRate;
        float CoolDown = Managers.Data.PlayerDic[num].CoolDown;
    
        foreach (int i in nums)
        {
            MaxHp += Managers.Data.RelicDic[i].MaxHp;
            Atk += Managers.Data.RelicDic[i].Atk;
            Speed += Managers.Data.RelicDic[i].Speed;
            CoinBonus += Managers.Data.RelicDic[i].ExtraGold;
            CritRate += Managers.Data.RelicDic[i].CritRate;
            CritDmgRate += Managers.Data.RelicDic[i].CritDmgRate;
            CoolDown -= Managers.Data.RelicDic[i].CoolTime / 100f;
        }

        Debug.Log($"첫 스탯 {CoinBonus}");
        Debug.Log($"첫 스탯 {CritRate}");
        Debug.Log($"첫 스탯 {CritDmgRate}");
        Debug.Log($"첫 스탯 {CoolDown}");

        if (CoolDown < 0.1)
            CoolDown = 0.1f;

        //표기 반올림 처리
        CoinBonus = RoundThird(CoinBonus);
        CritRate = RoundThird(CritRate);
        CritDmgRate = RoundThird(CritDmgRate);
        CoolDown = RoundThird(CoolDown);

        
        GetText((int)ETexts.MaxHealthText).text = $"{MaxHp}";
        GetText((int)ETexts.DamageText).text = $"{Atk}";
        GetText((int)ETexts.SpeedText).text = $"{Speed}";
        GetText((int)ETexts.CoinBonusText).text = $"{CoinBonus * 100}%";
        GetText((int)ETexts.CriticalRateText).text = $"{CritRate * 100}%";
        GetText((int)ETexts.CriticalDamageText).text = $"{CritDmgRate * 100}%";
        GetText((int)ETexts.CoolDownText).text = $"{CoolDown * 100}%";
        
        //todo(전지환) : 유물 스탯 인게임에서도 적용시켜야 함 -> playerController확인
        //todo(김형규) : 추가된 스탯 필드 설명창에 뜨도록 수정 필요
    }
    
    void SetClassImage(int num)
    {
        Image image = GetImage((int)EImages.ClassImage);
        if (image != null)
        {
            Sprite spr = Managers.Resource.Load<Sprite>(Managers.Data.PlayerDic[num].ThumbnailName);
            image.sprite = spr;
        }
    }
    
    void SetEquipedRelicImages(int[] nums)
    {
        for (int i = 0; i < nums.Length; i++)
        {

            Image image = GetImage(i + 2);
            Button button = GetButton(i + 1);
            button.GetComponent<Image>().sprite = nums[i] switch
            {
                < 1 => Managers.Resource.Load<Sprite>("RelicFrame_Default.sprite"),
                < 1000 => Managers.Resource.Load<Sprite>("RelicFrame_C.sprite"),
                < 2000 => Managers.Resource.Load<Sprite>("RelicFrame_B.sprite"),
                < 3000 => Managers.Resource.Load<Sprite>("RelicFrame_A.sprite"),
                < 4000 => Managers.Resource.Load<Sprite>("RelicFrame_S.sprite"),
                _ => Managers.Resource.Load<Sprite>("RelicFrame_SSS.sprite"),
            };
            if (image != null)
            {
                Color tempColor = image.color;
                if (nums[i] == 0)
                {
                    tempColor.a = 0f;
                   image.color = tempColor;
                }
                else
                {
                   tempColor.a = 1f;
                   image.color = tempColor;
                   string SprName = Managers.Data.RelicDic[nums[i]].ThumbnailName;
                   Sprite spr = Managers.Resource.Load<Sprite>(SprName);
                   image.sprite = spr;
                }
            }
        }
    }
    
    void SetEquiped(int[] nums)
    {
        GameObject relicListObject = GetObject((int)EGameObjects.RelicListObject);
        HashSet<string> equipedRelics = new();

        for (int i = 0; i < nums.Length; i++)
        {
            if (nums[i] == 0) continue;

            equipedRelics.Add($"RelicObject{nums[i]}");

        }

        for (int i = 0; i < relicListObject.transform.childCount; i++)
        {
            Transform child = relicListObject.transform.GetChild(i);
            if (equipedRelics.Contains(child.name))
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    
    private void GetRelicInfo()
    {
        StartCoroutine(JWTGetRequest("inventories", ProcessInventoryData));
    }
    
    private void ProcessInventoryData(string res)
    {
        InventoryDataRes inventoryDataRes = JsonUtility.FromJson<InventoryDataRes>(res);
        if (inventoryDataRes.status == 200)
        {
            _templateData.SelectedClassId = inventoryDataRes.data.currentClassNo;
            _templateData.OwnedRelics = inventoryDataRes.data.myRelicList;
            UpdateRelics();
        }
    }

    private void UpdateRelics()
    {
        SetClassDetailStatus(_templateData.SelectedClassId, _templateData.EquipedRelicIds);
        SetClassImage(_templateData.SelectedClassId);
        
        InstanciateInvenRelics();
    }

    private void InstanciateInvenRelics()
    {
        GameObject RelicListObject = GetObject((int)EGameObjects.RelicListObject);
        RelicListObject.DestroyChilds();

        foreach (var OwnedRelic in _templateData.OwnedRelics)
        {
            if (OwnedRelic.relicNo == 0)
                continue;

            int RelicId = OwnedRelic.relicNo * 10 + OwnedRelic.level;
            GameObject RelicObject = Managers.Resource.Instantiate("UI_RelicDetailObject", RelicListObject.transform);
            
            if(OwnedRelic.slot != 0)
                RelicObject.SetActive(false);
            
            RelicObject.name = $"RelicObject{RelicId}";
            RelicObject.GetComponent<Button>().onClick.AddListener(() => OnClickRelicInfoButton(RelicId));
            RelicObject.GetComponent<Image>().sprite = RelicId switch
            {
                < 1000 => Managers.Resource.Load<Sprite>("RelicFrame_C.sprite"),
                < 2000 => Managers.Resource.Load<Sprite>("RelicFrame_B.sprite"),
                < 3000 => Managers.Resource.Load<Sprite>("RelicFrame_A.sprite"),
                < 4000 => Managers.Resource.Load<Sprite>("RelicFrame_S.sprite"),
                _ => Managers.Resource.Load<Sprite>("RelicFrame_SSS.sprite"),
            };
            FindChild<Image>(RelicObject, "RelicImage").sprite = Managers.Resource.Load<Sprite>(
                Managers.Data.RelicDic[RelicId].ThumbnailName);
        }
        foreach (var OwnedRelic in _templateData.OwnedRelics)
        {
            if (OwnedRelic.slot != 0)
            {
                _templateData.SetRelicAt(OwnedRelic.slot - 1, OwnedRelic.relicNo * 10 + OwnedRelic.level);
            }
        }
        
    }
    
}
