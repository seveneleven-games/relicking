using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class ShurikenController : SkillController
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
    public int Damage { get; private set; }
    public float LifeTime { get; private set; } = 10;
    public float Speed { get; private set; }
    public int ProjectileNum { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        SkillType = Define.ESkillType.Shuriken;

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
        
        StartDestroy(LifeTime);
    }
    
    public override void UpdateController()
    {
        base.UpdateController();

        transform.position += _moveDir * Speed * Time.deltaTime;
    }
    
    public void SetOwner(CreatureController owner)
    {
        _owner = owner;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        _moveDir = direction.normalized;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.IsValid() == false)
            return;

        MonsterController monster = collision.gameObject.GetComponent<MonsterController>();

        if (monster.IsValid() == false)
            return;
        
        int damage = Damage;
        monster.OnDamaged(_owner, ref damage);
        
        Managers.Object.Despawn(this);
    }
}
