using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Util;

public class UI_DifficultySelectPopup : UI_Popup
{

    // 사용할 프리팹 가져오기
    [SerializeField] 
    GameObject UI_DifficultyButton;
    
    #region Enum
    enum EGameObjects
    {
        DifficultySelectContent, // Transform을 찾을 것임
    }
    
    enum EButtons
    {
        
    }
    
    enum ETexts
    {
        
    }
    
    enum EToggles
    {
        
    }
    
    enum EImages
    {
        
    }
    
    #endregion
    
    // 객체 관련 두는 곳
    private int _maxDifficulty;
    private int _currentStageId;
    
    public TemplateData _templateData;
    
    StageClearInfo _clearInfo;
    
    public static event Action<int> OnDifficultyChanged;
    
    private void Awake()
    {
        Init();
    }
    
    // 초기 세팅
    public override bool Init()
    {
        
        if (base.Init() == false)
            return false;

        #region Object Bind
        
        _templateData = Resources.Load<TemplateData>("GameTemplateData");

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));
        BindToggle(typeof(EToggles));
        BindImage(typeof(EImages));
        
        Refresh();
        
        
        #endregion
        
        return true;
    }

    // BattlePopup으로 부터 정보 가져오기
    public void SetMaxDifficulty(int currentStageId, int maxDifficulty)
    {
        _currentStageId = currentStageId;
        _maxDifficulty = maxDifficulty;
        
        // 현재 스테이지의 클리어 정보 가져오기
        _clearInfo = Managers.Game.DicStageClearInfo[currentStageId];
        
        GenerateDifficultyButtons(_maxDifficulty, _clearInfo.SelectedDifficulty);
    }
    
    void GenerateDifficultyButtons(int maxDifficulty, int selectedDifficulty)
    {
        // 현재 난이도가 5보다 작을 경우 대비
        int startLevel = Mathf.Max(1, maxDifficulty - 5);  // 1 이하로 내려가지 않도록 제한
        int endLevel = maxDifficulty + 5;
        
        for (int i = endLevel; i >= startLevel; i--)
        {
            int level = i;
            GameObject newDifficultyButton = Instantiate(UI_DifficultyButton,
                GetObject((int)EGameObjects.DifficultySelectContent).transform);
            newDifficultyButton.GetComponentInChildren<TMP_Text>().text = "Level " + i;
            
            // 현재 난이도라면 색깔 변화
            if (i == selectedDifficulty)
                newDifficultyButton.GetComponentInChildren<TMP_Text>().color = Util.HexToColor("4C2627");
            
            newDifficultyButton.GetComponent<Button>().onClick.AddListener(() => SelectDifficulty(level));
        }
    }

    
    void SelectDifficulty(int level)
    {
        Debug.Log($"Selected Difficulty: Level {level}");
        
        // 혹시 몰라서 주석으로 남겨둠.....
        // // 스테이지 ID를 사용하여 GameManager에서 SelectedDifficulty를 가져와서 바꿔줌
        // if (Managers.Game.DicStageClearInfo.TryGetValue(_currentStageId, out _clearInfo))
        // {
        //     _clearInfo.SelectedDifficulty = level;
        // }
        // else
        // {
        //     Debug.LogError("Stage ID not found in DicStageClearInfo");
        // }
        _templateData.difficulty = level;
        _clearInfo.SelectedDifficulty = level;
        OnDifficultyChanged?.Invoke(level); // 이벤트 발생
        
        Managers.UI.ClosePopupUI(this);
    }
    
    void Refresh()
    {

    }
    
}
