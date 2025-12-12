using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    [SerializeField] private TMP_Text keyCountText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider ninjaTimerBar;
    [SerializeField] private Slider knightTimerBar;
    [SerializeField] private Slider shieldTimerBar;
    [SerializeField] private Slider upgradeTimerBar;
    [SerializeField] private TMP_Text monsterKilled;
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
    public void UpdateKeyCount(int keyCount)
    {
        keyCountText.text = $"x{keyCount}";
    }
    public void UpdateMonsterKilled(int count)
    {
        monsterKilled.text = $"x{count}";
    }
    public void UpdateNinjaTimer(float timeLeft, float maxTime)
    {
        ninjaTimerBar.maxValue = maxTime;
        ninjaTimerBar.value = timeLeft;
        
    }
    public void UpdateKnightTimer(float timeLeft, float maxTime)
    {
        knightTimerBar.maxValue = maxTime;
        knightTimerBar.value = timeLeft;
        
    }
    public void UpdateShieldTimer(float timeLeft, float maxTime)
    {
        shieldTimerBar.maxValue = maxTime;
        shieldTimerBar.value = timeLeft;
        
    }
    public void UpdateUpgradeTimer(float timeLeft, float maxTime)
    {
        upgradeTimerBar.maxValue = maxTime;
        upgradeTimerBar.value = timeLeft;
        
    }
    
}
