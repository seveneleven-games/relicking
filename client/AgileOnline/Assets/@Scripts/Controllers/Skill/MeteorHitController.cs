using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class MeteorHitController : SkillController
{
    private CreatureController _owner;
    private Vector3 _moveDir;
    
    public int SkillId { get; private set; }
    public int NextId { get; private set; }
    public string PrefabName { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string IconName { get; private set; }
    public float CoolTime { get; private set; }
    public float Damage { get; private set; }
    public float LifeTime { get; private set; } = 10;
    public float Speed { get; private set; }
    public int ProjectileNum { get; private set; }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        SkillType = Define.ESkillType.MeteorHit;

        return true;
    }

    public void InitSkill(int templateId)
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

        ApplyDamageToMonstersInRange();
        
        StartDestroy(1f);
    }
    
    public void SetOwner(CreatureController owner)
    {
        _owner = owner;
    }
    
    private void ApplyDamageToMonstersInRange()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2f);

        foreach (Collider2D collider in colliders)
        {
            MonsterController monster = collider.GetComponent<MonsterController>();
            if (monster != null)
            {
                PlayerController pc = _owner as PlayerController;
                float realDamage = (Damage * pc.Atk);
                monster.OnDamaged(_owner, ref realDamage);
            }
        }
    }
}
