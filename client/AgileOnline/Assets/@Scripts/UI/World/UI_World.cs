using System.Collections;
using TMPro;
using UnityEngine;

public class UI_World : MonoBehaviour
{
    public static UI_World Instance { get; private set; }

    private Canvas _canvas;
    private RectTransform _canvasRectTransform;
    
    public TMP_FontAsset damageTextFont;

    private void Awake()
    {
        Instance = this;
        _canvas = GetComponent<Canvas>();
        _canvasRectTransform = _canvas.GetComponent<RectTransform>();
    }

    public void ShowDamage(int damage, Vector3 position)
    {
        GameObject damageTextObject = new GameObject("DamageText");
        damageTextObject.transform.SetParent(_canvas.transform);

        TextMeshProUGUI damageText = damageTextObject.AddComponent<TextMeshProUGUI>();
        damageText.text = damage.ToString();
        damageText.fontSize = 35;
        damageText.color = Color.white;
        damageText.alignment = TextAlignmentOptions.Center;
        Debug.Log("이거 폰트임:" + damageTextFont);
        damageText.font = damageTextFont;
        
        RectTransform rectTransform = damageTextObject.GetComponent<RectTransform>();
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(position);
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, screenPosition, _canvas.worldCamera, out canvasPosition);
        rectTransform.anchoredPosition = canvasPosition;
        rectTransform.sizeDelta = new Vector2(60, 30);
        
        StartCoroutine(MoveDamageText(damageTextObject));
    }
    
    private IEnumerator MoveDamageText(GameObject damageTextObject)
    {
        TextMeshProUGUI damageText = damageTextObject.GetComponent<TextMeshProUGUI>();
        Color alpha = damageText.color;
        float moveSpeed = 50f;
        float alphaSpeed = 2f;
        float destroyTime = 1f;

        while (destroyTime > 0)
        {
            damageTextObject.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
            damageText.color = alpha;

            destroyTime -= Time.deltaTime;
            yield return null;
        }

        Destroy(damageTextObject);
    }
}