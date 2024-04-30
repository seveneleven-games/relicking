using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextController : BaseController
{
    private TextMeshPro damageText;

    private float fontSize = 5f;
    private Color textColor = Color.white;
    private TextAlignmentOptions textAlignment = TextAlignmentOptions.Center;
    private Vector2 textSize = new Vector2(100, 50);

    private void Awake()
    {
        damageText = GetComponentInChildren<TextMeshPro>();
    }

    public void ShowDamageText(Vector3 position, int damage)
    {
        transform.position = position;
        damageText.text = damage.ToString();
        StartCoroutine(CoHideDamageText());
    }

    private IEnumerator CoHideDamageText()
    {
        yield return new WaitForSeconds(0.3f);
        Managers.Pool.Push(gameObject);
    }

    public void InitDamage(int templateId)
    {
        damageText.fontSize = fontSize;
        damageText.color = textColor;
        damageText.alignment = textAlignment;
        damageText.rectTransform.sizeDelta = textSize;
    }
}
