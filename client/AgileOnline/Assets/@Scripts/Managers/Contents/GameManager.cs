using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

// 게임 자체에서 사용할 전역으로 관리하면 좋은 데이터들 관리

[Serializable]

// 계정에 관한 모든 정보
public class GameData
{
    public int UserLevel = 1;
    public string UserName = "우주최강귀요미박설연";
    
    public int Ticket = 0;

}

public class GameManager
{
    #region GameData
    public GameData _gameData = new GameData();
    
    public int Ticket
    {
        get { return _gameData.Ticket; }
        set 
        { 
            _gameData.Ticket = value;
            // SaveGame();
            OnResourcesChanged?.Invoke();
        }
    }
    
    
    #endregion
    
    #region Player

    private Vector2 _moveDir;
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(value);
        }
    }

    private EJoystickState _joystickState;

    public EJoystickState JoystickState
    {
        get { return _joystickState; }
        set
        {
            _joystickState = value;
            OnJoystickStateChanged?.Invoke(_joystickState);
        }
    }

    #endregion
    
    #region Action

    public event Action<Vector2> OnMoveDirChanged;
    public event Action<EJoystickState> OnJoystickStateChanged;
    public event Action OnResourcesChanged;
    
    #endregion
    
    
}
