using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Random = UnityEngine.Random;

// 내가 볼 때 여기서 오류가 뜨는 것은 Destroy를 통해 해결하면 될 듯!!

// _StageData : 지금 여기서 쓰이는 객체
// Managers.Game.CurrentSelectStage : 유저 정보 내의 객체
// Managers.Data.StageDic : json으로 부터 가져온 전체 스테이지 정보

[Serializable]
public class EnterStageRes
{
    public int status;
    public string message;
    public EnterStageData data;
}

[Serializable]
public class EnterStageData
{
    public int currentClassNo;
    public List<RelicList> relicList;
}

public class UI_BattlePopup : UI_Popup
{

    #region UI 기능 리스트

    // 정보 갱신
    // StageScrollContentObject : UI_ChapterInfoItem이 들어갈 부모 개체
    // StageImage : 스테이지의 이미지 (테이블에 추가 필요)
    // StageNameValueText : 스테이지의 이름 (테이블에 추가 필요)

    #endregion
    
    #region Enum

    enum EGameObjects
    {
        ContentObject, // 나중에 애니메이션 넣을 시에 필요
        StageSelectScrollView,
        StageScrollContentObject,
    }
    
    enum EButtons
    {
        LArrowButton,
        RArrowButton,
        StartButton,
        DifficultySelectButton,
    }
    
    enum ETexts
    {
        StageNameText,
        StartButtonText,
        DifficultyText,
    }
    
    enum EToggles
    {
        
    }
    
    enum EImages
    {
        
    }
    
    #endregion
    
    // 객체 관련 두는 곳
    StageData _stageData; // Data.Contents
    HorizontalScrollSnap _scrollSnap;
    
    StageClearInfo _clearInfo;
    
    public TemplateData _templateData;
    
    
    
    // 이거 죽이면 에러 뜸
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
        
        // 시작 버튼
        GetButton((int)EButtons.StartButton).gameObject.BindEvent(OnClickStartButton);
        GetButton((int)EButtons.StartButton).GetOrAddComponent<UI_ButtonAnimation>();
        
        // 왼쪽 버튼
        GetButton((int)EButtons.LArrowButton).gameObject.BindEvent(onClickLArrowButton);
        GetButton((int)EButtons.LArrowButton).GetOrAddComponent<UI_ButtonAnimation>();
        
        // 오른쪽 버튼
        GetButton((int)EButtons.RArrowButton).gameObject.BindEvent(onClickRArrowButton);
        GetButton((int)EButtons.RArrowButton).GetOrAddComponent<UI_ButtonAnimation>();
        
        // 난이도(레벨) 선택 버튼
        GetButton((int)EButtons.DifficultySelectButton).gameObject.BindEvent(onClickDifficultySelectButton);
        
        // 현재 스테이지 관련 -> 임시로 1로 되어있음 나중에 유저 정보 저장한다면 그 값으로 바꿔주기 (근데 3개 뿐이라 딱히 안 해도 될지도)
        _stageData = Managers.Game.CurrentSelectStage;
        
        // 스크롤 관련 (스테이지)
        // HorizontalScrollSnap라는 유형의 컴포넌트를 찾아 변수 할당 (하위 자식 포함)
        _scrollSnap = Util.FindChild<HorizontalScrollSnap>(gameObject, recursive: true);
        // 스테이지가 변경될 때마다 OnChangeStage 호출
        _scrollSnap.OnSelectionPageChangedEvent.AddListener(OnChangeStage);
        // 첫 스테이지 상태
        _scrollSnap.StartingScreen = Managers.Game.CurrentSelectStage.StageId - 1;
        
        _templateData = Resources.Load<TemplateData>("GameTemplateData");
        
        #endregion
        
        Refresh();
        
        
        
        return true;
    }

    #region 난이도 선택 시 감지를 위해서

    private void OnEnable()
    {
        UI_DifficultySelectPopup.OnDifficultyChanged += HandleDifficultyChanged;
    }

    private void OnDisable()
    {
        UI_DifficultySelectPopup.OnDifficultyChanged -= HandleDifficultyChanged;
    }

    private void HandleDifficultyChanged(int level)
    {
        if (_clearInfo != null)
        {
            StageInfoRefresh(); // 스테이지 정보 갱신
        }
    }
    
    #endregion
    
    
    
    public void SetInfo(StageData stageData)
    {
        _stageData = stageData;
        Refresh();
    }
    
    // 갱신
    void Refresh()
    {

        if (_init == false)
            return;

        if (_stageData == null)
            return;
        
        #region 초기화
        
        #region 스테이지 리스트
        
        // 다 날리고
        GameObject StageContainer = GetObject((int)EGameObjects.StageScrollContentObject);
        StageContainer.DestroyChilds();
        
        // 다시 받아오기
        // 스테이지 갯수만큼 만들기 - json으로 부터 가져온 스테이지 정보를 통해
        _scrollSnap.ChildObjects = new GameObject[Managers.Data.StageDic.Count];
        foreach (StageData stageData in Managers.Data.StageDic.Values)
        {
            // StageInfoItem 달기
            UI_StageInfoItem item = Managers.UI.MakeSubItem<UI_StageInfoItem>(StageContainer.transform);
            item.SetInfo(stageData);
            _scrollSnap.ChildObjects[stageData.StageId - 1] = item.gameObject;
        }
        
        #endregion

        StageInfoRefresh();

        #endregion
    }

    
    // 스테이지 정보 갱신
    void StageInfoRefresh()
    {

        #region 스테이지 정보
        if (_stageData == null)
            return;
        
        // 스테이지 ID를 사용하여 GameManager에서 SelectedDifficulty 값을 가져옴
        if (Managers.Game.DicStageClearInfo.TryGetValue(_stageData.StageId, out _clearInfo))
        {
            int selectedDifficulty = _clearInfo.SelectedDifficulty;
            Debug.Log("-----------------------------------------------");
            Debug.Log("selectedDifficulty: " + selectedDifficulty);

            
            GetText((int)ETexts.DifficultyText).text = "Level " + selectedDifficulty;
        }
        else
        {
            Debug.LogError("Stage ID not found in DicStageClearInfo");
        }
        
        UIRefresh();
        #endregion

    }

    void UIRefresh()
    {
        // 기본 상태
        GetButton((int)EButtons.LArrowButton).gameObject.SetActive(true);
        GetButton((int)EButtons.RArrowButton).gameObject.SetActive(true);
        
        #region 스테이지 화살표 활성화 관련 

        // (스테이지 갯수에 따라 다르게 설정해주기) - 이건 하드코딩 말고도 가능할 텐데..
        
        if (_stageData.StageId == 1)
        {
            GetButton((int)EButtons.LArrowButton).gameObject.SetActive(false);
            GetButton((int)EButtons.RArrowButton).gameObject.SetActive(true);
        }
        else if (_stageData.StageId >= 2 && _stageData.StageId < 3)
        {
            GetButton((int)EButtons.LArrowButton).gameObject.SetActive(true);
            GetButton((int)EButtons.RArrowButton).gameObject.SetActive(true);
        }
        else if (_stageData.StageId == 3) // Todo 스테이지 갯수만큼 
        {
            GetButton((int)EButtons.LArrowButton).gameObject.SetActive(true);
            GetButton((int)EButtons.RArrowButton).gameObject.SetActive(false);
        }

        #endregion
        
    }

    // 왼쪽 버튼 클릭 함수
    void onClickLArrowButton()
    {
        Managers.Sound.PlayButtonClick();
        if (_scrollSnap.CurrentPage > 0)
        {
            _scrollSnap.GoToScreen(_scrollSnap.CurrentPage - 1);
        }
    }

    // 오른쪽 버튼 클릭 함수
    void onClickRArrowButton()
    {
        Managers.Sound.PlayButtonClick();
        if (_scrollSnap.CurrentPage < _scrollSnap.ChildObjects.Length - 1)
        {
            _scrollSnap.GoToScreen(_scrollSnap.CurrentPage + 1);
        }
    }
    
    void OnClickStartButton()
    {
        Managers.Sound.PlayButtonClick();
        // 유저의 현재 스테이지 정보를 저장
        Managers.Game.CurrentSelectStage = _stageData;
        SetInfo(Managers.Game.CurrentSelectStage);
        
        // 현재 난이도도 나중에 줘야 됨.
        // 여기서 게임씬으로 가는 것도 나중에 추가하기!!!
        
        // Todo -> change
        Debug.Log("go Game");
        _templateData.StageId = Managers.Game.CurrentSelectStage.StageId;
        _templateData.Difficulty =
            Managers.Game.DicStageClearInfo[Managers.Game.CurrentSelectStage.StageId].SelectedDifficulty;
        StartCoroutine(Util.JWTGetRequest("stages", res =>
        {
            Debug.Log(res);
            EnterStageRes enterStageRes = JsonUtility.FromJson<EnterStageRes>(res);

            if (enterStageRes != null)
            {
                _templateData.SelectedClassId = enterStageRes.data.currentClassNo;
                enterStageRes.data.relicList.ForEach(relic =>
                {
                    int index = relic.slot - 1;
                    if (index >= 0 && index < _templateData.EquipedRelicIds.Length)
                    {
                        _templateData.EquipedRelicIds[index] = relic.relicNo * 10 + relic.level;
                    }
                });
                foreach (int relicId in _templateData.EquipedRelicIds)
                {
                    Debug.Log("relic id is : " + relicId);
                }

                
                if (enterStageRes.status == 200)
                {
                    // SceneCover sceneCover = Managers.Resource.Instantiate("SceneCover").GetOrAddComponent<SceneCover>();
                    // sceneCover.CoverToScene("GameScene");
                    Managers.Scene.LoadScene(Define.EScene.GameScene);
                }
            }
        }));
    }

    void OnChangeStage(int index)
    {
        Managers.Sound.PlayButtonClick();
        // 현재 스테이지 설정
        _stageData = Managers.Data.StageDic[index + 1];
        Debug.Log(_stageData.StageId);

        StageInfoRefresh();
    }

    // 난이도 선택 팝업 열기
    void onClickDifficultySelectButton()
    {
        Managers.Sound.PlayButtonClick();
        UI_DifficultySelectPopup popup = Managers.UI.ShowPopupUI<UI_DifficultySelectPopup>();
        
        // maxDifficulty를 UI_DifficultySelectPopup으로 전달
        if (_clearInfo != null)
        {
            popup.SetMaxDifficulty(_stageData.StageId, _clearInfo.MaxDifficulty);
        }
    }
    
   
    

}


