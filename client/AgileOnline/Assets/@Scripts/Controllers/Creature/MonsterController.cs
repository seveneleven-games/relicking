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
    public float Atk { get; private set; }
    public float DropGold { get; private set; }
    public float CritRate { get; private set; }
    public float CritDmgRate { get; private set; }
    public float CoolDown { get; private set; }

    public List<int> MonsterSkillList { get; private set; }

    private Coroutine _skillCoroutine;
    private bool _isUsingSkill;
    private bool _isInCoolDown;

    private TemplateData _templateData;

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
        _templateData = Resources.Load<TemplateData>("GameTemplateData");
        int stageId = _templateData.StageId;
        int difficulty = Managers.Game.DicStageClearInfo[stageId].SelectedDifficulty;

        MonsterId = data.MonsterId;
        PrefabName = data.PrefabName;
        MonsterType = data.MonsterType;
        Name = data.Name;
        MaxHp = data.MaxHp * difficulty;
        Hp = MaxHp;
        Atk = data.Atk * difficulty;
        Speed = data.Speed;
        DropGold = data.DropGold;
        CritRate = data.CritRate;
        CritDmgRate = data.CritDmgRate;
        CoolDown = data.CoolDown;

        MonsterSkillList = new List<int>(new int[3]);
        if (MonsterType == 1)
        {
            MonsterSkillList[0] = 1001;
        }
        else if (MonsterType == 2)
        {
            MonsterSkillList[0] = 1001;
            MonsterSkillList[1] = 1011;
            transform.localScale = new Vector3(3, 3, 1);
        }
    }

    private void Start()
    {
        _player = Managers.Object.Player;
    }

    private void Update()
    {
        if (_player == null)
            return;

        if (!_isUsingSkill)
        {
            if (MonsterType == 2)
                Debug.Log("플레이어 따라가는중 : " + Speed);
            ChasePlayer();
        }
        else
        {
            if (MonsterType == 2)
                Debug.Log("플레이어 안 따라가는중 : " + Speed);
        }

        if (!_isInCoolDown && MonsterType != 0)
            StartRandomSkill();
    }

    private void ChasePlayer()
    {
        Vector3 dir = (_player.transform.position - transform.position).normalized;
        CreatureState = ECreatureState.Move;
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
            float damage = Atk;
            target.OnDamaged(this, ref damage);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override bool OnDamaged(BaseController attacker, ref float damage)
    {
        bool isCritical = base.OnDamaged(attacker, ref damage);
        UI_World.Instance.ShowDamage((int)damage, transform.position + Vector3.up * 1f, isCritical);
        if (gameObject.activeSelf && MonsterType != 2)
            StartCoroutine(HitStun(0.1f));

        return isCritical;
    }

    private IEnumerator HitStun(float duration)
    {
        CreatureState = ECreatureState.Idle;
        float originalSpeed = Speed;
        Speed = 0f;

        yield return new WaitForSeconds(duration);

        Speed = originalSpeed;

        yield return null;
    }

    public override void OnDead()
    {
        base.OnDead();
        CreatureState = ECreatureState.Dead;

        if (_coDotDamage != null)
            StopCoroutine(_coDotDamage);
        _coDotDamage = null;

        if (MonsterType == 2)
        {
            _player.IsBossKilled = true;
        }

        GoldController gc = Managers.Object.Spawn<GoldController>(transform.position, MonsterId);
        gc.InitGold(MonsterId);

        Managers.Object.Despawn(this);
    }

    #region Skill

    void StartRandomSkill()
    {
        if (_skillCoroutine != null)
            StopCoroutine(_skillCoroutine);
        
        int randomIndex = UnityEngine.Random.Range(0, MonsterSkillList.Count);
        int skillId = MonsterSkillList[randomIndex];

        if (skillId > 0)
        {
            _skillCoroutine = StartCoroutine(CoStartSkill(skillId));
        }
    }


    IEnumerator CoStartSkill(int skillId)
    {
        _isInCoolDown = true;

        SkillData skillData = Managers.Data.SkillDic[skillId];
        WaitForSeconds coolTimeWait = new WaitForSeconds(5f);

        switch (skillData.PrefabName)
        {
            case "EliteMonsterProjectile":
                if (MonsterType == 1)
                {
                    int empProjectileNum = skillData.ProjectileNum;
                    float angleStep1 = 360f / empProjectileNum;

                    for (int i = 0; i < empProjectileNum; i++)
                    {
                        float angle = i * angleStep1;
                        Vector3 direction = Quaternion.Euler(0f, 0f, angle) * Vector3.up;
                        EliteMonsterProjectileController emp =
                            Managers.Object.Spawn<EliteMonsterProjectileController>(transform.position, skillId);
                        emp.SetOwner(this);
                        emp.SetMoveDirection(direction);
                    }
                }

                if (MonsterType == 2)
                {
                    int empProjectileNum = skillData.ProjectileNum;
                    float angleStep1 = 360f / empProjectileNum;

                    for (int j = 0; j < 3; j++)
                    {
                        for (int i = 0; i < empProjectileNum; i++)
                        {
                            float angle = i * angleStep1;
                            Vector3 direction = Quaternion.Euler(0f, 0f, angle) * Vector3.up;
                            EliteMonsterProjectileController emp =
                                Managers.Object.Spawn<EliteMonsterProjectileController>(transform.position, skillId);
                            emp.SetOwner(this);
                            emp.SetMoveDirection(direction);
                        }

                        yield return new WaitForSeconds(0.5f);
                    }
                }

                break;

            case "BossMonsterCharge":
                _isUsingSkill = true;

                yield return new WaitForSeconds(1f);

                Vector3 playerDirection = (_player.transform.position - transform.position).normalized;
                
                yield return new WaitForSeconds(0.5f);

                for (float t = 0; t < 1.5f; t += Time.deltaTime)
                {
                    transform.position += playerDirection * 15f * Time.deltaTime;
                    yield return null;
                }

                _isUsingSkill = false;

                break;
        }

        yield return coolTimeWait;

        _isInCoolDown = false;
    }

    #endregion
}