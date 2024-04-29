using UnityEngine;
using UnityEngine.UI;

public class UI_WorldSpace : UI_Base
{
    enum Images
    {
        HealthBarFill,
    }

    enum Texts
    {
        HealthBarText,
        DamageText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        return true;
    }

    private void Start()
    {
        Init();
    }

    public void UpdateHealthBar(float health, float maxHealth)
    {
        float fillAmount = health / maxHealth;
        GetImage((int)Images.HealthBarFill).fillAmount = fillAmount;
        GetText((int)Texts.HealthBarText).text = $"{health}/{maxHealth}";
    }

    public void ShowDamage(Vector3 position, int damage)
    {
        GameObject damageTextObj = Managers.Resource.Instantiate("DamageText");
        damageTextObj.transform.position = position;
        damageTextObj.GetComponent<DamageText>().SetDamage(damage);
    }
}