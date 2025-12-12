using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealBarMT : MonoBehaviour
{
    public static HealBarMT Instance;
    [SerializeField] private Slider healthBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }
}
