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

    public HashSet<ChainLightningController> ChainLightnings { get; } = new HashSet<ChainLightningController>();

    public HashSet<ShurikenController> Shurikens { get; } = new HashSet<ShurikenController>();

    public HashSet<StormBladeController> StormBlades { get; } = new HashSet<StormBladeController>();

    public HashSet<BossMonsterThornController> BossMonsterThorns { get; } = new HashSet<BossMonsterThornController>();
    
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

    public Transform ChainLightningRoot
    {
        get { return GetRootTransform("@ChainLightning"); }
    }

    public Transform ShurikenRoot
    {
        get { return GetRootTransform("@Shuriken"); }
    }

    public Transform StormBladeRoot
    {
        get { return GetRootTransform("@StormBlade"); }
    }

    public Transform BossMonsterThornRoot
    {
        get { return GetRootTransform("@BossMonsterThorn"); }
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

    public T Spawn<T>(Vector3 position, int templateId, params object[] parameters) where T : BaseController
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
                    break;
                
                case ESkillType.ChainLightning:
                    Vector3 startPoint = (Vector3)parameters[0];
                    Vector3 endPoint = (Vector3)parameters[1];
                    sc.transform.parent = ChainLightningRoot;
                    ChainLightningController clc = sc.GetComponent<ChainLightningController>();
                    ChainLightnings.Add(clc);
                    clc.InitSkill(templateId, startPoint, endPoint);
                    break;
                
                case ESkillType.Shuriken:
                    sc.transform.parent = ShurikenRoot;
                    ShurikenController skc = sc.GetComponent<ShurikenController>();
                    Shurikens.Add(skc);
                    skc.InitSkill(templateId);
                    break;
                
                case ESkillType.StormBlade:
                    sc.transform.parent = StormBladeRoot;
                    StormBladeController sbc = sc.GetComponent<StormBladeController>();
                    StormBlades.Add(sbc);
                    sbc.InitSkill(templateId);
                    break;
                
                case ESkillType.BossMonsterThorn:
                    sc.transform.parent = BossMonsterThornRoot;
                    BossMonsterThornController bmtc = sc.GetComponent<BossMonsterThornController>();
                    BossMonsterThorns.Add(bmtc);
                    bmtc.InitSkill(templateId);
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
                
                case ESkillType.ChainLightning:
                    ChainLightningController clc = sc as ChainLightningController;
                    ChainLightnings.Remove(clc);
                    break;
                
                case ESkillType.Shuriken:
                    ShurikenController skc = sc as ShurikenController;
                    Shurikens.Remove(skc);
                    break;
                
                case ESkillType.StormBlade:
                    StormBladeController sbc = sc as StormBladeController;
                    StormBlades.Remove(sbc);
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
    
    public void CleanupResources()
    {
        // 몬스터와 골드 오브젝트 despawn
        DespawnObjects<MonsterController>("@Monsters");
        DespawnObjects<GoldController>("@Golds");

        // 맵 오브젝트 파괴
        DestroyObjects("@BaseMap");

        // 오브젝트 풀 정리
        Managers.Pool.Clear();
    }

    private void DespawnObjects<T>(string parentName) where T : MonoBehaviour
    {
        GameObject parentObject = GameObject.Find(parentName);
        if (parentObject != null)
        {
            foreach (Transform child in parentObject.transform)
            {
                T component = child.gameObject.GetComponent<T>();
                if (component != null)
                {
                    BaseController baseController = component as BaseController;
                    if (baseController != null)
                        Managers.Object.Despawn(baseController);
                }
            }
        }
    }

    private void DestroyObjects(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            Managers.Resource.Destroy(obj);
        }
    }
}