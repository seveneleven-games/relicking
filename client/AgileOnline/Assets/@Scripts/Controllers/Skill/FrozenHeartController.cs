using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class FrozenHeartController : SkillController
{
    public CreatureController _owner;
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
    public float Radius { get; private set; }
    public float RotationSpeed { get; private set; }
    public int ProjectileNum { get; private set; }
    
    private float _angle;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        SkillType = Define.ESkillType.FrozenHeart;

        return true;
    }
    
    public void InitSkill(int templateId, float radius, float angle)
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
        Radius = radius;
        RotationSpeed = data.Speed;
        ProjectileNum = data.ProjectileNum;
        
        _angle = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
    
        StartDestroy(LifeTime);
    }

    public void SetOwner(CreatureController owner)
    {
        _owner = owner;
    }
    
    private void Update()
    {
        if (_owner != null)
        {
            _angle += RotationSpeed * Time.deltaTime;
            
            // 플레이어를 중심으로 공전하는 위치 계산
            Vector3 offset = new Vector3(Mathf.Cos(_angle), Mathf.Sin(_angle), 0f) * Radius;
            transform.position = _owner.transform.position + offset;
            
            // 오브젝트의 방향을 플레이어 쪽으로 향하도록 설정
            Vector3 direction = _owner.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
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
    }
}