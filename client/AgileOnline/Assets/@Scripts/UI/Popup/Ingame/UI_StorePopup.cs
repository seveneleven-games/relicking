using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UI_StorePopup : UI_Popup
{
    private PlayerController _player = Managers.Object.Player;

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
        Gold = _player.PlayerGold;
        RerollCost = Define.INITIAL_REROLL_COST;
        
        // 버튼 함수 바인딩
        GetButton((int)Buttons.RerollButton).gameObject.BindEvent(Reroll);
        GetButton((int)Buttons.SettingButton).gameObject.BindEvent(ShowSettingPopup);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(Exit);

        // 현재 플레이어의 스킬 정보 가져오기
        DataSync(_player.PlayerSkillList);
        
        // 슬롯 개수 적용 : 일단 보류
        
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

    public event Action<int> OnSkillCardClick;

    int[] GetRandomSkillIdList(int length)
    {
        return _isSkillPoolFixed ? 
            Extension.RandomSkillList(length, _skillList, _maxSkillTypes) : 
            Extension.RandomIntList(length, 0, Define.TOTAL_PLAYER_SKILL_NUMBER);
    }
    
    void BuySkill(SkillCard skill)
    {
        //todo(전지환) : 스킬 구매 정보 반영 로직 필요
        /*
         * 1. 스킬 카드 리롤 (이건 스킬카드 클래스에서)
         *  : 단 현재 상품 탭에 떠있는 스킬이 떠서는 안 됨 [O]
         *  : 스킬을 3개 미만으로 받아와야 하는 상황에는 스킬 카드를 디폴트로 랜더링 할 수 있도록 함
         *      (그리고 클릭 이벤트는 모두 비활성화)
         */
        Debug.Log($"구매 스킬 : {Managers.Data.SkillDic[skill.SkillId].Name} {Managers.Data.SkillDic[skill.SkillId].SkillId % 10}Lv, " +
                  $"구매 클릭 슬롯 번호 : {_skillCards.IndexOf(skill) + 1}번");

        if (Gold < Define.TEST_SKILL_COST)
        {
            Debug.Log("소유한 골드가 적어 구매할 수 없어요!");
            return;
        }

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
        
        //todo(전지환) : 최대 레벨이면 스킬풀에서 제거하는 로직 필요!
        //분기처리. 만약 스킬 타입이 -1(더 이상 반환할 수 있는 스킬이 없음)이라면..?
        if (fixedSkillType == -1)
        {
            skill.RefreshNull();
        }
        else
        {
            int nowLevel = GetNowLevel(fixedSkillType);
            // nowLevel 만렙이면 우짤래 
            skill.Refresh(fixedSkillType*10 + nowLevel + 1);
        }
        
        //step2. 스킬 레벨 받아오기 -> 함수화 (여러번 씀)
        

        //todo(전지환) : 스킬 데이터에 맞는 코스트로 빼주기
        _player.PlayerGold -= Define.TEST_SKILL_COST;
        Gold -= Define.TEST_SKILL_COST;
        
        
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
        //todo(전지환) : 돈이 모자라면 안 돌아가게 만들어야 할 것.
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
                Debug.Log($"최고랩 달성! 달성 스킬 : {Managers.Data.SkillDic[skillId].Name}");
                _maxSkillTypes.Add(skillId / 10);
            }
                
        }
        
    }
}

    
