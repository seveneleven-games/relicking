using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    private Vector2 _moveDir = Vector2.zero;

    public int PlayerId { get; private set; }
    public string Name { get; private set; }
    public int Atk { get; private set; }
    public float CritRate { get; private set; }
    public float CritDmgRate { get; private set; }
    public float CoolDown { get; private set; }

    private int playerGold = INITIAL_GOLD;

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
    public List<int> _playerSkillList;

    public List<int> PlayerSkillList
    {
        get { return _playerSkillList; }
        set
        {
            OnPlayerSkillAdded?.Invoke(_playerSkillList);
            _playerSkillList = value;
        }
    }

    public List<int> PlayerRelicList { get; private set; }

    private Transform _indicator;

    private List<Coroutine> _skillCoroutines = new List<Coroutine>();

    private bool isSkillsActive = false;
    public bool IsSkillsActive => isSkillsActive;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Player;
        CreatureState = ECreatureState.Idle;

        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        PlayerSkillList = new List<int>(new int[6]);
        PlayerRelicList = new List<int>(new int[6]);

        AddSkill(3, 0);
        AddSkill(13, 1);
        AddSkill(22, 2);
        AddSkill(33, 3);
        AddSkill(42, 4);
        AddSkill(52, 5);

        // 보는 방향 정해주는 더미 오브젝트
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
        Hp = MaxHp;
        Atk = data.Atk;
        Speed = data.Speed;
        CritRate = data.CritRate;
        CritDmgRate = data.CritDmgRate;
        CoolDown = data.CoolDown;
    }

    private void Update()
    {
        Vector3 dir = _moveDir * Time.deltaTime * Speed;
        
        transform.TranslateEx(dir);

        if (_moveDir != Vector2.zero)
        {
            _indicator.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-dir.x, dir.y) * 180 / Mathf.PI);
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
            GoldController gc = collision.GetComponent<GoldController>();

            int goldValue = gc.GoldValue;

            PlayerGold += goldValue;

            UpdateRemainGoldText();

            Destroy(collision.gameObject);

            Debug.Log($"획득한 골드: {goldValue}, 현재 골드 량: {PlayerGold}");
        }
    }

    public void UpdateRemainGoldText()
    {
        UI_InGamePopup popup = Managers.UI.GetPopupUI<UI_InGamePopup>();
        if (popup != null)
            popup.UpdateRemainGoldText(PlayerGold);
    }

    public override void OnDamaged(BaseController attacker, int damage)
    {
        base.OnDamaged(attacker, damage);
        UI_World.Instance.UpdatePlayerHealth(Hp, MaxHp);
        Debug.Log($"OnDamaged ! {Hp}");
    }

    protected override void OnDead()
    {
        base.OnDead();
        CreatureState = ECreatureState.Dead;

        // TODO: Game 종료 씬으로~
        Managers.Scene.LoadScene(EScene.LobbyScene);
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

    IEnumerator CoStartSkill(int skillId)
    {
        SkillData skillData = Managers.Data.SkillDic[skillId];
        WaitForSeconds coolTimeWait = new WaitForSeconds(skillData.CoolTime);
        while (true)
        {
            yield return coolTimeWait;
            switch (skillData.PrefabName)
            {
                case "EnergyBolt":

                    int ebProjectileNum = skillData.ProjectileNum;
                    float ebSpreadAngle = 30f;

                    for (int i = 0; i < ebProjectileNum; i++)
                    {
                        EnergyBoltController ebc =
                            Managers.Object.Spawn<EnergyBoltController>(transform.position, skillId);

                        float angle;
                        if (ebProjectileNum == 1)
                            angle = 0f;
                        else
                        {
                            float offsetAngle = (i - (ebProjectileNum - 1) * 0.5f) *
                                                (ebSpreadAngle / (ebProjectileNum - 1));
                            angle = offsetAngle * Mathf.Deg2Rad;
                        }

                        Vector3 moveDirection = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg) * _indicator.up;
                        ebc.SetMoveDirection(moveDirection);
                    }

                    break;

                case "IceArrow":
                    int iaProjectileNum = skillData.ProjectileNum;

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

                    List<MonsterController> targetMonsters = nearbyMonsters.Take(iaProjectileNum).ToList();

                    for (int i = 0; i < targetMonsters.Count; i++)
                    {
                        MonsterController targetMonster = targetMonsters[i];

                        IceArrowController iac = Managers.Object.Spawn<IceArrowController>(transform.position, skillId);

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
                    int pfProjectileNum = skillData.ProjectileNum;
                    List<Vector3> installedPositions = new List<Vector3>();

                    for (int i = 0; i < pfProjectileNum; i++)
                    {
                        Vector3 randomPos;
                        do
                        {
                            float randomX = UnityEngine.Random.Range(-6f, 6f);
                            float randomY = UnityEngine.Random.Range(-6f, 6f);
                            randomPos = new Vector3(randomX, randomY, 0f);
                        } while (installedPositions.Any(pos => Vector3.Distance(pos, randomPos) < 2f));

                        PoisonFieldController pfc = Managers.Object.Spawn<PoisonFieldController>(randomPos, skillId);
                        pfc.SetOwner(this);

                        installedPositions.Add(randomPos);
                    }

                    break;

                case "WindCutter":

                    int wcProjectileNum = skillData.ProjectileNum;
                    float wcSpreadAngle = 30f;

                    for (int i = 0; i < wcProjectileNum; i++)
                    {
                        WindCutterController wcc =
                            Managers.Object.Spawn<WindCutterController>(transform.position, skillId);

                        float angle;
                        if (wcProjectileNum == 1)
                            angle = 0f;
                        else
                        {
                            float offsetAngle = (i - (wcProjectileNum - 1) * 0.5f) *
                                                (wcSpreadAngle / (wcProjectileNum - 1));
                            angle = offsetAngle * Mathf.Deg2Rad;
                        }

                        Vector3 moveDirection = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg) * _indicator.up;
                        wcc.SetMoveDirection(moveDirection);
                    }

                    break;
                
                case "FrozenHeart":
                    int fzProjectileNum = skillData.ProjectileNum;
                    float fzDistance = 2f;
                    for (int i = 0; i < fzProjectileNum; i++)
                    {
                        float degree = 360f / fzProjectileNum * i;
                        Vector3 spawnPos = transform.position;
                        FrozenHeartController fhc = Managers.Object.Spawn<FrozenHeartController>(spawnPos, skillId);
                        fhc.SetOwner(this);
                        fhc.InitSkill(skillId, fzDistance, degree);
                    }
                    yield break;
            }
        }
    }

    public void AddSkill(int addSkillId, int slotNum)
    {
        // 이전에 배운스킬 레벨업이면 해당 슬롯에 덮어씌움
        for (int i = 0; i < PlayerSkillList.Count; i++)
        {
            if (PlayerSkillList[i] == addSkillId - 1)
            {
                PlayerSkillList[i] = addSkillId;
                return;
            }
        }

        // 배운적 없는 스킬이면 가장 처음으로 빈 슬롯에 넣어줌
        for (int i = 0; i < PlayerSkillList.Count; i++)
        {
            if (PlayerSkillList[i] == 0)
            {
                PlayerSkillList[i] = addSkillId;
                return;
            }
        }

        // 이전에 배운스킬도 아닌데 빈슬롯도없으면 로그에러띄워줌
        Debug.LogError("스킬 넣는게 잘못됐어요");
    }

    #endregion
}