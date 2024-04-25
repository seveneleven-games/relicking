using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBoltController : SkillController
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
    public float LifeTime { get; private set; }
    public float Speed { get; private set; }

    public override bool Init()
    {
        base.Init();
        ObjectType = Define.EObjectType.EnergyBolt;
        
        StartDestroy(LifeTime);

        return true;
    }

    public void InitSkill(Data.SkillData data)
    {
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
    }

    public override void UpdateController()
    {
        base.UpdateController();

        transform.position += _moveDir * Speed * Time.deltaTime;
    }
    
    public void SetMoveDirection(Vector3 direction)
    {
        _moveDir = direction.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MonsterController monster = collision.gameObject.GetComponent<MonsterController>();

        if (monster.IsValid() == false)
            return;
        if (this.IsValid() == false)
            return;
        
        monster.OnDamaged(_owner, Damage);
        
        StopDestroy();
        
        Managers.Object.Despawn(this);
    }
}
