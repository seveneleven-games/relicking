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
    public int SelectedDifficulty = 0;
}


[Serializable]
// 계정에 관한 모든 정보
public class GameData
{
    
    public string UserName = "우주최강귀요미박설연";

    public string accessToken = "";
    public string refreshToken = "";
    
    public int Ticket = 0;

    // 유저가 현재 선택한 스테이지 정보
    public StageData CurrentSelectStage = new StageData();
    
    // 유저가 깬 각 스테이지별 난이도 정보
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
    
    public string AccessToken
    {
        get { return _gameData.accessToken; }
        set
        {
            _gameData.accessToken = value;
        }
    }
    
    public string RefreshToken
    {
        get { return _gameData.refreshToken; }
        set
        {
            _gameData.refreshToken = value;
        }
    }
    
    public StageData CurrentSelectStage
    {
        get { return _gameData.CurrentSelectStage; }
        set { _gameData.CurrentSelectStage = value; }
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
        // 현재 유저가 선택한 스테이지 정보 (나중에 유저 정보 값으로 바꿔주기!!!)
        CurrentSelectStage = Managers.Data.StageDic[1];
        
        // 스테이지 수에 맞게 각 스테이지 별 난이도 클리어 정보 Add하기
        foreach (StageData stageData in Managers.Data.StageDic.Values)
        {
            StageClearInfo info = new StageClearInfo
            {
                StageId = stageData.StageId,
                // MaxDifficulty = 10, // test를 위해
                // SelectedDifficulty = 11, // test를 위해
            };
            _gameData.DicStageClearInfo.Add(stageData.StageId, info);
        }
    }

    #region 로그인 시 스테이지 별 난이도 가져오기

    public void UpdateStageClearInfo(StageRes stageRes)
    {
        if (stageRes == null) return;

        // 각 스테이지별 정보 업데이트
        UpdateStageDifficulty(1, stageRes.stage1);
        UpdateStageDifficulty(2, stageRes.stage2);
        UpdateStageDifficulty(3, stageRes.stage3);
    }

    private void UpdateStageDifficulty(int stageId, int difficulty)
    {
        if (_gameData.DicStageClearInfo.ContainsKey(stageId))
        {
            // 난이도가 0인 경우 최소값 1로 설정
            int validDifficulty = Math.Max(difficulty, 1);
            _gameData.DicStageClearInfo[stageId].MaxDifficulty = validDifficulty;
            _gameData.DicStageClearInfo[stageId].SelectedDifficulty = difficulty + 1;
        }
    }
    
    #endregion
    
}
