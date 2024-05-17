using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class MeteorController : SkillController
{
    private CreatureController _owner;
    private Vector3 _moveDir;
    private float _elapsedTime = 0f;
    private float _duration = 2f;
    private float _distance = 4f;
    private Vector3 _initialPosition;

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

        SkillType = Define.ESkillType.Meteor;

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
        
        _initialPosition = transform.position;
    }

    private void OnEnable()
    {
        _elapsedTime = 0f;
        transform.position = _initialPosition;
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime <= _duration)
        {
            float t = _elapsedTime / _duration;
            float yOffset = Mathf.Lerp(0f, -_distance, t);
            transform.position += new Vector3(0f, yOffset, 0f) * Time.deltaTime;
        }
        else
        {
            Managers.Object.Despawn(this);
        }
    }
}