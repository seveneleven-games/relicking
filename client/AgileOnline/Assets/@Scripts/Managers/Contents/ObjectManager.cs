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
    public HashSet<ProjectileController> Projectiles { get; } = new HashSet<ProjectileController>();
    
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

    public Transform ProjectileRoot
    {
        get { return GetRootTransform("@Projectiles"); }
    }

    #endregion

    public T Spawn<T>(Vector3 position, string prefabName) where T : BaseController
    {
        GameObject prefab = Managers.Resource.Load<GameObject>(prefabName);
        GameObject go = Managers.Pool.Pop(prefab);
        go.name = prefab.name;
        go.transform.position = position;

        BaseController obj = go.GetComponent<BaseController>();
        
        if (obj.ObjectType == EObjectType.Creature)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            switch (cc.CreatureType)
            {
                case ECreatureType.Player:
                    obj.transform.parent = PlayerRoot;
                    Player = cc as PlayerController;
                    break;
                case ECreatureType.Monster:
                    obj.transform.parent = MonsterRoot;
                    MonsterController mc = cc as MonsterController;
                    Monsters.Add(mc);
                    break;
            }
        }
        else if (obj.ObjectType == EObjectType.Env)
        {
            GoldController gc = go.GetComponent<GoldController>();
            obj.transform.parent = GoldRoot;
            CircleCollider2D cc2D = obj.GetComponent<CircleCollider2D>();
            cc2D.isTrigger = true;
            Golds.Add(gc);
        }
        else if (obj.ObjectType == EObjectType.Projectile)
        {
            ProjectileController pc = go.GetComponent<ProjectileController>();
            obj.transform.parent = ProjectileRoot;
            CircleCollider2D cc2D = obj.GetComponent<CircleCollider2D>();
            cc2D.isTrigger = true;
            Projectiles.Add(pc);
        }
        
        return obj as T;
    }

    public void Despawn<T>(T obj) where T : BaseController
    {
        
        if (obj.ObjectType == EObjectType.Creature)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            switch (cc.CreatureType)
            {
                case ECreatureType.Player:
                    Player = null;
                    break;
                case ECreatureType.Monster:
                    Monsters.Remove(cc as MonsterController);
                    break;
            }
        }
        else if (obj.ObjectType == EObjectType.Env)
        {
            Golds.Remove(obj as GoldController);
        }
        else if (obj.ObjectType == EObjectType.Projectile)
        {
            Projectiles.Remove(obj as ProjectileController);
        }
        
        Managers.Pool.Push(obj.gameObject);
    }
}