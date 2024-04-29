using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;

    public void SetHealth(float health, float maxHealth)
    {
        healthSlider.value = health / maxHealth;
    }

    public void ShowHealthBar()
    {
        gameObject.SetActive(true);
    }

    public void HideHealthBar()
    {
        gameObject.SetActive(false);
    }
}
