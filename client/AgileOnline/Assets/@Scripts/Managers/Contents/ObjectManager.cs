using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
    // 추적 필요한 것만.
    public PlayerController Player { get; set; }
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();
    public HashSet<GoldController> Golds { get; } = new HashSet<GoldController>();
    public HashSet<EnergyBoltController> EnergyBolts { get; } = new HashSet<EnergyBoltController>();
    
    #region Roots

    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }
    
    public Transform PlayerRoot
    {
        get { return GetRootTransform("@Player"); }
    }
    
    public Transform MonsterRoot
    {
        get { return GetRootTransform("@Monsters"); }
    }

    public Transform GoldRoot
    {
        get { return GetRootTransform("@Golds"); }
    }

    public Transform EnergyBoltRoot
    {
        get { return GetRootTransform("@EnergyBolt"); }
    }

    #endregion

    public T Spawn<T>(Vector3 position, int templateId) where T : BaseController
    {
        string dataType = typeof(T).Name.Replace("Controller", "Data");

        string prefabName = Managers.Data.GetData<T>(dataType, templateId);
        
        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = position;
        
        BaseController obj = go.GetComponent<BaseController>();
        
        if (obj.ObjectType == EObjectType.Player)
        {
            obj.transform.parent = PlayerRoot;
            PlayerController pc = go.GetComponent<PlayerController>();
            Player = pc;
            pc.InitPlayer(templateId);
        }
        else if (obj.ObjectType == EObjectType.Monster)
        {
            obj.transform.parent = MonsterRoot;
            MonsterController mc = go.GetComponent<MonsterController>();
            Monsters.Add(mc);
            mc.SetInfo(templateId);
        }

        return obj as T;
    }

    public void Despawn<T>(T obj) where T : BaseController
    {
        EObjectType objectType = obj.ObjectType;

        // if (obj.ObjectType == EObjectType.Hero)
        // {
        //     Hero hero = obj.GetComponent<Hero>();
        //     Heroes.Remove(hero);
        // }
        // else if (obj.ObjectType == EObjectType.Monster)
        // {
        //     Monster monster = obj.GetComponent<Monster>();
        //     Monsters.Remove(monster);
        // }
        // else if (obj.ObjectType == EObjectType.Projectile)
        // {
        //     Projectile projectile = obj as Projectile;
        //     Projectiles.Remove(projectile);
        // }
        // else if (obj.ObjectType == EObjectType.Env)
        // {
        //     Env env = obj as Env;
        //     Envs.Remove(env);
        // }
        // else if (obj.ObjectType == EObjectType.Effect)
        // {
        //     EffectBase effect = obj as EffectBase;
        //     Effects.Remove(effect);
        // }
        // else if (obj.ObjectType == EObjectType.HeroCamp)
        // {
        //     Camp = null;
        // }
        // else if (obj.ObjectType == EObjectType.Npc)
        // {
        //     Npc npc = obj as Npc;
        //     Npcs.Remove(npc);
        // }

        Managers.Resource.Destroy(obj.gameObject);
    }
}