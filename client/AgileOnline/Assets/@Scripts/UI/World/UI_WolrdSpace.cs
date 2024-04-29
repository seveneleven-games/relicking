using UnityEngine;
using UnityEngine.UI;

public class UI_WorldSpace : UI_Base
{
    public HealthBar healthBarPrefab;
    public DamageText damageTextPrefab;

    private HealthBar _healthBar;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _healthBar = Managers.Pool.Pop(healthBarPrefab.gameObject).GetComponent<HealthBar>();
        _healthBar.HideHealthBar();

        return true;
    }

    public void UpdateHealthBar(float health, float maxHealth)
    {
        if (_healthBar != null)
        {
            _healthBar.SetHealth(health, maxHealth);
            _healthBar.ShowHealthBar();
        }
    }

    public void ShowDamage(Vector3 position, int damage)
    {
        DamageText damageText = Managers.Pool.Pop(damageTextPrefab.gameObject).GetComponent<DamageText>();
        damageText.transform.position = position;
        damageText.SetDamage(damage);
    }
}