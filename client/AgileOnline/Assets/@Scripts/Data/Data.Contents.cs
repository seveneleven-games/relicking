using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 파싱해주는 곳 (Json)

namespace Data
{
    #region PlayerData
    
    [Serializable]
    public class PlayerData
    {
        public int PlayerId;
        public string PrefabName;
        public string ThumbnailName;
        public string Name;
        public string Description;
        public int MaxHp;
        public int Atk;
        public float Speed;
        public float CritRate;
        public float CritDmgRate;
        public float CoolDown;
        public float ExtraGold;
    }

    [Serializable]
    public class PlayerDataLoader : ILoader<int, PlayerData>
    {
        public List<PlayerData> players = new List<PlayerData>();

        public Dictionary<int, PlayerData> MakeDict()
        {
            Dictionary<int, PlayerData> dict = new Dictionary<int, PlayerData>();
            foreach (PlayerData player in players)
                dict.Add(player.PlayerId, player);

            return dict;
        }
    }
    
    #endregion
    
    #region MonsterData

    [Serializable]
    public class MonsterData
    {
        public int MonsterId;
        public string PrefabName;
        public int MonsterType;
        public string Name;
        public int MaxHp;
        public float Atk;
        public float Speed;
        public int DropGold;
        public float CritRate;
        public float CritDmgRate;
        public float CoolDown;
    }
    
    [Serializable]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();

        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
                dict.Add(monster.MonsterId, monster);

            return dict;
        }
    }
    
    #endregion

    #region StageData

    [Serializable]
    public class StageData
    {
        public int StageId;
        public string Name;
        public string ThumbnailName;
        public int[] NodeMaps;
    }
    
    [Serializable]
    public class StageDataLoader : ILoader<int, StageData>
    {
        public List<StageData> stages = new List<StageData>();

        public Dictionary<int, StageData> MakeDict()
        {
            Dictionary<int, StageData> dict = new Dictionary<int, StageData>();
            foreach (StageData stage in stages)
                dict.Add(stage.StageId, stage);

            return dict;
        }
    }

    #endregion
    
    #region SkillData

    [Serializable]
    public class SkillData
    {
        public int SkillId;
        public int NextId;
        public string PrefabName;
        public string Name;
        public string Description;
        public string IconName; 
        public float CoolTime;
        public float Damage;
        public float LifeTime;
        public float Speed;
        public int ProjectileNum;
    }
    
    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dict.Add(skill.SkillId, skill);

            return dict;
        }
    }

    #endregion

    #region GoldData

    [Serializable]
    public class GoldData
    {
        public int GoldId;
        public string PrefabName;
        public string Name;
        public int DropGold;
    }
    
    [Serializable]
    public class GoldDataLoader : ILoader<int, GoldData>
    {
        public List<GoldData> golds = new List<GoldData>();

        public Dictionary<int, GoldData> MakeDict()
        {
            Dictionary<int, GoldData> dict = new Dictionary<int, GoldData>();
            foreach (GoldData gold in golds)
                dict.Add(gold.GoldId, gold);

            return dict;
        }
    }

    #endregion
    
    #region NodeMapData

    [Serializable]
    public class NodeData
    {
        public int NodeDepth;
        public string MapPrefabName;
        public int[] MonsterList;
    }

    [Serializable]
    public class NodeMapData
    {
        public int NodeMapId;
        public string PrefabName;
        public string BackgroundImage;
        public List<NodeData> NodeList;
    }

    [Serializable]
    public class NodeMapDataLoader : ILoader<int, NodeMapData>
    {
        public List<NodeMapData> nodeMaps;

        public Dictionary<int, NodeMapData> MakeDict()
        {
            Dictionary<int, NodeMapData> dict = new Dictionary<int, NodeMapData>();
            foreach (NodeMapData nodeMap in nodeMaps)
                dict.Add(nodeMap.NodeMapId, nodeMap);

            return dict;
        }
    }

    #endregion

    #region RelicData

    [Serializable]
    public class RelicData
    {
        public int RelicId;
        public string PrefabName;
        public string Name;
        public string ThumbnailName;
        public string Description;
        public int Rarity;
        public int Atk;
        public int MaxHp;
        public int CoolTime;
        public int Speed;
    }

    [Serializable]
    public class RelicDataLoader : ILoader<int, RelicData>
    {
        public List<RelicData> relics = new List<RelicData>();

        public Dictionary<int, RelicData> MakeDict()
        {
            Dictionary<int, RelicData> dict = new Dictionary<int, RelicData>();
            foreach (RelicData relic in relics)
                dict.Add(relic.RelicId, relic);

            return dict;
        }
    }

    #endregion
}