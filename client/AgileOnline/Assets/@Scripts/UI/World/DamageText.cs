using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public float lifeTime = 1f;

    private void OnEnable()
    {
        Invoke("ReturnToPool", lifeTime);
    }

    public void SetDamage(int damage)
    {
        damageText.text = damage.ToString();
    }

    private void ReturnToPool()
    {
        Managers.Pool.Push(gameObject);
    }
}