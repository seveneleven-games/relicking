using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLerpAnimation : MonoBehaviour {

    [Header("Initial Pos X and Pos Y of RectTransform")]
    public Vector2 initialPos;
    [Space]
    [Header("Final Pos X and Pos Y of RectTransform")]
    public Vector2 finalPos;
    [Space]
    [Header("Animation Speed")]
    public float animationSpeed = 5;

    public bool topbottomAnim = false;
    RectTransform rectTransform;

    private void OnEnable()
    {
       
        print("OnEnable");
        rectTransform = GetComponent<RectTransform>();


        if (topbottomAnim)
        {
            initialPos.y   = Mathf.Abs(initialPos.y);
            rectTransform.anchoredPosition = initialPos;
        }
        else {
            rectTransform.anchoredPosition = initialPos;
        }
       
           
            StartCoroutine(PositionAnimation());
       
    }
    IEnumerator PositionAnimation()
    {
        while(Vector2.Distance(rectTransform.anchoredPosition, finalPos) > 0) {
            rectTransform.anchoredPosition = Vector2.Lerp((Vector2)rectTransform.anchoredPosition, finalPos, Time.deltaTime * animationSpeed);
           yield return null;
        }
     
        yield return 0;
    }

   


}
