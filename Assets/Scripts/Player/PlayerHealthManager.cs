using System;
using Unity.VisualScripting;
using UnityEngine;

public static class PlayerHealthManager
{
    public static int MaxHealth = 100;
    private static int currentHealth = MaxHealth;
    public static int CurrentHealth => currentHealth;

    // UI / other listeners có thể đăng ký
    public static event Action<int> OnHealthChanged;

    public static void ResetHealth()
    {
        currentHealth = MaxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public static void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0) PlayerDied();
    }

    private static void PlayerDied()
    {
        //  GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            SceneManager.LoadScene("game_over");
        }

    }
    public static void Heal(int amount)
    {
        if (currentHealth < MaxHealth)
        {
            currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
}