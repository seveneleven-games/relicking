using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UI_StorePopup : UI_Popup
{
    public TemplateData _templateData;
    private PlayerController _player;

    enum GameObjects
    {
        StoreSkillCardList
    };

    enum Texts
    {
        RemainGold,
        RerollCost
    }

    enum Buttons
    {
        SettingButton,
        ExitButton,
        RerollButton
    }

    private GameObject _skillCardList;
    private SkillCard _skillCard1;
    private SkillCard _skillCard2;
    private SkillCard _skillCard3;
    
    private int[] _skillTypes = new int[3];
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        // 초기 비용 설정
        GetText((int)Texts.RemainGold).text = Define.INITIAL_GOLD.ToString();
        GetText((int)Texts.RerollCost).text = Define.INITIAL_REROLL_COST.ToString();
        
        // 버튼 함수 바인딩
        GetButton((int)Buttons.RerollButton).gameObject.BindEvent(Reroll);
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(ShowSettingPopup);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(Exit);


        _skillCardList = GetObject((int)GameObjects.StoreSkillCardList);
        _skillCardList.gameObject.DestroyChilds();
        _skillTypes = Extension.RandomIntList(0, Define.TOTAL_PLAYER_SKILL_NUMBER, 3);
        
        _skillCard1 = Managers.Resource.Instantiate("SkillCard", _skillCardList.transform).GetComponent<SkillCard>();
        _skillCard2 = Managers.Resource.Instantiate("SkillCard", _skillCardList.transform).GetComponent<SkillCard>();
        _skillCard3 = Managers.Resource.Instantiate("SkillCard", _skillCardList.transform).GetComponent<SkillCard>();

        _skillCard1.Init();
        _skillCard1.SkillId = _skillTypes[0] * 10 + 1;
        _skillCard1.gameObject.BindEvent(()=>BuySkill(_skillCard1.SkillId));
        
        _skillCard2.Init();
        _skillCard2.SkillId = _skillTypes[1] * 10 + 1;
        _skillCard2.gameObject.BindEvent(()=>BuySkill(_skillCard2.SkillId));

        _skillCard3.Init();
        _skillCard3.SkillId = _skillTypes[2] * 10 + 1;
        _skillCard3.gameObject.BindEvent(()=>BuySkill(_skillCard3.SkillId));
        
        return true;
    }

    public event Action<int> OnSkillCardClick;
    
    void BuySkill(int skillId)
    {
        //todo(전지환) : 스킬 구매 정보 반영 로직 필요
        /*
         * 1. 스킬 카드 리롤
         * 2. 소지금액에서 가격을 제함 (소지금액이 더 작으면 구매 불가)
         * 3. 스킬 반영
         * 4. 반영된 스킬 정보 가져올 것
         */
        _player = Managers.Object.Player;
        _player.UpdateRemainGoldText();
        Debug.Log(skillId);
        OnSkillCardClick?.Invoke(skillId);
    }

    void Reroll()
    {
        //todo(전지환) : 돈이 모자라면 안 돌아가게 만들어야 할 것.
        _skillTypes = Extension.RandomIntList(0, Define.TOTAL_PLAYER_SKILL_NUMBER, 3);
        
        // 스킬 정보에 따라서 더해주는 값을 바꾸어 줘야 할 것.
        _skillCard1.SkillId = _skillTypes[0] * 10 + Define.MY_SKILL_LEVEL + 1;
        _skillCard2.SkillId = _skillTypes[1] * 10 + Define.MY_SKILL_LEVEL + 1;
        _skillCard3.SkillId = _skillTypes[2] * 10 + Define.MY_SKILL_LEVEL + 1;
        
    }

    void ShowSettingPopup()
    {
        //todo(전지환) 셋팅 모달 컴포넌트 나오면 show popup 해야할듯
        // Managers.UI.ShowPopupUI<셋팅모달컴포넌트>();
    }

    void Exit()
    {
        //todo(전지환) : 스킬 구매 데이터 반영 여부에 따라 여기서 관리하게 될 수도 있음
        
        ClosePopupUI();
    }
    
}

    
