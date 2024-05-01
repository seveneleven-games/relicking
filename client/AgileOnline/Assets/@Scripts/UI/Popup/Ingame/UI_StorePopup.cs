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
    private List<SkillCard> _skillCards = new();
    
    private int[] _skillTypes = new int[3];
    private List<int> _skillList;
    private bool _isSkillPoolFixed = false;
    
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
        _skillTypes = Extension.RandomIntList(3, 0, Define.TOTAL_PLAYER_SKILL_NUMBER);
        
        _skillCard1 = Managers.Resource.Instantiate("SkillCard", _skillCardList.transform).GetComponent<SkillCard>();
        _skillCard2 = Managers.Resource.Instantiate("SkillCard", _skillCardList.transform).GetComponent<SkillCard>();
        _skillCard3 = Managers.Resource.Instantiate("SkillCard", _skillCardList.transform).GetComponent<SkillCard>();
        _skillCards.Add(_skillCard1);
        _skillCards.Add(_skillCard2);
        _skillCards.Add(_skillCard3);

        for (int i = 0; i < 3; i++)
        {
            _skillCards[i].Init();
            _skillCards[i].SkillId = _skillTypes[i] * 10 + 1;
        }
        
        _skillCard1.gameObject.BindEvent(()=>BuySkill(_skillCard1));
        _skillCard2.gameObject.BindEvent(()=>BuySkill(_skillCard2));
        _skillCard3.gameObject.BindEvent(()=>BuySkill(_skillCard3));
        
        return true;
    }

    public event Action<int> OnSkillCardClick;

    int[] GetRandomSkillIdList(int length)
    {
        return _isSkillPoolFixed ? 
            Extension.RandomSkillList(length, _skillList) : 
            Extension.RandomIntList(length, 0, Define.TOTAL_PLAYER_SKILL_NUMBER);
    }
    
    void BuySkill(SkillCard skill)
    {
        //todo(전지환) : 스킬 구매 정보 반영 로직 필요
        /*
         * 1. 스킬 카드 리롤 (이건 스킬카드 클래스에서)
         *  : 단 현재 상품 탭에 떠있는 스킬이 떠서는 안 됨
         * 2. 소지금액에서 가격을 제함 (소지금액이 더 작으면 구매 불가)
         * 3. 스킬 반영
         * 4. 반영된 스킬 정보 가져올 것
         */
        Debug.Log($"{skill.SkillId}, {_skillCards.IndexOf(skill)}");

        int selectedCardIdx = _skillCards.IndexOf(skill);
        
        //GameScene에서 구독한 BuySkill 함수 실행
        OnSkillCardClick?.Invoke(skill.SkillId);
        
        //스킬 업데이트
        //step1. 스킬 타입 정하기
        int fixedSkillType = 0;
        int[] candidates = GetRandomSkillIdList(3);
        foreach (int skillType in candidates)
        {
            bool isDuplicate = false;
            
            for (int i = 0; i < 3; i++)
            {
                if(i == selectedCardIdx) continue;

                if (_skillCards[i].SkillId / 10 == skillType)
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (isDuplicate) continue;
            else
            {
                fixedSkillType = skillType;
                break;
            };
        }
        
        //step2. 스킬 레벨 받아오기
        int nowLevel = 0;
        foreach (int skillId in _skillList)
        {
            if (skillId != 0 && skillId / 10 == fixedSkillType)
            {
                nowLevel = skillId % 10;
                break;
            }
        }
        
        //todo(전지환) : 지금은 스킬 데이터 문제로 현재 레벨 반환, +1 필요
        skill.Refresh(fixedSkillType*10 + nowLevel);
        _player = Managers.Object.Player;
        _player.UpdateRemainGoldText();
        Debug.Log(skillId);
        OnSkillCardClick?.Invoke(skillId);
    }

    
    void Reroll()
    {
        //todo(전지환) : 돈이 모자라면 안 돌아가게 만들어야 할 것.

        _skillTypes = GetRandomSkillIdList(3);
        
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

    public void DataSync(List<int> skillList)
    {
        _skillList = skillList;
        
        // skillPool 관리
        if (!_isSkillPoolFixed && skillList.Count >= 6)
        {
            _isSkillPoolFixed = true;
        }
        
    }
}

    
