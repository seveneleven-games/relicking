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

    private Transform _indicator;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = ECreatureType.Player;
        CreatureState = ECreatureState.Idle;
        Speed = 5.0f;

        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        // 보는 방향 정해주는 더미 오브젝트
        GameObject indicatorObject = new GameObject("Indicator");
        indicatorObject.transform.SetParent(transform);
        indicatorObject.transform.localPosition = Vector3.zero;
        indicatorObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        _indicator = indicatorObject.transform;
        
        StartProjectile();

        return true;
    }

    public void InitPlayer(Data.PlayerData data)
    {
        PlayerId = data.PlayerId;
        Name = data.Name;
        MaxHp = data.MaxHp;
        Hp = MaxHp;
        Atk = data.Atk;
        Speed = data.Speed;
        CritRate = data.CritRate;
        CritDmgRate = data.CritDmgRate;
        CoolDown = data.CoolDown;
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

        // TODO: Game 종료
    }

    #region EnergyBallProjectile

    private Coroutine _coEnergyBallProjectile;

    void StartProjectile()
    {
        if (_coEnergyBallProjectile != null)
            StopCoroutine(_coEnergyBallProjectile);

        _coEnergyBallProjectile = StartCoroutine(CoStartProjectile(1));
    }

    IEnumerator CoStartProjectile(int skillId)
    {
        SkillData skillData = Managers.Data.SkillDic[skillId];
        WaitForSeconds wait = new WaitForSeconds(skillData.CoolTime);

        while (true)
        {
            string skillDataPrefabName = skillData.PrefabName;
            ProjectileController pc = Managers.Object.Spawn<ProjectileController>(transform.position, skillDataPrefabName);
            SkillData projectileData = Managers.Data.SkillDic[skillId];
            pc.InitSkill(projectileData);

            Vector3 moveDirection = _indicator.up;
            pc.SetMoveDirection(moveDirection);

            yield return wait;
        }
    }

    #endregion
}