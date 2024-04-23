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

    public void Init()
    {
        PlayerDic = LoadJson<PlayerDataLoader, int, PlayerData>("PlayerData").MakeDict();
        MonsterDic = LoadJson<MonsterDataLoader, int, MonsterData>("MonsterData").MakeDict();
        StageDic = LoadJson<StageDataLoader, int, StageData>("StageData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}