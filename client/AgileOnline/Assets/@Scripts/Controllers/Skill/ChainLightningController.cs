using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class ChainLightningController : SkillController
{
    private CreatureController _owner;
    private Vector3 _moveDir;

    private Vector3 startPoint;
    private Vector3 endPoint;
    
    public int SkillId { get; private set; }
    public int NextId { get; private set; }
    public string PrefabName { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string IconName { get; private set; }
    public float CoolTime { get; private set; }
    public int Damage { get; private set; }
    public float LifeTime { get; private set; } = 10;
    public float Speed { get; private set; }
    public int ProjectileNum { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        SkillType = Define.ESkillType.ChainLightning;

        return true;
    }
    
    public void InitSkill(int templateId, Vector3 start, Vector3 end)
    {
        SkillData data = Managers.Data.SkillDic[templateId];
        
        SkillId = data.SkillId;
        NextId = data.NextId;
        PrefabName = data.PrefabName;
        Name = data.Name;
        Description = data.Description;
        IconName = data.IconName;
        CoolTime = data.CoolTime;
        Damage = data.Damage;
        LifeTime = data.LifeTime;
        Speed = data.Speed;
        ProjectileNum = data.ProjectileNum;
        
        Vector3 direction = end - start;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.localScale = new Vector3(direction.magnitude, 1f, 1f);
        
        StartDestroy(LifeTime);
    }
}
