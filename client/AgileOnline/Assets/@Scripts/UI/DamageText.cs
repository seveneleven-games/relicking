using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private TextMeshProUGUI damageText;

    private void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();
    }

    public void SetDamage(int damage)
    {
        damageText.text = damage.ToString();
        Invoke("DestroyText", 1f); // 1초 후에 데미지 텍스트 삭제
    }

    private void DestroyText()
    {
        Managers.Resource.Destroy(gameObject);
    }
}