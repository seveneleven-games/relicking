using UnityEngine;
using UnityEngine.UI;

public class KeyboardAdjuster : MonoBehaviour
{
    public RectTransform inputFieldRectTransform; // 입력 필드의 RectTransform
    private Vector2 originalPosition; // 원래 위치 저장

    void Start()
    {
        originalPosition = inputFieldRectTransform.anchoredPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            AdjustForKeyboard();
        }
    }

    private void AdjustForKeyboard()
    {
        // 터치 스크린 키보드의 상태 확인
        if (TouchScreenKeyboard.visible)
        {
            // 키보드가 활성화되면 입력 필드를 위로 이동
            if (TouchScreenKeyboard.area.height > 0)
            {
                float keyboardHeight = TouchScreenKeyboard.area.height;
                inputFieldRectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + keyboardHeight);
            }
        }
        else
        {
            // 키보드가 비활성화되면 원래 위치로 돌아감
            inputFieldRectTransform.anchoredPosition = originalPosition;
        }
    }
}