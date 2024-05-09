using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : BaseController
{
    public float Speed { get; protected set; }
    public int Hp { get; protected set; }
    public int MaxHp { get; protected set; }

    protected ECreatureState _creatureState = ECreatureState.None;

    public virtual ECreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_creatureState != value)
            {
                _creatureState = value;
                Animator.SetInteger("CreatureState", (int)_creatureState);
            }
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        CreatureState = ECreatureState.Idle;
        return true;
    }

    public virtual bool OnDamaged(BaseController attacker, ref int damage)
    {
        bool isCritical = false;
        
        if (attacker is PlayerController playerAttacker)
        {
            Debug.Log("플레이어임");
            float critRoll = UnityEngine.Random.value;
            if (critRoll <= playerAttacker.CritRate)
            {
                damage *= (int)playerAttacker.CritDmgRate;
                isCritical = true;
            }
        }
        else
        {
            Debug.Log("플레이어 아님..");
        }
        
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }

        return isCritical;
    }

    public virtual void OnDead()
    {
        CreatureState = ECreatureState.Dead;
    }
    
}
