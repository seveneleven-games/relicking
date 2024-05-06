using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public HashSet<ElectronicFieldController> ElectronicFields { get; } = new HashSet<ElectronicFieldController>();
    public HashSet<PoisonFieldController> PoisonFields { get; } = new HashSet<PoisonFieldController>();
    public HashSet<EliteMonsterProjectileController> EliteMonsterProjectiles { get; } =
        new HashSet<EliteMonsterProjectileController>();

    public HashSet<WindCutterController> WindCutters { get; } = new HashSet<WindCutterController>();
    public HashSet<FrozenHeartController> FrozenHearts { get; } = new HashSet<FrozenHeartController>();

    public HashSet<MeteorHitController> MeteorHits { get; } = new HashSet<MeteorHitController>();

    public HashSet<MeteorController> Meteors { get; } = new HashSet<MeteorController>();

    public HashSet<MeteorShadowController> MeteorShadows { get; } = new HashSet<MeteorShadowController>();
    
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

    public Transform ElectronicFieldRoot
    {
        get { return GetRootTransform("@ElectronicField"); }
    }
    
    public Transform PoisonFieldRoot
    {
        get { return GetRootTransform("@PoisonField"); }
    }

    public Transform EliteMonsterProjectileRoot
    {
        get { return GetRootTransform("@EliteMonsterProjectile"); }
    }

    public Transform WindCutterRoot
    {
        get { return GetRootTransform("@WindCutter"); }
    }

    public Transform FrozenHeartRoot
    {
        get { return GetRootTransform("@FrozenHeart"); }
    }

    public Transform MeteorRoot
    {
        get { return GetRootTransform("@Meteor"); }
    }
    
    public Transform MeteorShadowRoot
    {
        get { return GetRootTransform("@MeteorShadow"); }
    }

    public Transform MeteorHitRoot
    {
        get { return GetRootTransform("@MeteorHit"); }
    }

    #endregion
    
    public PlayerController CreatePlayer(int templateId)
    {
        string prefabName = Managers.Data.PlayerDic[templateId].PrefabName;
        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = Vector3.zero;

        PlayerController pc = go.GetComponent<PlayerController>();
        Player = pc;
        pc.InitPlayer(templateId);

        return pc;
    }

    public T Spawn<T>(Vector3 position, int templateId) where T : BaseController
    {
        string dataType = typeof(T).Name.Replace("Controller", "Data");

        string prefabName = "";
        
        if (dataType == "MeteorShadowData")
        {
            prefabName = "MeteorShadow";
        }
        else if (dataType == "MeteorHitData")
        {
            prefabName = "MeteorHit";
        }
        else
        {
            prefabName = Managers.Data.GetData<T>(dataType, templateId);   
        }
        
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        go.name = prefabName;
        go.transform.position = position;

        BaseController obj = go.GetComponent<BaseController>();

        if (obj.ObjectType == EObjectType.Player)
        {
            obj.transform.parent = PlayerRoot;
            Player = go.GetComponent<PlayerController>();
            Player.InitPlayer(templateId);
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
                
                case ESkillType.ElectronicField:
                    sc.transform.parent = ElectronicFieldRoot;
                    ElectronicFieldController efc = sc.GetComponent<ElectronicFieldController>();
                    ElectronicFields.Add(efc);
                    efc.InitSkill(templateId);
                    break;
                
                case ESkillType.PoisonField:
                    sc.transform.parent = PoisonFieldRoot;
                    PoisonFieldController pfc = sc.GetComponent<PoisonFieldController>();
                    PoisonFields.Add(pfc);
                    pfc.InitSkill(templateId);
                    break;
                
                case ESkillType.EliteMonsterProjectile:
                    sc.transform.parent = EliteMonsterProjectileRoot;
                    EliteMonsterProjectileController empc = sc.GetComponent<EliteMonsterProjectileController>();
                    EliteMonsterProjectiles.Add(empc);
                    empc.InitSkill(templateId);
                    break;
                
                case ESkillType.WindCutter:
                    sc.transform.parent = WindCutterRoot;
                    WindCutterController wcc = sc.GetComponent<WindCutterController>();
                    WindCutters.Add(wcc);
                    wcc.InitSkill(templateId);
                    break;
                
                case ESkillType.FrozenHeart:
                    sc.transform.parent = FrozenHeartRoot;
                    FrozenHeartController fhc = sc.GetComponent<FrozenHeartController>();
                    FrozenHearts.Add(fhc);
                    break;
                
                case ESkillType.Meteor:
                    sc.transform.parent = MeteorRoot;
                    MeteorController mc = sc.GetComponent<MeteorController>();
                    Meteors.Add(mc);
                    mc.InitSkill(templateId);
                    break;

                case ESkillType.MeteorShadow:
                    sc.transform.parent = MeteorShadowRoot;
                    MeteorShadowController msc = sc.GetComponent<MeteorShadowController>();
                    msc.InitSkill(templateId);
                    break;

                case ESkillType.MeteorHit:
                    sc.transform.parent = MeteorHitRoot;
                    MeteorHitController mhc = sc.GetComponent<MeteorHitController>();
                    MeteorHits.Add(mhc);
                    mhc.InitSkill(templateId);
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
                
                case ESkillType.ElectronicField:
                    ElectronicFieldController efc = sc as ElectronicFieldController;
                    ElectronicFields.Remove(efc);
                    break;
                
                case ESkillType.PoisonField:
                    PoisonFieldController pfc = sc as PoisonFieldController;
                    PoisonFields.Remove(pfc);
                    break;
                
                case ESkillType.EliteMonsterProjectile:
                    EliteMonsterProjectileController empc = sc as EliteMonsterProjectileController;
                    EliteMonsterProjectiles.Remove(empc);
                    break;
                
                case ESkillType.WindCutter:
                    WindCutterController wcc = sc as WindCutterController;
                    WindCutters.Remove(wcc);
                    break;
                
                case ESkillType.FrozenHeart:
                    FrozenHeartController fhc = sc as FrozenHeartController;
                    FrozenHearts.Remove(fhc);
                    break;
                
                case ESkillType.MeteorHit:
                    MeteorHitController mhc = sc as MeteorHitController;
                    MeteorHits.Remove(mhc);
                    break;
                
                case ESkillType.Meteor:
                    MeteorController mtc = sc as MeteorController;
                    Meteors.Remove(mtc);
                    break;
                
                case ESkillType.MeteorShadow:
                    MeteorShadowController msc = sc as MeteorShadowController;
                    MeteorShadows.Remove(msc);
                    break;
            }
        }

        Managers.Resource.Destroy(obj.gameObject);
    }

    public List<MonsterController> GetNearestMonsters(int count = 1, int distanceThreshold = 0)
    {
        List<MonsterController> monsterList = Monsters
            .OrderBy(monster => (Player.CenterPosition - monster.CenterPosition).sqrMagnitude).ToList();

        if (distanceThreshold > 0)
            monsterList = monsterList.Where(monster =>
                (Player.CenterPosition - monster.CenterPosition).magnitude > distanceThreshold).ToList();
        
        int min = Mathf.Min(count, monsterList.Count);
        
        List<MonsterController> nearestMonsters = monsterList.Take(min).ToList();
        
        if (nearestMonsters.Count == 0) return null;
        
        while (nearestMonsters.Count < count)
        {
            nearestMonsters.Add(nearestMonsters.Last());
        }
        
        return nearestMonsters;
    }
}