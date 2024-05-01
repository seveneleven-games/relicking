using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class SkillCard : UI_Base
{
    enum Images
    {
        SkillImage,
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
        
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        OnSkillIdChanged += Refresh;
        
        return true;
    }

    public void Refresh(int skillId)
    {
        SkillData data = Managers.Data.SkillDic[skillId];
        
        GetText((int)Texts.SkillName).text = data.Name;
        GetText((int)Texts.NowLv).text = (skillId % 10 - 1).ToString();
        GetText((int)Texts.NextLv).text = (skillId % 10).ToString();
        GetText((int)Texts.SkillInfo).text = data.Description;
        //todo(전지환) : 스킬 코스트를 각 스킬 별로 설정해주어야 할 것.
        GetText((int)Texts.SkillCost).text = Define.TEST_SKILL_COST.ToString();
        GetImage((int)Images.SkillImage).sprite = Managers.Resource.Load<Sprite>(data.IconName);
    }
}
