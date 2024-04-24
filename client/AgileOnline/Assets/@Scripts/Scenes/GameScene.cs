using System.Collections;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    private const float MONSTER_SPAWN_INTERVAL = 5f;
    private const int PER_SEC_MOSTER_GENERATION = 20;
    private const float TARGET_SPAWN_TIME = 0.5f;
    private const int NORMAL_MONSTER = 0;
    private const int ELITE_MONSTER = 1;
    private const int BOSS_MONSTER = 2;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.GameScene;
        
        // TODO: 상점 UI에서 StartGame을 시작하도록 변경해야함
        StartGame(1, 1, 0);

        return true;
    }

    public void StartGame(int stageId, int playerId, int nodeType)
    {
        StageData stageData = Managers.Data.StageDic[stageId];
        
        GameObject map = Managers.Resource.Instantiate(stageData.PrefabName);
        map.transform.position = Vector3.zero;
        map.name = "@BaseMap";
        
        PlayerData playerData = Managers.Data.PlayerDic[playerId];

        PlayerController pc = Managers.Object.Spawn<PlayerController>(Vector3.zero, playerData.PrefabName);
        pc.InitPlayer(playerData);

        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = pc;

        GameObject joystickObject = Managers.Resource.Instantiate("UI_Joystick");
        joystickObject.name = "@UI_Joystick";
        
        List<int> normalList = Managers.Data.StageDic[stageId].NormalMonsterList;
        List<int> eliteList = Managers.Data.StageDic[stageId].EliteMonsterList;
        List<int> bossList = Managers.Data.StageDic[stageId].BossMonsterList;

        // TODO: Elite 및 Boss 맵 몬스터 스폰 로직은 추후에 개발 예정입니다 ㅎㅎ
        switch (nodeType)
        {
            case 0:
                StartCoroutine(SpawnNormalMonsters(normalList));
                break;
            case 1:
                StartCoroutine(SpawnNormalMonsters(eliteList));
                break;
            case 2:
                StartCoroutine(SpawnNormalMonsters(bossList));
                break;
        }
    }

    private IEnumerator SpawnNormalMonsters(List<int> monsterIds)
    {
        while (true)
        {
            for (int i = 0; i < PER_SEC_MOSTER_GENERATION; i++)
            {
                Vector3 randomPosition = GetRandomPositionOutsidePlayerRadius();
                GameObject target = Managers.Resource.Load<GameObject>("Target");
                target = Managers.Pool.Pop(target);
                target.transform.position = randomPosition;

                yield return new WaitForSeconds(TARGET_SPAWN_TIME);

                Managers.Pool.Push(target);
    
                int randomIndex = Random.Range(0, monsterIds.Count);
                int randomMonsterId = monsterIds[randomIndex];
                string randomMonsterPrefabName = Managers.Data.MonsterDic[randomMonsterId].PrefabName;
    
                MonsterController mc = Managers.Object.Spawn<MonsterController>(randomPosition, randomMonsterPrefabName);
                MonsterData monsterData = Managers.Data.MonsterDic[1];
                mc.InitMonster(monsterData);
            }

            yield return new WaitForSeconds(MONSTER_SPAWN_INTERVAL);
        }
    }

    private Vector3 GetRandomPositionOutsidePlayerRadius()
    {
        Vector3 playerPosition = Managers.Object.Player.transform.position;
        float playerRadius = 4f;
        Vector3 randomPosition;

        do
        {
            randomPosition = new Vector3(Random.Range(-8f, 8f), Random.Range(-8f, 8f), 0f);
        } while (Vector3.Distance(playerPosition, randomPosition) <= playerRadius);

        return randomPosition;
    }

    public override void Clear()
    {
    }
}