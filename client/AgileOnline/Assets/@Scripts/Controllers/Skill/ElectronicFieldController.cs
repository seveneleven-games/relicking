using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using static Define;

public class ElectronicFieldController : SkillController
{
    private CreatureController _owner;
    private float _damageInterval = 0.5f;
    private float _lastDamageTime;
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
    
    public void SetOwner(CreatureController owner)
    {
        _owner = owner;
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        SkillType = ESkillType.ElectronicField;

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
    
    private void Update()
    {
        if (_owner != null)
        {
            transform.position = _owner.transform.position;
        }
        
        if (Time.time - _lastDamageTime >= _damageInterval)
        {
            _lastDamageTime = Time.time;
            DealDamageToNearbyMonsters();
        }
    }
    
    // private void OnTriggerStay2D(Collider2D collision)
    // {
    //     if (this.IsValid() == false)
    //         return;
    //
    //     MonsterController monster = collision.gameObject.GetComponent<MonsterController>();
    //
    //     if (monster.IsValid() == false)
    //         return;
    //
    //     monster.OnDamaged(_owner, Damage);
    // }
    
    private void DealDamageToNearbyMonsters()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f);
        foreach (Collider2D collider in colliders)
        {
            MonsterController monster = collider.GetComponent<MonsterController>();
            if (monster != null && monster.IsValid())
            {
                monster.OnDamaged(_owner, Damage);
            }
        }
    }
}
