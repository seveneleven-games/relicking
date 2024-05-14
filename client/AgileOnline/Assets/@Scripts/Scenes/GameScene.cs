using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Define;
using Random = UnityEngine.Random;

[Serializable]
public class ClearDataReq
{
    public int eliteKill;
    public int normalKill;
    public int stage;
    public int difficulty;
    public List<Skill> skillList;
}

[Serializable]
public class Skill
{
    public int slot;
    public int skillNo;
    public int level;
}

[Serializable]
public class ClearDataRes
{
    public int status;
    public string message;
    public bool data;
}

public class GameScene : BaseScene
{
    //todo(전지환) : 스폰 관련 수치 원복
    /*
     * private const float MONSTER_SPAWN_INTERVAL = 5f;
     * private const int PER_SEC_MOSTER_GENERATION = 20;
     * private const float TARGET_SPAWN_TIME = 0.5f;
     * 
     */
    private const float MONSTER_SPAWN_INTERVAL = 2f;
    private const int PER_SEC_MOSTER_GENERATION = 40;
    private const float TARGET_SPAWN_TIME = 0.3f;
    private const int NORMAL_MONSTER = 0;
    private const int ELITE_MONSTER = 1;
    private const int BOSS_MONSTER = 2;

    public TemplateData _templateData;
    private int _classId;
    private PlayerController _player;

    // 노드 정보
    private UI_NodeMapPopup _nodeMap;
    private UI_StorePopup _store;
    private UI_InGamePopup _inGame;

    private Text timerText;
    private Coroutine _timerCoroutine;
    
    public event Action OnGameOverEvent;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.GameScene;

        _templateData = Resources.Load<TemplateData>("GameTemplateData");
        _classId = _templateData.SelectedClassId;
        
        _player = Managers.Object.CreatePlayer(_classId);

        _player.StopSkills();
        _player.OnBossKilled += OnGameClear;
        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = _player;

        _nodeMap = Managers.UI.ShowPopupUI<UI_NodeMapPopup>();
        _nodeMap.OnEnterNode += StartGame;
        _store = InstantiateStore();
        
        // _inGame = Managers.UI.ShowPopupUI<UI_InGamePopup>();
        
        GameObject joystickObject = Managers.Resource.Instantiate("UI_Joystick");
        joystickObject.name = "@UI_Joystick";

        OnGameOverEvent += HandleGameOver;

        // TODO: 노드맵 UI에서 게임을 시작해야 한다. 

        
        

        return true;
    }

    void EnableNodeMap(int nodeNo)
    {
        _nodeMap.DataSync(nodeNo);
        _nodeMap.gameObject.SetActive(true);
    }
    
    void DisableNodeMap()
    {
        // 연관관계 모두 초기화
        
        _nodeMap.gameObject.SetActive(false);
    }

    UI_StorePopup InstantiateStore()
    {
        var store = Managers.UI.ShowPopupUI<UI_StorePopup>();
        store.DataSync(_player.PlayerSkillList);

        return store;
    }
    
    
    
    #region 노드 정보에 맞는 몬스터 스폰

    /* 스테이지 정보와 노드맵 템플릿 아이디를 기반으로 노드 정보 배열 관리
     * 1. 스테이지 정보(Lobby에서 제공)와 노드맵 템플릿 아이디(NodeMapPopup에서 제공)로 적합한 노드 배열 가져와서 보관
     * 2. 노드 번호를 받아와서 노드 정보(스폰 몬스터 종류, 추가적으로 배경 이미지 정도) 반영 후 스폰
     */

    #endregion

    // 진입한 노드 번호를 가지고 있을 변수
    private int _nodeNo;
    private static bool _isBossNode;

    public static bool IsBossNode
    {
        get { return _isBossNode; }
        set { _isBossNode = value; }
    }
    
    public void StartGame(int nodeNo, bool isBossNode)
    {
        // 클리어 이벤트 핸들링할 변수 전역화
        _nodeNo = nodeNo;
        _isBossNode = isBossNode;
        Debug.Log("지금 보스 노드인가요 ? : " + _isBossNode);
        
        // TODO: 팝업 관리 리팩토링 예정
        DisableNodeMap();
        
        _inGame = Managers.UI.ShowPopupUI<UI_InGamePopup>();
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name.StartsWith("Target"))
                Managers.Pool.Push(obj);
        }

        _player.GetComponent<CircleCollider2D>().enabled = true;
        _player.StartSkills();
        
        NodeMapData nodeMapData = Managers.Data.NodeMapDic[_templateData.TempNodeNum];
        NodeData node = nodeMapData.NodeList[nodeNo];
        
        GameObject map = Managers.Resource.Instantiate(node.MapPrefabName);
        map.transform.position = Vector3.zero;
        map.name = "@BaseMap";

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
                StartCoroutine(SpawnBossMonsters(normalMonsters,  bossMonsters));
                break;
        }

        if (_isBossNode)
        {
            _timerCoroutine = StartCoroutine(StartBossTimer(30f));
            // 인게임보스 사운드 넣기
            Managers.Sound.Play(Define.ESound.Bgm,"Bgm_InGameBoss");
            
        }
        else
        {
            // 인게임 사운드 넣기
            Managers.Sound.Play(Define.ESound.Bgm,"Bgm_InGame");
            _timerCoroutine = StartCoroutine(StartTimer(30f));
        }
    }
    
    private IEnumerator StartBossTimer(float duration)
    {
        float timer = duration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        _player.OnDead();
    }
    
    private IEnumerator StartTimer(float duration)
    {
        float timer = duration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        OnGameClear();
    }

    private void OnGameClear()
    {
        StopAllCoroutines();
        _inGame.ClosePopupUI();
        
        if (_isBossNode)
        {
            ClearServerCommunication();
            Managers.UI.ShowPopupUI<UI_ClearPopup>();
        }
        else
        {
            _player.GetComponent<CircleCollider2D>().enabled = false;
            EnableNodeMap(_nodeNo);
            _store = InstantiateStore();
        }
        
        #region 플레이어 설정
        // 플레이어 스킬 중지
        _player.StopSkills();
        
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name.StartsWith("@BaseMap"))
            {
                Debug.Log("맵 삭제");
                Managers.Resource.Destroy(obj);   
            }
        }
        
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            GameObject monsterPool = GameObject.Find("@Monsters");
            if (monsterPool != null)
            {
                foreach (Transform child in monsterPool.transform)
                {
                    GameObject monsterObj = child.gameObject;
                    MonsterController monsterController = monsterObj.GetComponent<MonsterController>();
                    if (monsterController != null)
                        Managers.Object.Despawn(monsterController);
                }
            }
            
            // 플레이어 위치 초기화
            Debug.Log("플레이어 위치 초기화 시킬게요");
            _player.transform.position = Vector3.zero;
        
            GameObject goldPool = GameObject.Find("@Golds");
            if (goldPool == null)
                return;
            
            if (monsterPool != null)
            {
                foreach (Transform child in goldPool.transform)
                {
                    GameObject goldObj = child.gameObject;
                    GoldController goldController = goldObj.GetComponent<GoldController>();
                    if (goldController != null)
                        Managers.Object.Despawn(goldController);
                }
            }
        }
        
        #endregion
    }

    void ClearServerCommunication()
    {
        Debug.Log("보스노드 클리어!");
        ClearDataReq clearDataReq = new ClearDataReq();
        clearDataReq.eliteKill = 0;
        clearDataReq.normalKill = 0;
        clearDataReq.stage = Int32.Parse(_nodeMap._stageNo);
        clearDataReq.difficulty = _templateData.Difficulty;
            
        List<Skill> skillList = _player.PlayerSkillList
            .Select((skillId, index) => new Skill
            {
                skillNo = skillId / 10,
                level = skillId % 10 == 0 ? 10 : skillId % 10,
                slot = index + 1
            })
            .ToList();

        clearDataReq.skillList = skillList;
            
        string clearJsonData = JsonUtility.ToJson(clearDataReq);

        StartCoroutine(
            Util.JWTPatchRequest("stages", clearJsonData, res =>
            {
                Debug.Log("Server Response: " + res);
                ClearDataRes clearDataRes = JsonUtility.FromJson<ClearDataRes>(res);
                if (clearDataRes != null && clearDataRes.status == 200)
                {
                    Debug.Log(clearDataRes.message);
                    
                    // 성공 했으면 데이터 갱신해주기 (로그인때만 스테이지 정보를 가져오므로)
                    Managers.Game.UpdateStageDifficulty(clearDataReq.stage, clearDataReq.difficulty);
                }
                else
                {
                    Debug.LogError("서버 응답 오류: " + res);
                }
            }));
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
                
                if (gameObject == null)
                    yield break;
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
                
                if (gameObject == null)
                    yield break;
            }

            yield return new WaitForSeconds(MONSTER_SPAWN_INTERVAL);
        }
    }

    private IEnumerator SpawnBossMonsters(List<int> normalMonsterIds, List<int> boosMonsterIds)
    {
        Vector3 bossSpawn = new Vector3(0, 4, 0);
        Managers.Object.Spawn<MonsterController>(bossSpawn, boosMonsterIds[0]);
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
                
                if (gameObject == null)
                    yield break;
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

    public static void SpawnBossMonsterSkill(Vector3 spawnLocation, int monsterId)
    {
        for (int i = 0; i < 10; i++)
            Managers.Object.Spawn<MonsterController>(spawnLocation, monsterId);
    }
    
    public void InvokeGameOverEvent()
    {
        OnGameOverEvent?.Invoke();
    }
    
    private void HandleGameOver()
    {
        OnGameOver();
    }
    
    private void OnGameOver()
    {
        StopAllCoroutines();
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }
    
    private void OnDestroy()
    {
        OnGameOverEvent -= HandleGameOver;
    }

    public override void Clear()
    {
    }
}