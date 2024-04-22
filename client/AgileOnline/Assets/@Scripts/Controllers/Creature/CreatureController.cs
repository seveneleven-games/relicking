using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : BaseController
{
    public float Speed { get; protected set; } = 1.0f;
    public int Hp { get; protected set; } = 100;
    public int MaxHp { get; protected set; } = 100;

    public ECreatureType CreatureType { get; protected set; } = ECreatureType.None;

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

        ObjectType = EObjectType.Creature;
        CreatureState = ECreatureState.Idle;
        return true;
    }

    public virtual void OnDamaged(BaseController attacker, int damage)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }
    }

    protected virtual void OnDead()
    {
        
    }
    
}
