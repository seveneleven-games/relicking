using UnityEngine;

public class KeyboardAdjuster : MonoBehaviour
{
    public RectTransform targetRectTransform; // 조정할 RectTransform
    private Vector2 originalPosition; // 원래 위치 저장

    void Start()
    {
        // 원래 위치 저장
        originalPosition = targetRectTransform.anchoredPosition;
    }

    void Update()
    {
        AdjustForKeyboard();
    }

    private void AdjustForKeyboard()
    {
        if (TouchScreenKeyboard.visible)
        {
            if (TouchScreenKeyboard.area.height > 0)
            {
                float keyboardHeight = TouchScreenKeyboard.area.height;
                targetRectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + keyboardHeight);
            }
        }
        else
        {
            targetRectTransform.anchoredPosition = originalPosition;
        }
    }
}