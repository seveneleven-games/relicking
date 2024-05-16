using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Util;
public static class Define
{
    public enum EScene
    {
        Unknown,
        TitleScene,
        LobbyScene,
        GameScene,
        LoginScene,
        IdleScene,
    }

    public enum EUIEvent
    {
        Click,
        Pressed,
        PointerDown,
        PointerUp,
        BeginDrag,
        Drag,
        EndDrag,
    }

    public enum ESound
    {
        Bgm,
        Effect,
        Max,
    }

    public enum EObjectType
    {
        None,
        Player,
        Monster,
        Skill,
        Env,
        Damage
    }

    public enum ECreatureState
    {
        None = 0,
        Idle = 1,
        Move = 2,
        Dead = 3
    }

    public enum ESkillType
    {
        None,
        EnergyBolt,
        IceArrow,
        ElectronicField,
        PoisonField,
        EliteMonsterProjectile,
        WindCutter,
        FrozenHeart,
        MeteorHit,
        Meteor,
        MeteorShadow,
        ChainLightning,
        Shuriken,
        StormBlade,
        BossMonsterThorn
    }

    public enum EJoystickState
    {
        PointerDown,
        PointerUp,
        Drag
    }
    
    #region 스태틱 변수 (테스트용 다수)

    //todo(전지환) : NodeMapPopup 임시 데이터 (점검 후 삭제 필요)
    public static int STAGE_NO = 1;
    public static string STAGE_NODEMAP_NAME = "UI_Stage1NodeMap_01";
    public static string STAGE_BG_NAME = "Stage1BG.sprite";
    
    //todo(전지환) : StorePopup 임시 데이터
    public static int INITIAL_GOLD = 250;
    public static int INITIAL_REROLL_COST = 0;
    
    //todo(전지환) : 총 스킬 개수 
    public static int TOTAL_PLAYER_SKILL_NUMBER = 10;
    public static int INITIAL_SKILL_COST = 100;
    
    public static string BASE_URI = "https://k10d211.p.ssafy.io/api/";
    
    //unity speed 관련 계수
    public static int MIN_SPEED = 3;
    public static int SPEED_RATE = 500;
    public static int RANGE = 7;

    //난이도 몬스터 스탯 상관계수
    public static float DIFFICULTY_COEFFICIENT = 0.1f;
    
    public static int ADDRESSABLE_COUNT = 163;
    
    //포이즌 필드 생성 위치 리스트
    public static Vector3[] POISON_FIELD_POS = new Vector3[] {
        new Vector3(2.7f, 0.0f, 0f),
        new Vector3(1.909f, 1.909f, 0f),
        new Vector3(0.0f, 2.7f, 0f),
        new Vector3(-1.909f, 1.909f, 0f),
        new Vector3(-2.7f, 0.0f, 0f),
        new Vector3(-1.909f, -1.909f, 0f),
        new Vector3(0.0f, -2.7f, 0f),
        new Vector3(1.909f, -1.909f, 0f)
    };

    public static string VERSION = "1.0.0";

    #endregion

}

public static class RelicUIColors
{
    // 배경 색깔
    public static readonly Color GradeSSS = HexToColor("FF0000");
    public static readonly Color GradeS = HexToColor("FBFF31");
    public static readonly Color GradeA = HexToColor("A507CC");
    public static readonly Color GradeB = HexToColor("2641CB");
    public static readonly Color GradeC = HexToColor("696969");
}


