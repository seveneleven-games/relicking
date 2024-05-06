using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using static Define;

public class PoisonFieldController : SkillController
{
    private CreatureController _owner;
    private float _damageInterval = 0.5f;
    private float _lastDamageTime;

    private float _scaleUpDuration = 1f;
    private Vector3 _initialScale;
    private Vector3 _targetScale;
    
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
        
        SkillType = ESkillType.PoisonField;

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
        
        _initialScale = transform.localScale * 0.1f;
        _targetScale = transform.localScale;

        transform.localScale = _initialScale;

        StartCoroutine(ScaleUpCoroutine());
        
        StartDestroy(LifeTime);
    }
    
    private IEnumerator ScaleUpCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _scaleUpDuration)
        {
            transform.localScale = Vector3.Lerp(_initialScale, _targetScale, elapsedTime / _scaleUpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = _targetScale;
    }
    
    private void Update()
    {
        if (Time.time - _lastDamageTime >= _damageInterval)
        {
            _lastDamageTime = Time.time;
            DealDamageToNearbyMonsters();
        }
    }
    
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
