using System.Collections;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    private const float MONSTER_SPAWN_INTERVAL = 3f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.GameScene;

        StartGame(1, 1);

        return true;
    }

    public void StartGame(int stageId, int playerId)
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

        StartCoroutine(SpawnMonsters(stageData));
    }

    private IEnumerator SpawnMonsters(StageData stageData)
    {
        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector3 randomPosition = GetRandomPositionOutsidePlayerRadius();
                GameObject target = Managers.Resource.Load<GameObject>("Target");
                target = Managers.Pool.Pop(target);
                target.transform.position = randomPosition;

                yield return new WaitForSeconds(0.5f);

                Managers.Pool.Push(target);
                string normalMonsterPrefabName = stageData.NormalMonsterList[Random.Range(0, stageData.NormalMonsterList.Count)];
                MonsterController mc = Managers.Object.Spawn<MonsterController>(randomPosition, normalMonsterPrefabName);
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