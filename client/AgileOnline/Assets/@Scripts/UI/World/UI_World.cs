using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_World : MonoBehaviour
{
    public static UI_World Instance { get; private set; }

    private Canvas _canvas;
    private RectTransform _canvasRectTransform;
    private Slider _playerHealthSlider;
    
    public TMP_FontAsset damageTextFont;

    private void Awake()
    {
        Instance = this;
        _canvas = GetComponent<Canvas>();
        _canvasRectTransform = _canvas.GetComponent<RectTransform>();
        _playerHealthSlider = GameObject.FindWithTag("PlayerHealthSlider").GetComponent<Slider>();
    }
    
    public void UpdatePlayerHealth(float currentHealth, float maxHealth)
    {
        _playerHealthSlider.value = currentHealth / maxHealth;
    }

    // public void UpdateBossHealth(float currentHealth, float maxHealth)
    // {
    //     _bossMonsterHealthSlider.value = currentHealth / maxHealth;
    // }

    private void LateUpdate()
    {
        if (Managers.Object.Player != null)
        {
            Vector3 playerPosition = Managers.Object.Player.transform.position;
            Vector3 healthBarPosition = playerPosition + Vector3.down * 0.4f;
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(healthBarPosition);
            Vector2 canvasPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, screenPosition, _canvas.worldCamera, out canvasPosition);
            _playerHealthSlider.GetComponent<RectTransform>().anchoredPosition = canvasPosition;
        }
    }

    public void ShowDamage(int damage, Vector3 position, bool isCritical = false)
    {
        GameObject damageTextObject = new GameObject("DamageText");
        damageTextObject.transform.SetParent(_canvas.transform);

        TextMeshProUGUI damageText = damageTextObject.AddComponent<TextMeshProUGUI>();
        damageText.text = damage.ToString();
        damageText.fontSize = isCritical ? 55 : 45;
        damageText.color = isCritical ? Color.red : Color.white;
        damageText.alignment = TextAlignmentOptions.Center;
        damageText.font = damageTextFont;
        
        RectTransform rectTransform = damageTextObject.GetComponent<RectTransform>();
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(position);
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, screenPosition, _canvas.worldCamera, out canvasPosition);
        rectTransform.anchoredPosition = canvasPosition;
        rectTransform.sizeDelta = new Vector2(600, 100);
        
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