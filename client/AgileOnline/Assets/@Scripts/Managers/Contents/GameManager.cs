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
    public bool isLoaded = false; // 리소스 로딩 관련
    
    public string UserName = "";

    public string accessToken = "";
    public string refreshToken = "";
    
    public int Ticket = 0;

    // 보상 팝업 관련 플래그 설정 
    public bool showIdleRewardPopup = false;

    public int idleRewardTime = 0;

    public string LastActivePopup;
    
    // 유저가 현재 선택한 스테이지 정보
    public StageData CurrentSelectStage = new StageData();
    
    // 유저가 깬 각 스테이지별 난이도 정보
    public Dictionary<int, StageClearInfo> DicStageClearInfo = new Dictionary<int, StageClearInfo>();
    
    // 사운드 관련
    public bool BGMOn = true;
    public bool EffectSoundOn = true;
    
}

public class GameManager
{
    #region GameData
    public GameData _gameData = new GameData();

    public bool showIdleRewardPopup
    {
        get { return _gameData.showIdleRewardPopup; }
        set
        {
            _gameData.showIdleRewardPopup = value;
        }
    }
    
    public int idleRewardTime
    {
        get { return _gameData.idleRewardTime; }
        set
        {
            _gameData.idleRewardTime = value;
        }
    }

    public string LastActivePopup
    {
        get { return _gameData.LastActivePopup; }
        set
        {
            _gameData.LastActivePopup = value;
        }
    }
    
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

    #region Option

    public bool BGMOn
    {
        get { return _gameData.BGMOn; }
        set 
        {
            if (_gameData.BGMOn == value)
                return;
            _gameData.BGMOn = value;
            if (_gameData.BGMOn == false)
            {
                Managers.Sound.Stop(ESound.Bgm);
            }
            
            // Todo 여긴 계속 추가 될 것임!!! -> 아니면 현재 제일 위 팝업이 무엇인지를 알면 될듯
            else
            {
                string name = "Bgm_Lobby";
                if (Managers.Scene.CurrentScene.SceneType == Define.EScene.GameScene)
                {
                    switch (Managers.UI.GetSecondTopPopupType())
                    {
                        case UI_Popup.PopupType.NodeMap:
                            name = "Bgm_NodeMap";
                            break;
                        case UI_Popup.PopupType.InGameShop:
                            name = "Bgm_NodeMap";
                            break;
                        case UI_Popup.PopupType.InGame:
                            name = "Bgm_InGame";
                            break;
                        case UI_Popup.PopupType.InGameBoss:
                            name = "Bgm_InGameBoss";
                            break;
                        default:
                            name = "Bgm_Lobby";
                            break;
                    }
                    
                    
                }
                if (name == "Bgm_Lobby")
                {
                    Managers.Sound.Play(Define.ESound.Bgm, name,0.8f);
                }
                else
                {
                    Managers.Sound.Play(Define.ESound.Bgm, name);
                }
            }
        }
    }

    public bool EffectSoundOn
    {
        get { return _gameData.EffectSoundOn; }
        set { _gameData.EffectSoundOn = value; }
    }
    

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

    public void UpdateStageDifficulty(int stageId, int difficulty)
    {
        if (_gameData.DicStageClearInfo.ContainsKey(stageId))
        {
            // 난이도가 0인 경우 최소값 1로 설정
            int validDifficulty = Math.Max(difficulty, 1);
            
            // 이전 Max 난이도가 더 높으면 그대로 두기
            _gameData.DicStageClearInfo[stageId].MaxDifficulty = Math.Max(validDifficulty, _gameData.DicStageClearInfo[stageId].MaxDifficulty);
            _gameData.DicStageClearInfo[stageId].SelectedDifficulty = difficulty + 1;
        }
    }
    
    #endregion
    }
