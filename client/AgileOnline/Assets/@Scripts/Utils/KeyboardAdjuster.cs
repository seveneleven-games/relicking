using System.Collections;
using UnityEngine;

public class KeyboardAdjuster : MonoBehaviour
{
    public RectTransform targetRectTransform; // 조정할 RectTransform
    public Vector2 originalPosition; // 원래 위치 저장

    
    void Start()
    {
        // 원래 위치 저장
        originalPosition = targetRectTransform.anchoredPosition;
        StartCoroutine(AdjustForKeyboard()); // 코루틴 시작
    }

    private IEnumerator AdjustForKeyboard()
    {
        while (true)
        {
            // 키보드가 보일 때까지 대기
            // yield return new WaitUntil(() => true);
            yield return new WaitUntil(() => TouchScreenKeyboard.visible);
            
            // 아래는 왜 인식이 안될까? -> 기기마다 안되는 것이 있다고는 하는데.. 왜?
            float keyboardHeight = TouchScreenKeyboard.area.height;
            Debug.Log($"Keyboard visible height: {keyboardHeight}");

            
            if (keyboardHeight > 0)
            {
                // 키보드 높이 추정 (실제 높이는 다른 방법으로 얻어야 할 수도 있음)
                targetRectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + keyboardHeight);
            }
            else
            {
                keyboardHeight = 1000.0f; // 임시.
                // 키보드 높이에 맞춰 위치 조정
                targetRectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + keyboardHeight);
            }

            // 키보드가 사라질 때까지 대기
            yield return new WaitUntil(() => !TouchScreenKeyboard.visible);
            Debug.Log($"Keyboard visible height: {keyboardHeight}");
            
            // 원래 위치로 복원
            targetRectTransform.anchoredPosition = originalPosition;
            
            
        }
    }
}