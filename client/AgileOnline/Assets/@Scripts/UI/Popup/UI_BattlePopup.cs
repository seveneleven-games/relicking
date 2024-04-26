using System.Collections;
using System.Collections.Generic;
using Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

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
        ContentObject,
        StageSelectScrollView,
        StageScrollContentObject,
    }
    
    enum EButtons
    {
        LArrowButton,
        RArrowButton,
        StartButton,
    }
    
    enum ETexts
    {
        StageNameText,
        StartButtonText
    }
    
    enum EToggles
    {
        
    }
    
    enum EImages
    {
        
    }
    
    #endregion
    
    // 객체 관련 두는 곳
    StageData _stageData;
    HorizontalScrollSnap _scrollSnap;
    
    
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
        GetButton((int)EButtons.StartButton).gameObject.SetActive(false);
        GetButton((int)EButtons.StartButton).gameObject.BindEvent(OnClickStartButton);
        GetButton((int)EButtons.StartButton).GetOrAddComponent<UI_ButtonAnimation>();
        
        // 왼쪽 버튼
        GetButton((int)EButtons.LArrowButton).gameObject.SetActive(false);
        // GetButton((int)EButtons.LArrowButton).gameObject.BindEvent(onClickLArrowButton);
        GetButton((int)EButtons.LArrowButton).GetOrAddComponent<UI_ButtonAnimation>();
        
        // 오른쪽 버튼
        GetButton((int)EButtons.RArrowButton).gameObject.SetActive(false);
        // GetButton((int)EButtons.RArrowButton).gameObject.BindEvent(onClickRArrowButton);
        GetButton((int)EButtons.RArrowButton).GetOrAddComponent<UI_ButtonAnimation>();

        // 스크롤 관련 (스테이지)
        // HorizontalScrollSnap라는 유형의 컴포넌트를 찾아 변수 할당 (하위 자식 포함)
        _scrollSnap = Util.FindChild<HorizontalScrollSnap>(gameObject, recursive: true);
        // 스테이지가 변경될 때마다 OnChangeStage 호출
        _scrollSnap.OnSelectionPageChangedEvent.AddListener(OnChangeStage);
        // 첫 스테이지 상태
        _scrollSnap.StartingScreen = Managers.Game.CurrentStageData.StageId - 1;
        
        
        
        // 임시
        GetObject((int)EGameObjects.StageSelectScrollView).BindEvent(() =>
        {
            Debug.Log("go Game");
            Managers.Scene.LoadScene(Define.EScene.GameScene);
        });
        
        #endregion
        
        Refresh();
        
        return true;
    }

    
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
        _scrollSnap.ChildObjects = new GameObject[Managers.Data.StageDic.Count];
        foreach (StageData stageData in Managers.Data.StageDic.Values)
        {
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

        UIRefresh();

        #endregion

    }

    void UIRefresh()
    {
        // 기본 상태
        GetButton((int)EButtons.LArrowButton).gameObject.SetActive(true);
        GetButton((int)EButtons.RArrowButton).gameObject.SetActive(true);
        GetButton((int)EButtons.StartButton).gameObject.SetActive(true);
        
        
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
        
        
        #region 스테이지 시작 버튼

        if (Managers.Game.DicStageClearInfo.TryGetValue(_stageData.StageId, out StageClearInfo info) == false)
            return;
        
        // 게임 처음 시작하고 스테이지창을 오픈 한 경우
        if (info.StageId == 1 && info.MaxDifficulty == 0)
        {
            GetButton((int)EButtons.StartButton).gameObject.SetActive(true);   
        }
        // 스테이지 진행중
        if (info.StageId <= _stageData.StageId)
            GetButton((int)EButtons.StartButton).gameObject.SetActive(true);
        
        // 새로운 스테이지
        if (Managers.Game.DicStageClearInfo.TryGetValue(_stageData.StageId - 1, out StageClearInfo PrevInfo) == false)
            return;
        if (PrevInfo.isClear == true)
            GetButton((int)EButtons.StartButton).gameObject.SetActive(true);
        else
            GetButton((int)EButtons.StartButton).gameObject.SetActive(false);
        
        #endregion
        
    }

    void OnClickStartButton()
    {
        Managers.Game.CurrentStageData = _stageData;
        // 현재 난이도도 나중에 줘야 됨.
        // 여기서 게임씬으로 가는 것도 나중에 추가하기!!!
    }

    void OnChangeStage(int index)
    {
        // 현재 스테이지 설정
        _stageData = Managers.Data.StageDic[index + 1];
        
        UIRefresh();
    }
    

}