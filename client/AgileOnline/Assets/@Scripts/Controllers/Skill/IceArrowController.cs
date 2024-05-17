using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class IceArrowController : SkillController
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
        
        SkillType = Define.ESkillType.IceArrow;

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
    
    public void SetMoveDirection(Vector3 direction)
    {
        _moveDir = direction.normalized;
        float angle = Mathf.Atan2(_moveDir.y, _moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }
    
    public void SetOwner(CreatureController owner)
    {
        _owner = owner;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.IsValid() == false)
            return;

        MonsterController monster = collision.gameObject.GetComponent<MonsterController>();

        if (monster.IsValid() == false)
            return;
        
        PlayerController pc = _owner as PlayerController;
        float realDamage = (Damage * pc.Atk);
        monster.OnDamaged(_owner, ref realDamage);
        
        Managers.Object.Despawn(this);
    }
}
