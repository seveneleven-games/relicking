using System.Collections;
using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, PlayerData> PlayerDic { get; private set; } = new Dictionary<int, PlayerData>();
    public Dictionary<int, MonsterData> MonsterDic { get; private set; } = new Dictionary<int, MonsterData>();
    public Dictionary<int, StageData> StageDic { get; private set; } = new Dictionary<int, StageData>();
    public Dictionary<int, SkillData> SkillDic { get; private set; } = new Dictionary<int, SkillData>();
    public Dictionary<int, GoldData> GoldDic { get; private set; } = new Dictionary<int, GoldData>();
    public Dictionary<int, NodeMapData> NodeMapDic { get; private set; } = new Dictionary<int, NodeMapData>();
    public Dictionary<int, RelicData> RelicDic { get; private set; } = new Dictionary<int, RelicData>();
    
    public void Init()
    {
        PlayerDic = LoadJson<PlayerDataLoader, int, PlayerData>("PlayerData").MakeDict();
        MonsterDic = LoadJson<MonsterDataLoader, int, MonsterData>("MonsterData").MakeDict();
        StageDic = LoadJson<StageDataLoader, int, StageData>("StageData").MakeDict();
        SkillDic = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDict();
        GoldDic = LoadJson<GoldDataLoader, int, GoldData>("GoldData").MakeDict();
        NodeMapDic = LoadJson<NodeMapDataLoader, int, NodeMapData>("NodeMapData").MakeDict();
        RelicDic = LoadJson<RelicDataLoader, int, RelicData>("RelicData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
        if (textAsset == null)
        {
            Debug.LogError($"Failed to load JSON file: {path}");
            return default(Loader);
        }
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
    
    
    public string GetData<T>(string dataType, int templateId) where T : class
    {
        switch (dataType)
        {
            case "PlayerData":
                return Managers.Data.PlayerDic[templateId].PrefabName;
            case "MonsterData":
                return Managers.Data.MonsterDic[templateId].PrefabName;
            case "GoldData":
                return Managers.Data.GoldDic[templateId].PrefabName;
            case "EnergyBoltData":
                return Managers.Data.SkillDic[templateId].PrefabName;
            case "IceArrowData":
                return Managers.Data.SkillDic[templateId].PrefabName;
            case "ElectronicFieldData":
                return Managers.Data.SkillDic[templateId].PrefabName;
            case "PoisonFieldData":
                return Managers.Data.SkillDic[templateId].PrefabName;
            case "EliteMonsterProjectileData":
                return Managers.Data.SkillDic[templateId].PrefabName;
            case "WindCutterData":
                return Managers.Data.SkillDic[templateId].PrefabName;
            case "FrozenHeartData":
                return Managers.Data.SkillDic[templateId].PrefabName;
            case "ChainLightningData":
                return Managers.Data.SkillDic[templateId].PrefabName;
            case "MeteorHitData":
                return Managers.Data.SkillDic[templateId].PrefabName;
            case "MeteorData":
                return Managers.Data.SkillDic[templateId].PrefabName;
            case "MeteorShadowData":
                return Managers.Data.SkillDic[templateId].PrefabName;
            default:
                return null;
        }
    }
}