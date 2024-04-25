using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
    // 추적 필요한 것만.
    public PlayerController Player { get; set; }
    public HashSet<MonsterController> Monsters { get; } = new HashSet<MonsterController>();

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

    #endregion

    public T Spawn<T>(Vector3 position, GameObject prefab) where T : BaseController
    {
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
        else if (obj.ObjectType == EObjectType.Projectile)
        {
            // TODO
        }
        
        return obj as T;
    }

    public void Despawn<T>(T obj) where T : BaseController
    {
        EObjectType objectType = obj.ObjectType;

        if (obj.ObjectType == EObjectType.Creature)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            switch (cc.CreatureType)
            {
                case ECreatureType.Player:
                    Player = null;
                    break;
                case ECreatureType.Monster:
                    MonsterController mc = cc as MonsterController;
                    Monsters.Remove(mc);
                    break;
            }
        }
        else if (obj.ObjectType == EObjectType.Projectile)
        {
            // TODO
        }
        
        Managers.Pool.Push(obj.gameObject);
    }
}