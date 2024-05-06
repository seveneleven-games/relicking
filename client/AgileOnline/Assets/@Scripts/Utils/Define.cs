using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
        //todo(전지환) : 머지 전에 test씬 삭제하기
        TestNodeMapScene,
        TestStoreScene
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
        FrozenHeart
    }

    public enum EJoystickState
    {
        PointerDown,
        PointerUp,
        Drag
    }


    #region UI_NodeMapScene
    
    //todo(전지환) : StorePopup 임시 데이터
    public static int INITIAL_GOLD = 99999;
    public static int INITIAL_REROLL_COST = 0;
    
    //총 스킬 개수 
    public static int TOTAL_PLAYER_SKILL_NUMBER = 6;
    public static int TEST_SKILL_COST = 100;
    
    public static string BASE_URI = "https://k10d211.p.ssafy.io/api/";
    #endregion

}


