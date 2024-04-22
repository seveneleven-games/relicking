using System.Collections;
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

        GameObject map = Managers.Resource.Instantiate("BaseMap");
        map.transform.position = Vector3.zero;
        map.name = "@BaseMap";

        PlayerController pc = Managers.Object.Spawn<PlayerController>(Vector3.zero, Managers.Resource.Load<GameObject>("Player2"));
        Data.PlayerData playerData = Managers.Data.PlayerDic[2];
        pc.InitPlayer(playerData);

        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = pc;
        
        GameObject joystickObject = Managers.Resource.Instantiate("UI_Joystick");
        joystickObject.name = "@UI_Joystick";

        StartCoroutine(SpawnMonsters());
        
        return true;
    }
    
    private IEnumerator SpawnMonsters()
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
                MonsterController mc1 = Managers.Object.Spawn<MonsterController>(randomPosition, Managers.Resource.Load<GameObject>("Monster1"));
                Data.MonsterData monsterData = Managers.Data.MonsterDic[1];
                mc1.InitMonster(monsterData);
            
                Vector3 randomPosition2 = GetRandomPositionOutsidePlayerRadius();
                target = Managers.Resource.Load<GameObject>("Target");
                target = Managers.Pool.Pop(target);
                target.transform.position = randomPosition2;
            
                yield return new WaitForSeconds(0.5f);
            
                Managers.Pool.Push(target);
                MonsterController mc2 = Managers.Object.Spawn<MonsterController>(randomPosition2, Managers.Resource.Load<GameObject>("Monster2"));
                Data.MonsterData monster2Data = Managers.Data.MonsterDic[2];
                mc2.InitMonster(monster2Data);
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
