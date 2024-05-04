using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class SkillCard : UI_Base
{
    enum GameObjects
    {
        SkillLv,
        SkillIconMask,
        SkillCostSymbol
    }
    
    enum Images
    {
        SkillImage
    }

    enum Texts
    {
        SkillName,
        NowLv,
        NextLv,
        SkillInfo,
        SkillCost
    }

    public event Action<int> OnSkillIdChanged;
    private int _skillId;
    
    public int SkillId
    {
        get { return _skillId; }
        set
        {
            OnSkillIdChanged?.Invoke(value);
            _skillId = value;
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        OnSkillIdChanged += Refresh;
        
        return true;
    }

    public void Refresh(int skillId)
    {
        _skillId = skillId;
        
        SkillData data = Managers.Data.SkillDic[skillId];
        
        GetText((int)Texts.SkillName).gameObject.SetActive(true);
        GetText((int)Texts.SkillCost).gameObject.SetActive(false);
        GetObject((int)GameObjects.SkillLv).gameObject.SetActive(true);
        GetObject((int)GameObjects.SkillIconMask).gameObject.SetActive(true);
        GetObject((int)GameObjects.SkillCostSymbol).gameObject.SetActive(true);
        
        GetText((int)Texts.SkillInfo).color = new Color(50/255f, 50/255f, 50/255f, 1.0f);
        
        GetText((int)Texts.SkillName).text = data.Name;
        GetText((int)Texts.NowLv).text = (skillId % 10 - 1).ToString();
        GetText((int)Texts.NextLv).text = (skillId % 10).ToString();
        GetText((int)Texts.SkillInfo).text = data.Description;
        GetImage((int)Images.SkillImage).sprite = Managers.Resource.Load<Sprite>(data.IconName);

        //todo(전지환) : 스킬 코스트를 각 스킬 별로 설정해주어야 할 것.
        GetText((int)Texts.SkillCost).text = Define.TEST_SKILL_COST.ToString();
    }

    public void RefreshNull()
    {
        _skillId = -1;
        
        GetText((int)Texts.SkillName).gameObject.SetActive(false);
        GetText((int)Texts.SkillCost).gameObject.SetActive(false);
        GetObject((int)GameObjects.SkillLv).gameObject.SetActive(false);
        GetObject((int)GameObjects.SkillIconMask).gameObject.SetActive(false);
        GetObject((int)GameObjects.SkillCostSymbol).gameObject.SetActive(false);
        GetText((int)Texts.SkillInfo).text = "SOLD OUT!";
        GetText((int)Texts.SkillInfo).color = new Color(0.7f, 0.1f, 0.0f, 1.0f);
        
        //todo(전지환) : 스킬 코스트를 각 스킬 별로 설정해주어야 할 것.
    }
}
