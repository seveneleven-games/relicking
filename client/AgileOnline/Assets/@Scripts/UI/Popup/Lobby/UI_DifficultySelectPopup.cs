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
    
    // 임시 -> 테스트를 위해
    int currentDifficulty = 10; 
    
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

        BindObject(typeof(EGameObjects));
        BindButton(typeof(EButtons));
        BindText(typeof(ETexts));
        BindToggle(typeof(EToggles));
        BindImage(typeof(EImages));


        GenerateDifficultyButtons(currentDifficulty);
        
        
        Refresh();
        
        #endregion
        
        return true;
    }

    void GenerateDifficultyButtons(int currentDifficulty)
    {
        // 현재 난이도가 5보다 작을 경우 대비
        int startLevel = Mathf.Max(1, currentDifficulty - 5);  // 1 이하로 내려가지 않도록 제한
        int endLevel = currentDifficulty + 5;

        
        for (int i = endLevel; i >= startLevel; i--)
        {
            int level = i;
            GameObject newDifficultyButton = Instantiate(UI_DifficultyButton,
                GetObject((int)EGameObjects.DifficultySelectContent).transform);
            newDifficultyButton.GetComponentInChildren<TMP_Text>().text = "Level " + i;
            
            // 현재 난이도라면 색깔 변화
            if (i == currentDifficulty)
                newDifficultyButton.GetComponentInChildren<TMP_Text>().color = Util.HexToColor("4C2627");
            
            newDifficultyButton.GetComponent<Button>().onClick.AddListener(() => SelectDifficulty(level));
        }
    }

    void SelectDifficulty(int level)
    {
        Debug.Log($"Selected Difficulty: Level {level}");
        Managers.UI.ClosePopupUI(this);
    }
    
    void Refresh()
    {

    }
    
}
