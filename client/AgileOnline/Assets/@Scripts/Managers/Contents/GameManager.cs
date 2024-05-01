using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

// 게임 자체에서 사용할 전역으로 관리하면 좋은 데이터들 관리

[Serializable]
public class StageClearInfo
{
    public int StageId = 1;
    public int MaxDifficulty = 0;
    public bool isClear = false;
}


[Serializable]
// 계정에 관한 모든 정보
public class GameData
{
    public int UserLevel = 1;
    public string UserName = "우주최강귀요미박설연";

    public int Ticket = 0;

    public StageData CurrentStage = new StageData();
    public Dictionary<int, StageClearInfo> DicStageClearInfo = new Dictionary<int, StageClearInfo>();
}

public class GameManager
{
    public void InitializeGameData()
    {
        // TODO: 게임 데이터 초기화 로직 구현
        _gameData = new GameData();
        _gameData.UserLevel = 1;
        _gameData.UserName = "우주최강귀요미박설연";
        _gameData.Ticket = 0;
        _gameData.CurrentStage = new StageData();
        _gameData.DicStageClearInfo = new Dictionary<int, StageClearInfo>();
    }

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

    public Dictionary<int, StageClearInfo> DicStageClearInfo
    {
        get { return _gameData.DicStageClearInfo; }
        set
        {
            _gameData.DicStageClearInfo = value;
            // SaveGame();
        }
    }

    public StageData CurrentStageData
    {
        get { return _gameData.CurrentStage; }
        set { _gameData.CurrentStage = value; }
    }

    #endregion

    #region Player

    public PlayerController Player { get; set; }
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