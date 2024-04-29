using System.Collections;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEditor.iOS;
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

    public TemplateData _templateData;
    private int _classId;
    private PlayerController _player;

    // 노드 정보
    private UI_NodeMapPopup _nodeMap;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.GameScene;

        _nodeMap = Managers.UI.ShowPopupUI<UI_NodeMapPopup>();
        Debug.Log("GameScene Init이 여러번 도나??");
        _nodeMap.OnEnterNode += StartGame;

        _templateData = Resources.Load<TemplateData>("GameTemplateData");
        _classId = _templateData.TemplateIds[1];

        _player = Managers.Object.Spawn<PlayerController>(Vector3.zero, _classId);

        // TODO: 노드맵 UI에서 게임을 시작해야 한다. 


        return true;
    }

    #region 노드 정보에 맞는 몬스터 스폰

    /* 스테이지 정보와 노드맵 템플릿 아이디를 기반으로 노드 정보 배열 관리
     * 1. 스테이지 정보(Lobby에서 제공)와 노드맵 템플릿 아이디(NodeMapPopup에서 제공)로 적합한 노드 배열 가져와서 보관
     * 2. 노드 번호를 받아와서 노드 정보(스폰 몬스터 종류, 추가적으로 배경 이미지 정도) 반영 후 스폰
     */

    #endregion

    public void StartGame(int nodeNo, bool isBossNode)
    {
        NodeMapData nodeMapData = Managers.Data.NodeMapDic[_templateData.TempNodeNum];

        NodeData node = nodeMapData.NodeList[nodeNo];

        GameObject map = Managers.Resource.Instantiate(node.MapPrefabName);
        map.transform.position = Vector3.zero;
        map.name = "@BaseMap";

        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = _player;

        GameObject joystickObject = Managers.Resource.Instantiate("UI_Joystick");
        joystickObject.name = "@UI_Joystick";

        // 여기에 팝업 닫는 함수 있어야 할 것.
        _nodeMap.ClosePopupUI();
        Debug.Log($"{nodeNo}번 노드 진입! 게임 시작!! 보스노드여부 : {isBossNode}");

        int nodeType = 0;
        List<int> normalMonsters = new List<int>();
        List<int> eliteMonsters = new List<int>();
        List<int> bossMonsters = new List<int>();

        foreach (int monsterId in node.MonsterList)
        {
            MonsterData monsterData = Managers.Data.MonsterDic[monsterId];

            switch (monsterData.MonsterType)
            {
                case 0:
                    normalMonsters.Add(monsterId);
                    break;
                case 1:
                    eliteMonsters.Add(monsterId);
                    break;
                case 2:
                    bossMonsters.Add(monsterId);
                    break;
            }
        }

        if (bossMonsters.Count > 0)
        {
            nodeType = 2;
        }
        else if (eliteMonsters.Count > 0)
        {
            nodeType = 1;
        }

        switch (nodeType)
        {
            case 0:
                StartCoroutine(SpawnNormalMonsters(normalMonsters));
                break;
            case 1:
                StartCoroutine(SpawnEliteMonsters(normalMonsters, eliteMonsters));
                break;
            case 2:
                StartCoroutine(SpawnBossMonsters(normalMonsters, eliteMonsters, bossMonsters));
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

                MonsterController mc = Managers.Object.Spawn<MonsterController>(randomPosition, randomMonsterId);
                mc.InitMonster(randomMonsterId);
            }

            yield return new WaitForSeconds(MONSTER_SPAWN_INTERVAL);
        }
    }

    private IEnumerator SpawnEliteMonsters(List<int> normalMonsterIds, List<int> eliteMonsterIds)
    {
        Vector3 eliteSpawn = new Vector3(0, -4, 0);
        Managers.Object.Spawn<MonsterController>(eliteSpawn, eliteMonsterIds[0]);
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

                int randomIndex = Random.Range(0, normalMonsterIds.Count);
                int randomMonsterId = normalMonsterIds[randomIndex];

                MonsterController mc = Managers.Object.Spawn<MonsterController>(randomPosition, randomMonsterId);
                mc.InitMonster(randomMonsterId);
            }

            yield return new WaitForSeconds(MONSTER_SPAWN_INTERVAL);
        }
    }

    private IEnumerator SpawnBossMonsters(List<int> normalMonsterIds, List<int> eliteMonsterIds,
        List<int> boosMonsterIds)
    {
        Vector3 bossSpawn = new Vector3(0, 4, 0);
        Managers.Object.Spawn<MonsterController>(bossSpawn, boosMonsterIds[0]);
        Vector3 eliteSpawn = new Vector3(0, -4, 0);
        Managers.Object.Spawn<MonsterController>(eliteSpawn, eliteMonsterIds[0]);
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

                int randomIndex = Random.Range(0, normalMonsterIds.Count);
                int randomMonsterId = normalMonsterIds[randomIndex];

                MonsterController mc = Managers.Object.Spawn<MonsterController>(randomPosition, randomMonsterId);
                mc.InitMonster(randomMonsterId);
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