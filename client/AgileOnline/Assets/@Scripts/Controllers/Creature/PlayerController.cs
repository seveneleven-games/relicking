using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    private Vector2 _moveDir = Vector2.zero;

    public int PlayerId { get; private set; }
    public string Name { get; private set; }
    public int Atk { get; private set; }
    public float CritRate { get; private set; }
    public float CritDmgRate { get; private set; }
    public float CoolDown { get; private set; }

    public int PlayerGold { get; private set; }

    public List<int> PlayerSkillList { get; private set; }
    public List<int> PlayerRelicList { get; private set; }

    private Transform _indicator;

    private List<Coroutine> _skillCoroutines = new List<Coroutine>();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Player;
        CreatureState = ECreatureState.Idle;

        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        PlayerSkillList = new List<int>(new int[6]);
        PlayerRelicList = new List<int>(new int[6]);

        // AddSkill(3, 0);
        // AddSkill(1, 1);
        AddSkill(2, 2);

        // 보는 방향 정해주는 더미 오브젝트
        GameObject indicatorObject = new GameObject("Indicator");
        indicatorObject.transform.SetParent(transform);
        indicatorObject.transform.localPosition = Vector3.zero;
        indicatorObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        _indicator = indicatorObject.transform;

        StartSkills();

        return true;
    }

    // TODO: InitPlayer시 RelicList도 같이 받아야함
    public void InitPlayer(int templateId)
    {
        PlayerData data = Managers.Data.PlayerDic[templateId];

        PlayerId = data.PlayerId;
        Name = data.Name;
        MaxHp = data.MaxHp;
        Hp = MaxHp;
        Atk = data.Atk;
        Speed = data.Speed;
        CritRate = data.CritRate;
        CritDmgRate = data.CritDmgRate;
        CoolDown = data.CoolDown;

        PlayerSkillList = new List<int>(new int[6]);
        PlayerRelicList = new List<int>(new int[6]);
    }

    private void Update()
    {
        Vector3 dir = _moveDir * Time.deltaTime * Speed;
        transform.TranslateEx(dir);

        if (_moveDir != Vector2.zero)
        {
            _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
        }

        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
    }

    private void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case EJoystickState.PointerDown:
                CreatureState = ECreatureState.Move;
                break;
            case EJoystickState.Drag:
                break;
            case EJoystickState.PointerUp:
                CreatureState = ECreatureState.Idle;
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MonsterController target = collision.gameObject.GetComponent<MonsterController>();
        if (target == null)
            return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gold"))
        {
            GoldController gc = collision.GetComponent<GoldController>();

            int goldValue = gc.GoldValue;

            PlayerGold += goldValue;

            Destroy(collision.gameObject);

            Debug.Log($"획득한 골드: {goldValue}, 현재 골드 량: {PlayerGold}");
        }
    }

    public override void OnDamaged(BaseController attacker, int damage)
    {
        base.OnDamaged(attacker, damage);

        Debug.Log($"OnDamaged ! {Hp}");

        CreatureController cc = attacker as CreatureController;
        cc?.OnDamaged(this, 10000);
    }

    protected override void OnDead()
    {
        base.OnDead();
        CreatureState = ECreatureState.Dead;

        // TODO: Game 종료 씬으로~
        Managers.Scene.LoadScene(EScene.LobbyScene);
    }

    #region Skill

    void StartSkills()
    {
        StopSkills();

        foreach (int skillId in PlayerSkillList)
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
            switch (skillData.PrefabName)
            {
                case "EnergyBolt":
                    yield return coolTimeWait;

                    int projectileNum = skillData.ProjectileNum;
                    float spreadAngle = 30f;

                    for (int i = 0; i < projectileNum; i++)
                    {
                        EnergyBoltController ebc =
                            Managers.Object.Spawn<EnergyBoltController>(transform.position, skillId);
                        ebc.InitSkill(skillId);

                        float angle;
                        if (projectileNum == 1)
                            angle = 0f;
                        else
                        {
                            float offsetAngle = (i - (projectileNum - 1) * 0.5f) * (spreadAngle / (projectileNum - 1));
                            angle = offsetAngle * Mathf.Deg2Rad;
                        }

                        Vector3 moveDirection = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg) * _indicator.up;
                        ebc.SetMoveDirection(moveDirection);
                    }
                    break;
                
                case "IceArrow":
                    break;
            }
        }
    }

    public void AddSkill(int addSkillId, int slotNum)
    {
        PlayerSkillList[slotNum] = addSkillId;
    }

    #endregion
}