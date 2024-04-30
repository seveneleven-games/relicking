using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UI_StorePopup : UI_Popup
{
    public TemplateData _templateData;

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
    private SkillCard[] _skillList = new SkillCard[3];
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
        
        for (int i = 0; i < 3; i++)
        {
            _skillList[i] = Managers.Resource.Instantiate("SkillCard", _skillCardList.transform).GetComponent<SkillCard>();
            _skillList[i].Init();
            _skillList[i].SkillId = _skillTypes[i] * 10 + 1;
            
            _skillList[i].gameObject.BindEvent(()=>BuySkill(_skillList[i].SkillId));
        }
        
        return true;
    }

    public event Action<int> OnSkillCardClick;
    
    void BuySkill(int skillId)
    {
        //todo(전지환) : 스킬 구매 정보 반영 로직 필요
        Debug.Log(skillId);
        OnSkillCardClick?.Invoke(skillId);
    }

    void Reroll()
    {
        //todo(전지환) : 돈이 모자라면 안 돌아가게 만들어야 할 것.
        _skillTypes = Extension.RandomIntList(0, Define.TOTAL_PLAYER_SKILL_NUMBER, 3);

        for (int i = 0; i < 3; i++)
        {
            // 구매 정보에 따라서 더해주는 값을 바꾸어 줘야 할 것.
            _skillList[i].SkillId = _skillTypes[i] * 10 + Define.MY_SKILL_LEVEL + 1;
        }
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

    
