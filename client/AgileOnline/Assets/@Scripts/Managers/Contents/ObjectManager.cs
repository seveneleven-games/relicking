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
    public HashSet<IceArrowController> IceArrows { get; } = new HashSet<IceArrowController>();

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

    public Transform IceArrowRoot
    {
        get { return GetRootTransform("@IceArrow"); }
    }

    #endregion

    public T Spawn<T>(Vector3 position, int templateId) where T : BaseController
    {
        string dataType = typeof(T).Name.Replace("Controller", "Data");

        Debug.Log("이거 템플릿아이디임" + templateId);
        string prefabName = Managers.Data.GetData<T>(dataType, templateId);

        Debug.Log("이거 프리팹 이름" + prefabName);
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
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
            mc.InitMonster(templateId);
        }
        else if (obj.ObjectType == EObjectType.Env)
        {
            obj.transform.parent = GoldRoot;
            GoldController gc = go.GetComponent<GoldController>();
            Golds.Add(gc);
            gc.InitGold(templateId);
        }
        else if (obj.ObjectType == EObjectType.Skill)
        {
            SkillController sc = go.GetComponent<SkillController>();
            switch (sc.SkillType)
            {
                case ESkillType.EnergyBolt:
                    sc.transform.parent = EnergyBoltRoot;
                    EnergyBoltController ebc = sc.GetComponent<EnergyBoltController>();
                    EnergyBolts.Add(ebc);
                    ebc.InitSkill(templateId);
                    break;
                
                case ESkillType.IceArrow:
                    sc.transform.parent = IceArrowRoot;
                    IceArrowController iac = sc.GetComponent<IceArrowController>();
                    IceArrows.Add(iac);
                    iac.InitSkill(templateId);
                    break;
            }
        }

        return obj as T;
    }

    public void Despawn<T>(T obj) where T : BaseController
    {
        if (obj.ObjectType == EObjectType.Player)
        {
            Player = null;
        }
        else if (obj.ObjectType == EObjectType.Monster)
        {
            MonsterController mc = obj.GetComponent<MonsterController>();
            Monsters.Remove(mc);
        }
        else if (obj.ObjectType == EObjectType.Env)
        {
            GoldController gc = obj as GoldController;
            Golds.Remove(gc);
        }
        else if (obj.ObjectType == EObjectType.Skill)
        {
            SkillController sc = obj.GetComponent<SkillController>();
            switch (sc.SkillType)
            {
                case ESkillType.EnergyBolt:
                    EnergyBoltController ebc = sc as EnergyBoltController;
                    EnergyBolts.Remove(ebc);
                    break;
                
                case ESkillType.IceArrow:
                    IceArrowController iac = sc as IceArrowController;
                    IceArrows.Remove(iac);
                    break;
            }
        }

        Managers.Resource.Destroy(obj.gameObject);
    }
}