using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UI_StorePopup : UI_Popup
{
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

    // 스킬 카드 관련 변수
    private GameObject _skillCardParent;
    private SkillCard _skillCard1;
    private SkillCard _skillCard2;
    private SkillCard _skillCard3;
    
    private List<SkillCard> _skillCards = new();
    
    

    // 스킬 관련 변수
    private int[] _skillTypes = new int[3];
    private List<int> _skillList; // GameScene에서 연결해줌 변경될 때 마다 싱크 맞춰줌
    private bool _isSkillPoolFixed = false;
    
    private HashSet<int> _maxSkillTypes = new(); 
    
    // 골드 관련 변수
    private int _gold;
    private int _rerollCost;

    public int Gold
    {
        get { return _gold;}
        set
        {
            _gold = value;
            GetText((int)Texts.RemainGold).text = _gold.ToString();
        }
    }

    public int RerollCost
    {
        get { return _rerollCost; }
        set
        {
            _rerollCost = value;
            GetText((int)Texts.RerollCost).text = _rerollCost.ToString();
        }
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        // 초기 비용 설정
        _player = Managers.Object.Player;
        Gold = _player.PlayerGold;
        RerollCost = Define.INITIAL_REROLL_COST;
        
        // 버튼 함수 바인딩
        GetButton((int)Buttons.RerollButton).gameObject.BindEvent(Reroll);
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(ShowSettingPopup);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(Exit);

        // 현재 플레이어의 스킬 정보 가져오기
        DataSync(_player.PlayerSkillList);
        
        _skillCardParent = GetObject((int)GameObjects.StoreSkillCardList);
        _skillCardParent.gameObject.DestroyChilds();
        
        _skillCard1 = Managers.Resource.Instantiate("SkillCard", _skillCardParent.transform).GetComponent<SkillCard>();
        _skillCard2 = Managers.Resource.Instantiate("SkillCard", _skillCardParent.transform).GetComponent<SkillCard>();
        _skillCard3 = Managers.Resource.Instantiate("SkillCard", _skillCardParent.transform).GetComponent<SkillCard>();
        _skillCards.Add(_skillCard1);
        _skillCards.Add(_skillCard2);
        _skillCards.Add(_skillCard3);
        
        _skillCard1.gameObject.BindEvent(()=>BuySkill(_skillCard1));
        _skillCard2.gameObject.BindEvent(()=>BuySkill(_skillCard2));
        _skillCard3.gameObject.BindEvent(()=>BuySkill(_skillCard3));
        
        for (int i = 0; i < 3; i++) _skillCards[i].Init();
        
        Reroll();
        
        return true;
    }
    
    void ShowSettingPopup()
    {
        Managers.UI.ShowPopupUI<UI_IngameSettingPopup>();
    }

    public event Action<int> OnSkillCardClick;

    int[] GetRandomSkillIdList(int length)
    {
        return _isSkillPoolFixed ? 
            Extension.RandomSkillList(length, _skillList, _maxSkillTypes) : 
            Extension.RandomIntList(length, 0, Define.TOTAL_PLAYER_SKILL_NUMBER, _maxSkillTypes);
    }
    
    void BuySkill(SkillCard skill)
    {
        if (skill.SkillId == -1)
        {
            Debug.Log("빈 슬롯! 구매 처리 미진행");
            return;
        }
        
        Debug.Log($"구매 스킬 : {Managers.Data.SkillDic[skill.SkillId].Name} {Managers.Data.SkillDic[skill.SkillId].SkillId % 10}Lv, " +
                  $"구매 클릭 슬롯 번호 : {_skillCards.IndexOf(skill) + 1}번");

        if (Gold < Define.TEST_SKILL_COST)
        {
            Debug.Log("소유한 골드가 적어 구매할 수 없어요!");
            return;
        }

        //GameScene에서 구독한 BuySkill 함수 실행 & 싱크 작업
        DataSync(_player.AddSkill(skill.SkillId));
        
        int fixedSkillType = GetFixedSkillType(_skillCards.IndexOf(skill),GetRandomSkillIdList(3));
        int nowLevel = GetNowLevel(fixedSkillType);
        
        if (fixedSkillType == -1)
            skill.RefreshNull();
        else
            skill.Refresh(fixedSkillType*10 + nowLevel + 1);
        

        //todo(전지환) : 스킬 데이터에 맞는 코스트로 빼주기
        _player.PlayerGold -= Define.TEST_SKILL_COST;
        Gold -= Define.TEST_SKILL_COST;
    }

    int GetFixedSkillType(int selectedCardIdx, int[] candidates)
    {
        int fixedSkillType = -1;
        
        foreach (int skillType in candidates)
        {
            bool isDuplicate = false;
            
            for (int i = 0; i < candidates.Length; i++)
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

        return fixedSkillType;
    }
    

    int GetNowLevel(int skillType)
    {
        int nowLevel = 0;
        
        foreach (int skillId in _skillList)
        {
            if (skillId != 0 && skillId / 10 == skillType)
            {
                nowLevel = skillId % 10;
                break;
            }
        }

        return nowLevel;
    }
    
    
    void Reroll()
    {
        if (Gold < RerollCost)
        {
            Debug.Log("소유한 골드가 적어 리롤 칠 수가 없어요!");
            return;
        }

        _skillTypes = GetRandomSkillIdList(3);
        
        // 스킬 정보에 따라서 더해주는 값을 바꾸어 줘야 할 것.
        for (int i = 0; i < 3; i++)
        {
            if (_skillTypes[i] == -1)
                _skillCards[i].RefreshNull();
            else
                _skillCards[i].SkillId = _skillTypes[i] * 10 + GetNowLevel(_skillTypes[i]) + 1;

        }

        Gold -= RerollCost;
        RerollCost += 5;
    }

    

    void Exit()
    {
        ClosePopupUI();
    }

    public void DataSync(List<int> skillList)
    {
        _skillList = skillList;
        int count = 0;
        
        // 빈 스킬 슬롯 확인
        foreach (int skillId in _skillList)
        {
            if (skillId != 0) count++;
        }
        
        // skillPool 관리
        if (!_isSkillPoolFixed && count >= 6)
        {
            _isSkillPoolFixed = true;
            Debug.Log($"여섯개 꽉 참!! 굿 ㅋㅋ {_isSkillPoolFixed} <== 이거보셈 ㅋㅋㅋ");
        }
        
        // max level 스킬 관리
        foreach (int skillId in _skillList)
        {
            if (skillId != 0 && Managers.Data.SkillDic[skillId].NextId == -1)
            {
                Debug.Log($"만렙. {Managers.Data.SkillDic[skillId].Name}");
                _maxSkillTypes.Add(skillId / 10);
            }
                
        }
        
    }
}

    
