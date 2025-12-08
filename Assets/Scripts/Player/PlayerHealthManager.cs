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
        Debug.Log($"Player took {damage} dmg. HP = {currentHealth}");
        OnHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0) PlayerDied();
    }

    private static void PlayerDied()
    {
        Debug.Log("Player died (shared health).Game Over.");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        // Thêm event / logic nếu cần
    }
}