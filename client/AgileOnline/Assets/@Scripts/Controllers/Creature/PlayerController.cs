using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using static Define;
using Random = UnityEngine.Random;


public class PlayerController : CreatureController
{
    private Vector2 _moveDir = Vector2.zero;

    public int PlayerId { get; private set; }
    public string Name { get; private set; }
    public float Atk { get; private set; }
    public float CritRate { get; private set; }
    public float CritDmgRate { get; private set; }
    public float CoolDown { get; private set; }
    public float CoinBonus {get; private set; }
    
    private int playerGold = INITIAL_GOLD;

    private bool _isPlayerFrozen = false;
    private float _freezeDuration = 1.5f;

    public void FreezePlayerMovement()
    {
        StartCoroutine(FreezePlayerMovementCoroutine());
    }
    
    private IEnumerator FreezePlayerMovementCoroutine()
    {
        _isPlayerFrozen = true;
        GameObject restraintObj = Managers.Resource.Instantiate("Restraint");
        restraintObj.transform.position = transform.position + new Vector3(0, 0.2f, 0);
        float elapsedTime = 0f;

        while (elapsedTime < _freezeDuration)
        {
            _moveDir = Vector2.zero;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(restraintObj);
        _isPlayerFrozen = false;
    }

    // 스킬풀 플래그 변수
    public bool SkillPoolFixedInit { get; set; } = false;

    // 보스킬 플래그 변수
    public event Action OnBossKilled;
    private bool _isBossKilled = false;

    public bool IsBossKilled
    {
        get {return _isBossKilled; }
        set
        {
            _isBossKilled = value;
            OnBossKilled?.Invoke();
        }
    }
    
    public int PlayerGold
    {
        get { return playerGold; }
        set
        {
            playerGold = value;
            UpdateRemainGoldText();
        }
    }

    public event Action<List<int>> OnPlayerSkillAdded;
    private List<int> _playerSkillList;

    public List<int> PlayerSkillList
    {
        get { return _playerSkillList; }
        set { _playerSkillList = value; }
    }

    public List<int> PlayerRelicList { get; private set; }

    private Transform _indicator;

    private List<Coroutine> _skillCoroutines = new List<Coroutine>();

    private bool isSkillsActive = false;
    
    private GameScene _gameScene;

    private TemplateData _templateData;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        
        _gameScene = FindObjectOfType<GameScene>();

        ObjectType = EObjectType.Player;
        CreatureState = ECreatureState.Idle;

        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        PlayerSkillList = new List<int>(new int[6]);
        PlayerRelicList = new List<int>(new int[6]);
        
        GameObject indicatorObject = new GameObject("Indicator");
        indicatorObject.transform.SetParent(transform);
        indicatorObject.transform.localPosition = Vector3.zero;
        indicatorObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        _indicator = indicatorObject.transform;

        StartSkills();

        DontDestroyOnLoad(this);

        return true;
    }

    // TODO: InitPlayer시 RelicList도 같이 받아야함
    public void InitPlayer(int templateId)
    {
        PlayerData data = Managers.Data.PlayerDic[templateId];

        PlayerId = data.PlayerId;
        Name = data.Name;
        MaxHp = data.MaxHp;
        Atk = data.Atk;
        Speed = data.Speed;
        CritRate = data.CritRate;
        CritDmgRate = data.CritDmgRate;
        CoolDown = data.CoolDown;
        CoinBonus = data.ExtraGold;
        
        _templateData = Resources.Load<TemplateData>("GameTemplateData");
        
        int[] relicIds = _templateData.EquipedRelicIds;
        foreach (int relicId in relicIds)
        {
            RelicData relicData = Managers.Data.RelicDic[relicId];
            Atk += relicData.Atk;
            MaxHp += relicData.MaxHp;
            Speed += relicData.Speed;
            CoinBonus += relicData.ExtraGold;
            CritRate += relicData.CritRate;
            CritDmgRate += relicData.CritDmgRate;
            CoolDown -= relicData.CoolTime / 100f;
        }
        
        Hp = MaxHp;

        if (CoolDown < 0.1)
            CoolDown = 0.1f;
        
    }

    private void Update()
    {
        if (!_isPlayerFrozen)
        {
            Vector3 dir = _moveDir * (Time.deltaTime * Util.UnitySpeed(Speed));
            transform.TranslateEx(dir);

            if (_moveDir != Vector2.zero)
            {
                _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
            }
        }

        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        _moveDir = dir;
    }

    private void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case EJoystickState.PointerDown:
                CreatureState = ECreatureState.Move;
                break;
            case EJoystickState.Drag:
                break;
            case EJoystickState.PointerUp:
                CreatureState = ECreatureState.Idle;
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MonsterController target = collision.gameObject.GetComponent<MonsterController>();
        if (target == null)
            return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gold"))
        {
            Managers.Sound.Play(ESound.Effect,"Reroll");
            
            GoldController gc = collision.GetComponent<GoldController>();

            int goldValue = (int) (gc.GoldValue * (1 + CoinBonus) + 0.5f);

            PlayerGold += goldValue;

            UpdateRemainGoldText();

            Destroy(collision.gameObject);
        }
    }

    public void UpdateRemainGoldText()
    {
        UI_InGamePopup popup = Managers.UI.GetPopupUI<UI_InGamePopup>();
        if (popup != null)
            popup.UpdateRemainGoldText(PlayerGold);
    }

    public override bool OnDamaged(BaseController attacker,ref float damage)
    {
        Managers.Sound.Play(ESound.Effect,"FemaleHurt",0.3f);
        
        base.OnDamaged(attacker, ref damage);
        UI_World.Instance.UpdatePlayerHealth(Hp, MaxHp);
        return true;
    }

    #region Skill

    public void StartSkills()
    {
        if (isSkillsActive) return;

        StopSkills();

        foreach (int skillId in PlayerSkillList)
        {
            if (skillId > 0)
            {
                Coroutine skillCoroutine = StartCoroutine(CoStartSkill(skillId));
                _skillCoroutines.Add(skillCoroutine);
            }
        }

        isSkillsActive = true;
    }

    public void StopSkills()
    {
        ElectronicFieldController[] electronicFields = FindObjectsOfType<ElectronicFieldController>();
        foreach (ElectronicFieldController ef in electronicFields)
        {
            if (ef._owner == this)
            {
                Managers.Object.Despawn(ef);
            }
        }

        FrozenHeartController[] frozenHearts = FindObjectsOfType<FrozenHeartController>();
        foreach (FrozenHeartController fh in frozenHearts)
        {
            if (fh._owner == this)
            {
                Managers.Object.Despawn(fh);
            }
        }

        foreach (Coroutine coroutine in _skillCoroutines)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        _skillCoroutines.Clear();
        isSkillsActive = false;
    }

    private int[] _poisonFieldPos;
    
    IEnumerator CoStartSkill(int skillId)
    {
        SkillData skillData = Managers.Data.SkillDic[skillId];
        WaitForSeconds coolTimeWait = new WaitForSeconds(skillData.CoolTime * CoolDown);

        while (true)
        {
            yield return coolTimeWait;
            switch (skillData.PrefabName)
            {
                case "EnergyBolt":
                    float ebSpreadAngle = 30f;

                    for (int i = 0; i < skillData.ProjectileNum; i++)
                    {
                        EnergyBoltController ebc =
                            Managers.Object.Spawn<EnergyBoltController>(transform.position, skillId);
                        ebc.SetOwner(this);

                        float angle;
                        if (skillData.ProjectileNum == 1)
                            angle = 0f;
                        else
                        {
                            float offsetAngle = (i - (skillData.ProjectileNum - 1) * 0.5f) *
                                                (ebSpreadAngle / (skillData.ProjectileNum - 1));
                            angle = offsetAngle * Mathf.Deg2Rad;
                        }

                        Vector3 moveDirection = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg) * _indicator.up;
                        ebc.SetMoveDirection(moveDirection);
                    }

                    break;

                case "IceArrow":
                    List<MonsterController> nearbyMonsters = new List<MonsterController>();
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 15f);
                    foreach (Collider2D collider in colliders)
                    {
                        MonsterController monster = collider.GetComponent<MonsterController>();
                        if (monster != null)
                            nearbyMonsters.Add(monster);
                    }

                    nearbyMonsters.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
                        .CompareTo(Vector3.Distance(transform.position, b.transform.position)));

                    List<MonsterController> targetMonsters = nearbyMonsters.Take(skillData.ProjectileNum).ToList();

                    for (int i = 0; i < targetMonsters.Count; i++)
                    {
                        MonsterController targetMonster = targetMonsters[i];

                        IceArrowController iac = Managers.Object.Spawn<IceArrowController>(transform.position, skillId);
                        iac.SetOwner(this);

                        Vector3 direction = (targetMonster.transform.position - transform.position).normalized;
                        iac.SetMoveDirection(direction);
                    }

                    break;

                case "ElectronicField":
                    ElectronicFieldController efc =
                        Managers.Object.Spawn<ElectronicFieldController>(transform.position, skillId);
                    efc.SetOwner(this);
                    yield break;

                case "PoisonField":
                    // List<Vector3> installedPositions = new List<Vector3>();
                    //
                    // for (int i = 0; i < skillData.ProjectileNum; i++)
                    // {
                    //     Vector3 randomPos;
                    //     do
                    //     {
                    //         float randomX = transform.position.x + Random.Range(-2f, 2f);
                    //         float randomY = transform.position.y + Random.Range(-2f, 2f);
                    //         randomPos = new Vector3(randomX, randomY, 0f);
                    //     } while (installedPositions.Any(pos => Vector3.Distance(pos, randomPos) < 2f));
                    //
                    //     PoisonFieldController pfc = Managers.Object.Spawn<PoisonFieldController>(randomPos, skillId);
                    //     pfc.SetOwner(this);
                    //
                    //     installedPositions.Add(randomPos);
                    // }
                    StartCoroutine(SpawnPoisonField(skillData, skillId));
                    
                    break;

                case "WindCutter":
                    float wcSpreadAngle = 30f;

                    for (int i = 0; i < skillData.ProjectileNum; i++)
                    {
                        WindCutterController wcc =
                            Managers.Object.Spawn<WindCutterController>(transform.position, skillId);
                        wcc.SetOwner(this);

                        float angle;
                        if (skillData.ProjectileNum == 1)
                            angle = 0f;
                        else
                        {
                            float offsetAngle = (i - (skillData.ProjectileNum - 1) * 0.5f) *
                                                (wcSpreadAngle / (skillData.ProjectileNum - 1));
                            angle = offsetAngle * Mathf.Deg2Rad;
                        }

                        Vector3 moveDirection = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg) * _indicator.up;
                        wcc.SetMoveDirection(moveDirection);
                    }

                    break;

                case "FrozenHeart":
                    float fzDistance = 2f;
                    for (int i = 0; i < skillData.ProjectileNum; i++)
                    {
                        float degree = 360f / skillData.ProjectileNum * i;
                        Vector3 spawnPos = transform.position;
                        FrozenHeartController fhc = Managers.Object.Spawn<FrozenHeartController>(spawnPos, skillId);
                        fhc.SetOwner(this);
                        fhc.InitSkill(skillId, fzDistance, degree);
                    }

                    yield break;
                
                case "Meteor":
                    for (int i = 0; i < skillData.ProjectileNum; i++)
                    {
                        Debug.Log("내 포지션 : " + transform.position);
                        Vector3 shadowSpawnPos = new Vector3(
                            Mathf.Clamp(transform.position.x + Random.Range(-3f, 3f), -8f, 8f), 
                            Mathf.Clamp(transform.position.y + Random.Range(-3f, 3f), -8f, 8f), 
                            0f);
                        Debug.Log("메테오 포지션 : " + shadowSpawnPos);
                        Managers.Object.Spawn<MeteorShadowController>(shadowSpawnPos, skillId);
                        
                        Vector3 meteorSpawnPos = shadowSpawnPos + new Vector3(0f, 5.5f, 0f);
                        Managers.Object.Spawn<MeteorController>(meteorSpawnPos, skillId);
                        
                        Vector3 hitSpawnPos = shadowSpawnPos;
                        StartCoroutine(DelayedMeteorHit(hitSpawnPos, skillId, 2f));
                    }
                    break;
                
                case "ChainLightning":
                    for (int i = 0; i < skillData.ProjectileNum; i++)
                    {
                        float damageFloat = Managers.Data.SkillDic[skillId].Damage;
                        float realDamage = (damageFloat * Atk);
                        
                        List<MonsterController> chainMonsters = new List<MonsterController>();
                        Collider2D[] chainColliders = Physics2D.OverlapCircleAll(transform.position, 10f);
                        foreach (Collider2D collider in chainColliders)
                        {
                            MonsterController monster = collider.GetComponent<MonsterController>();
                            if (monster != null)
                                chainMonsters.Add(monster);
                        }
                        chainMonsters.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
                            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
                        int numOfBounce = Mathf.Min(3, chainMonsters.Count);
                        List<MonsterController> chainTargetMonsters = chainMonsters.Take(numOfBounce).ToList();

                        Vector3 startPoint = transform.position;
                        foreach (MonsterController monster in chainTargetMonsters)
                        {
                            monster.OnDamaged(this, ref realDamage);
                            Vector3 endPoint = monster.transform.position;
                            Managers.Object.Spawn<ChainLightningController>(startPoint, skillId, new object[] { startPoint, endPoint });
                            startPoint = endPoint;
                        }
                    }
                    break;
                
                case "Shuriken":
                    for (int i = 0; i < skillData.ProjectileNum; i++)
                    {
                        ShurikenController skc = Managers.Object.Spawn<ShurikenController>(transform.position, skillId);
                        skc.SetOwner(this);
                        float randomAngle = Random.Range(0f, 360f);
                        Vector3 moveDirection = Quaternion.Euler(0f, 0f, randomAngle * Mathf.Rad2Deg) * _indicator.up;
                        
                        skc.SetMoveDirection(moveDirection);
                    }
                    break;
                
                case "StormBlade":
                    StartCoroutine(ShootStormBlades(skillData.ProjectileNum, skillId));
                    break;
            }
        }
    }

    private IEnumerator SpawnPoisonField(SkillData skillData, int skillId)
    {
        _poisonFieldPos = Extension.RandomInts(skillData.ProjectileNum, 0, 8);

        for (int i = 0; i < skillData.ProjectileNum; i++)
        {
            PoisonFieldController pfc = Managers.Object.Spawn<PoisonFieldController>(
                transform.position + POISON_FIELD_POS[_poisonFieldPos[i]], skillId);
            pfc.SetOwner(this);

            yield return new WaitForSeconds(0.2f);;
        }
    }
    
    private IEnumerator ShootStormBlades(int projectileNum, int skillId)
    {
        for (int i = 0; i < projectileNum; i++)
        {
            StormBladeController sbc =
                Managers.Object.Spawn<StormBladeController>(transform.position, skillId);
            sbc.SetOwner(this);

            float angle = 0;

            Vector3 moveDirection = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg) * _indicator.up;
            sbc.SetMoveDirection(moveDirection);

            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private IEnumerator DelayedMeteorHit(Vector3 spawnPos, int skillId, float delay)
    {
        yield return new WaitForSeconds(delay);
        MeteorHitController mhc = Managers.Object.Spawn<MeteorHitController>(spawnPos, skillId);
        mhc.SetOwner(this);
        mhc.InitSkill(skillId);
    }


    public List<int> AddSkill(int addSkillId)
    {
        // 이전에 배운스킬 레벨업이면 해당 슬롯에 덮어씌움
        for (int i = 0; i < PlayerSkillList.Count; i++)
        {
            if (PlayerSkillList[i] == addSkillId - 1)
            {
                PlayerSkillList[i] = addSkillId;
                Debug.Log($"스킬 레벨 업!");
                return PlayerSkillList;
            }
        }

        // 배운적 없는 스킬이면 가장 처음으로 빈 슬롯에 넣어줌
        for (int i = 0; i < PlayerSkillList.Count; i++)
        {
            if (PlayerSkillList[i] == 0)
            {
                PlayerSkillList[i] = addSkillId;
                Debug.Log($"스킬 추가 !!");
                return PlayerSkillList;
            }
        }

        // 이전에 배운스킬도 아닌데 빈슬롯도없으면 로그에러띄워줌
        Debug.LogError("스킬 넣는게 잘못됐어요");
        return PlayerSkillList;
    }

    #endregion

    #region OnDead -> Lobby

    public override void OnDead()
    {
        base.OnDead();
        
        Managers.UI.ShowPopupUI<UI_DeadPopup>();
        
        CreatureState = ECreatureState.Dead;
        
        PlayerController player = Managers.Object.Player;
        if (player != null)
        {
            player.gameObject.SetActive(false);
            Managers.Object.Player = null;
        }
        StopAllCoroutines();
        // 리소스 정리
        CleanupResources();
        
        if (_gameScene != null)
        {
            _gameScene.InvokeGameOverEvent();
        }
    }
    
    private void CleanupResources()
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

    public void DestroyObjects(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            Managers.Resource.Destroy(obj);
        }
    }

    #endregion
}