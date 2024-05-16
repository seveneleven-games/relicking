using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneCover : UI_Base
{
    public Image coverImage;
    public float coverSpeed = 1.5f;
    private RectTransform coverRectTransform;

    void Start()
    {
        coverRectTransform = coverImage.GetComponent<RectTransform>();
        // 초기 위치 설정: 화면 위쪽 바깥에 위치시키기
        coverRectTransform.anchoredPosition = new Vector2(0, coverRectTransform.sizeDelta.y);
        StartCoroutine(CoverScreen());
        
        // 이 GameObject를 씬 전환 중에도 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }

    public void CoverToScene(Define.EScene eScene)
    {
        StartCoroutine(UncoverScreen(eScene));
    }

    IEnumerator CoverScreen()
    {
        float positionY = coverRectTransform.anchoredPosition.y;

        // 이미지가 화면 아래로 내려오도록
        while (positionY > 0)
        {
            positionY -= Time.deltaTime * coverSpeed * coverRectTransform.sizeDelta.y;
            coverRectTransform.anchoredPosition = new Vector2(0, positionY);
            yield return null;
        }
    }
    
    IEnumerator UncoverScreen(Define.EScene eScene)
    {
        float positionY = coverRectTransform.anchoredPosition.y;

        // 화면을 다시 위로 밀어내기
        while (positionY < coverRectTransform.sizeDelta.y)
        {
            positionY += Time.deltaTime * coverSpeed * coverRectTransform.sizeDelta.y;
            coverRectTransform.anchoredPosition = new Vector2(0, positionY);
            yield return null;
        }

        Managers.Scene.LoadScene(eScene);
        
        // 새 씬이 로드된 후, 이 이미지를 비활성화하거나 제거
        Destroy(gameObject);  // 이 방식은 GameObject를 완전히 파괴합니다.
    }
}