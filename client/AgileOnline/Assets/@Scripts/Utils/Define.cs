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
        Creature,
        Projectile,
        Env,
    }

    public enum ECreatureType
    {
        None,
        Player,
        Monster
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
        Projectile,
        Melee,
        Field
    }

    public enum EJoystickState
    {
        PointerDown,
        PointerUp,
        Drag
    }


    #region UI_NodeMapScene

    public static int STAGE_NO = 1;
    public static string STAGE_BG_NAME = "Stage1BG";

    #endregion

}


