﻿using System.Collections;
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

    public void Init()
    {
        PlayerDic = LoadJson<PlayerDataLoader, int, PlayerData>("PlayerData").MakeDict();
        MonsterDic = LoadJson<MonsterDataLoader, int, MonsterData>("MonsterData").MakeDict();
        StageDic = LoadJson<StageDataLoader, int, StageData>("StageData").MakeDict();
        SkillDic = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDict();
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
}