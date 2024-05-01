using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using TMPro;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    private PlayerController _player;
    
    public int MonsterId { get; private set; }
    public string PrefabName { get; private set; }
    public int MonsterType { get; private set; }
    public string Name { get; private set; }
    public int Atk { get; private set; }
    public float DropGold { get; private set; }
    public float CritRate { get; private set; }
    public float CritDmgRate { get; private set; }
    public float CoolDown { get; private set; }
    
    public List<int> MonsterSkillList { get; private set; }
    
    private List<Coroutine> _skillCoroutines = new List<Coroutine>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Monster;
        CreatureState = ECreatureState.Idle;

        return true;
    }
    
    public void InitMonster(int templateId)
    {
        MonsterData data = Managers.Data.MonsterDic[templateId];
        
        MonsterId = data.MonsterId;
        PrefabName = data.PrefabName;
        MonsterType = data.MonsterType;
        Name = data.Name;
        MaxHp = data.MaxHp;
        Hp = MaxHp;
        Atk = data.Atk;
        Speed = data.Speed;
        DropGold = data.DropGold;
        CritRate = data.CritRate;
        CritDmgRate = data.CritDmgRate;
        CoolDown = data.CoolDown;
        
        MonsterSkillList = new List<int>(new int[3]);
        if (MonsterType == 1)
        {
            MonsterSkillList[0] = 1000;
        }
    }

    private void Start()
    {
        _player = Managers.Object.Player;
        
        if (MonsterType == 1)
        {
            StartSkills();
        }
        // GameObject hudPosObject = new GameObject("HUDPos");
        // hudPosObject.transform.SetParent(transform);
        // hudPosObject.transform.localPosition = Vector3.up * 1.5f;
        // hudPos = hudPosObject.transform;
    }

    private void Update()
    {
        if (_player == null)
            return;
        
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        Vector3 dir = (_player.transform.position - transform.position).normalized;
        TranslateEx(dir * Time.deltaTime * Speed);
    }

    private void TranslateEx(Vector3 dir)
    {
        transform.Translate(dir);

        if (dir.x < 0)
            LookLeft = true;
        else if (dir.x > 0)
            LookLeft = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController target = collision.gameObject.GetComponent<PlayerController>();
        if (target.IsValid() == false)
            return;
        if (this.IsValid() == false)
            return;
        
        if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);

        _coDotDamage = StartCoroutine(CoStartDotDamage(target));
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        PlayerController target = collision.gameObject.GetComponent<PlayerController>();
        if (target.IsValid() == false)
            return;
        if (this.IsValid() == false)
            return;
        if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);
        _coDotDamage = null;
    }

    private Coroutine _coDotDamage;
    public IEnumerator CoStartDotDamage(PlayerController target)
    {
        while (true)
        {
            target.OnDamaged(this, Atk);
            yield return new WaitForSeconds(0.1f);
        }
    }

    // public override void OnDamaged(BaseController attacker, int damage)
    // {
    //     base.OnDamaged(attacker, damage);
    //     TakeDamage(damage);
    // }
    
    protected override void OnDead()
    {
        base.OnDead();
        
        if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);
        _coDotDamage = null;
        
        // if (hudPos != null && hudPos.childCount > 0)
        // {
        //     for (int i = 0; i < hudPos.childCount; i++)
        //     {
        //         Destroy(hudPos.GetChild(i).gameObject);
        //     }
        // }

        GoldController gc = Managers.Object.Spawn<GoldController>(transform.position, MonsterId);
        gc.InitGold(MonsterId);
        
        Managers.Object.Despawn(this);
    }

    public GameObject hudDamageText;
    public Transform hudPos;

    // private void TakeDamage(int damage)
    // {
    //     if (hudPos != null && hudPos.childCount > 0)
    //     {
    //         for (int i = 0; i < hudPos.childCount; i++)
    //         {
    //             Managers.Pool.Push(hudPos.GetChild(i).gameObject);
    //         }
    //     }
    //     
    //     GameObject hudText = Managers.Pool.Pop(hudDamageText);
    //     hudText.transform.position = hudPos.position;
    //     hudText.GetComponent<Damage>().damage = damage;
    // }

    #region Skill

    void StartSkills()
    {
        StopSkills();

        foreach (int skillId in MonsterSkillList)
        {
            if (skillId > 0)
            {
                Coroutine skillCoroutine = StartCoroutine(CoStartSkill(skillId));
                _skillCoroutines.Add(skillCoroutine);
            }
        }
    }
    
    void StopSkills()
    {
        foreach (Coroutine coroutine in _skillCoroutines)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        _skillCoroutines.Clear();
    }
    
    IEnumerator CoStartSkill(int skillId)
    {
        SkillData skillData = Managers.Data.SkillDic[skillId];
        WaitForSeconds coolTimeWait = new WaitForSeconds(skillData.CoolTime);
        while (true)
        {
            yield return coolTimeWait;
            switch (skillData.PrefabName)
            {
                case "EliteMonsterProjectile":
                    int empProjectileNum = skillData.ProjectileNum;
                    float angleStep = 360f / empProjectileNum;

                    for (int i = 0; i < empProjectileNum; i++)
                    {
                        float angle = i * angleStep;
                        Vector3 direction = Quaternion.Euler(0f, 0f, angle) * Vector3.up;
                        Debug.Log("스킬타입: " + PrefabName);
                        EliteMonsterProjectileController emp = Managers.Object.Spawn<EliteMonsterProjectileController>(transform.position, skillId);
                        if (emp != null)
                        {
                            emp.SetMoveDirection(direction);
                        }
                        else
                        {
                            Debug.LogError("Failed to spawn or initialize EliteMonsterProjectileController");
                        }
                    }
                    break;
            }
        }
    }
    

    #endregion

}