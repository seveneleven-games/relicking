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
        
        //todo(전지환) : 머지 전에 test씬 삭제하기
        TestNodeMapScene
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
        ElectronicField
    }

    public enum EJoystickState
    {
        PointerDown,
        PointerUp,
        Drag
    }


    #region UI_NodeMapScene

    public static int STAGE_NO = 1;
    public static string STAGE_NODEMAP_NAME = "UI_Stage1NodeMap_01";
    public static string STAGE_BG_NAME = "Stage1BG.sprite";

    #endregion

}


