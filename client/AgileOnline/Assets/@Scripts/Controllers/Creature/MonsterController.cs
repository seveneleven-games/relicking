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
    public int DropGold { get; private set; }
    public float CritRate { get; private set; }
    public float CritDmgRate { get; private set; }
    public float CoolDown { get; private set; }

    public List<int> MonsterSkillList { get; private set; }

    private Coroutine _skillCoroutine;
    private bool _isUsingSkill;
    private bool _isInCoolDown;

    private TemplateData _templateData;
    
    private UI_InGamePopup _inGamePopup = Managers.UI.GetPopupUI<UI_InGamePopup>();

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

        MonsterSkillList = new List<int>(new int[4]);
        if (MonsterType == 1)
        {
            MonsterSkillList[0] = 1001;
        }
        else if (MonsterType == 2)
        {
            _inGamePopup.CheckBossNode();
            MonsterSkillList[0] = 1001;
            MonsterSkillList[1] = 1011;
            MonsterSkillList[2] = 1021;
            MonsterSkillList[3] = 1031;
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
            ChasePlayer();
        
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
        // if (gameObject.activeSelf && MonsterType != 2)
        //     StartCoroutine(HitStun(0.1f));
        if (MonsterType == 2)
            _inGamePopup.UpdateBossHealth(Hp, MaxHp);

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

        GoldController gc = Managers.Object.Spawn<GoldController>(transform.position, DropGold);
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
                        Vector3 direction1 = Quaternion.Euler(0f, 0f, angle) * Vector3.up;
                        EliteMonsterProjectileController emp =
                            Managers.Object.Spawn<EliteMonsterProjectileController>(transform.position, skillId);
                        emp.SetOwner(this);
                        emp.SetMoveDirection(direction1);
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

                GameObject go1 = Managers.Resource.Instantiate("LineAlert");
                ParticleSystem ps1 = go1.GetComponent<ParticleSystem>();
                ps1.transform.position = transform.position;
                float psAngle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
                ps1.transform.rotation = Quaternion.AngleAxis(psAngle, Vector3.forward);
                ps1.transform.rotation *= Quaternion.Euler(0f, 0f, -90f);
                ps1.transform.localScale = new Vector3(ps1.transform.localScale.x * 2f, ps1.transform.localScale.y * 10f, ps1.transform.localScale.z);

                yield return new WaitForSeconds(0.5f);
                Destroy(go1);

                for (float t = 0; t < 1.5f; t += Time.deltaTime)
                {
                    transform.position += playerDirection * 15f * Time.deltaTime;
                    yield return null;
                }

                _isUsingSkill = false;

                break;
            
            case "BossMonsterThorn":
                for (int x = -8; x <= 8; x += 4) {
                    for (int y = -8; y <= 8; y += 4) {
                        Vector3 spawnPosition = new Vector3(x, y, 0);
                        GameObject go2 = Managers.Resource.Instantiate("CircleAlert");
                        ParticleSystem ps2 = go2.GetComponent<ParticleSystem>();
                        ps2.transform.position = spawnPosition;
                        ps2.Play();
                    }
                }

                yield return new WaitForSeconds(1.5f);
                
                for (int x = -8; x <= 8; x += 4) {
                    for (int y = -8; y <= 8; y += 4) {
                        Vector3 spawnPosition = new Vector3(x, y, 0);
                        BossMonsterThornController bmtc = 
                            Managers.Object.Spawn<BossMonsterThornController>(spawnPosition, skillId);
                        bmtc.SetOwner(this);
                    }
                }
                break;
            
            case "BossMonsterJump":
                _isUsingSkill = true;
                Vector3 targetPosition = _player.transform.position;
                GameObject go3 = Managers.Resource.Instantiate("CircleAlert");
                ParticleSystem ps3 = go3.GetComponent<ParticleSystem>();
                ps3.transform.position = targetPosition;
                ps3.transform.localScale = new Vector3(0.8f, 0.8f, 0);
                ps3.Play();
                yield return new WaitForSeconds(0.8f);
                transform.position = targetPosition;
                _isUsingSkill = false;
                break;
        }

        yield return coolTimeWait;

        _isInCoolDown = false;
    }

    #endregion
}