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

    private bool _hitState;
    protected bool IsRage = false;

    public virtual bool HitState
    {
        get => _hitState;
        set
        {
            if (!IsRage)
            {
                _hitState = value;
                Animator.SetBool("HitState", value);
            }
            
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        CreatureState = ECreatureState.Idle;
        HitState = false;
        return true;
    }

    public virtual bool OnDamaged(BaseController attacker, ref float damage)
    {
        bool isCritical = false;

        if (HitState == false)
        {
            HitState = true;
            StartCoroutine(StopHitEffect());
        }
        
        Debug.Log("때린놈 : " + attacker);
        Debug.Log("대미지 : " + damage);
        
        if (attacker is PlayerController playerAttacker)
        {
            float critRoll = UnityEngine.Random.value;
            if (critRoll <= playerAttacker.CritRate)
            {
                damage *= (int)playerAttacker.CritDmgRate;
                isCritical = true;
            }
        }
        
        Hp -= (int) damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }

        return isCritical;
    }

    private IEnumerator StopHitEffect()
    {
        yield return new WaitForSeconds(0.3f);
        HitState = false;
    }
    
    public virtual void OnDead()
    {
        CreatureState = ECreatureState.Dead;
    }
    
}
