using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum EScene
    {
        Unknown,
        TitleScene,
        GameScene,
        LobbyScene,
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
        Dead = 3,
        Skill = 4
    }

    public enum EJoystickState
    {
        PointerDown,
        PointerUp,
        Drag
    }
}


