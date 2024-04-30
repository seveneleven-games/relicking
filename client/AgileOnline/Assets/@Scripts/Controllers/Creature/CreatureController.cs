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
    
    public event Action<float, float> OnHealthChanged;

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

    public virtual void OnDamaged(BaseController attacker, int damage)
    {
        Debug.Log(attacker + " " + damage);
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
    }

    protected virtual void OnDead()
    {
        transform.localScale = new Vector3(1, 1, 1);
        CreatureState = ECreatureState.Dead;
    }
    
}
