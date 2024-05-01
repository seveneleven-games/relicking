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

    // 유저의 현재 스테이지 정보
    public StageData CurrentStage = new StageData();
    public Dictionary<int, StageClearInfo> DicStageClearInfo = new Dictionary<int, StageClearInfo>();

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

    // 초기 세팅
    public void Init()
    {
        // 나중에 유저 정보 값으로 바꿔주기!!!
        // CurrentStageData = Managers.Data.StageDic[2];
        CurrentStageData = Managers.Data.StageDic[1];
    }
    
    
}
