using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    private Vector2 _moveDir = Vector2.zero;
    
    public int PlayerId { get; private set; }
    public string Name { get; protected set; }
    public int Atk { get; protected set; }
    public float DropGold { get; private set; }
    public float CritRate { get; protected set; }
    public float CritDmgRate { get; protected set; }
    public float CoolDown { get; protected set; }
    public List<int> SkillList { get; protected set; }
    
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
        DropGold = data.DropGold;
        CritRate = data.CritRate;
        CritDmgRate = data.CritDmgRate;
        CoolDown = data.CoolDown;
        SkillList = data.SkillList;
    }

    private void Update()
    {
        transform.TranslateEx(_moveDir * Time.deltaTime * Speed);
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
}
