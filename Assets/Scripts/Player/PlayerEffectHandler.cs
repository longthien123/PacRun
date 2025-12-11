using UnityEngine;

public class PlayerEffectHandler : MonoBehaviour
{
    private bool isBurning = false;
    private float burnDamagePerSecond;
    private float burnTimeLeft;
    private float burnTickTimer;
    
    private bool isFrozen = false;
    private float frozenTimeLeft;
    
    private CharacterBase characterBase;
    
    void Awake()
    {
        characterBase = GetComponent<CharacterBase>();
    }
    
    void Update()
    {
        // Cập nhật reference đến CharacterBase mỗi frame (vì có thể đổi form)
        if (characterBase == null)
        {
            characterBase = GetComponent<CharacterBase>();
        }
        
        // Xử lý Burn effect
        if (isBurning)
        {
            burnTimeLeft -= Time.deltaTime;
            burnTickTimer -= Time.deltaTime;
            
            if (burnTickTimer <= 0f)
            {
                Instantiate(PlayerController.Instance.redSkullFx, transform.position, Quaternion.identity);
                // Gây damage mỗi giây
                PlayerHealthManager.TakeDamage((int)burnDamagePerSecond);
                Debug.Log($"Burn damage: {burnDamagePerSecond}");
                burnTickTimer = 1f;
            }
            
            if (burnTimeLeft <= 0f)
            {
                isBurning = false;
                Debug.Log("Burn effect ended!");
            }
        }
        
        // Xử lý Freeze effect
        if (isFrozen)
        {
            frozenTimeLeft -= Time.deltaTime;
            
            // Đảm bảo tốc độ = 0 trong lúc freeze
            if (characterBase != null)
            {
                Instantiate(PlayerController.Instance.blueSkullFx, transform.position, Quaternion.identity);
                characterBase.moveSpeed = 0f;
            }
            
            if (frozenTimeLeft <= 0f)
            {
                RemoveFreezeEffect();
            }
        }
    }
    
    public void ApplyBurnEffect(int damagePerSecond, float duration)
    {
        isBurning = true;
        burnDamagePerSecond = damagePerSecond;
        burnTimeLeft = duration;
        burnTickTimer = 1f;
        
        Debug.Log($"Burn effect applied: {damagePerSecond} dmg/s for {duration}s");
    }
    
    public void ApplyFreezeEffect(float duration)
    {
        // Cập nhật reference đến CharacterBase
        if (characterBase == null)
        {
            characterBase = GetComponent<CharacterBase>();
        }
        
        // Nếu chưa bị freeze thì áp dụng freeze mới
        if (!isFrozen)
        {
            isFrozen = true;
            frozenTimeLeft = duration;
            
            if (characterBase != null)
            {
                characterBase.moveSpeed = 0f; // Đóng băng = tốc độ 0
            }
            
            Debug.Log($"Freeze effect applied for {duration}s! Player cannot move!");
        }
        else
        {
            // Nếu đã bị freeze thì làm mới thời gian
            frozenTimeLeft = duration;
            Debug.Log($"Freeze effect refreshed! Time reset to {duration}s");
        }
    }
    
    void RemoveFreezeEffect()
    {
        if (isFrozen)
        {
            isFrozen = false;
            
            // Cập nhật reference đến CharacterBase
            if (characterBase == null)
            {
                characterBase = GetComponent<CharacterBase>();
            }
            
            if (characterBase != null)
            {
                // Restore tốc độ dựa trên form hiện tại
                if (characterBase is NinjaCharacter)
                {
                    characterBase.moveSpeed = 4.5f; // Tốc độ Ninja
                }
                else if (characterBase is KnightCharacter)
                {
                    characterBase.moveSpeed = 2.5f; // Tốc độ Knight
                }
                else if (characterBase is NormalCharacter)
                {
                    characterBase.moveSpeed = 3f; // Tốc độ Normal
                }
                
                Debug.Log($"Freeze effect removed! Speed restored to: {characterBase.moveSpeed}");
            }
        }
    }
    
    void OnDisable()
    {
        // Khi component bị disable, xóa freeze
        if (isFrozen)
        {
            RemoveFreezeEffect();
        }
    }
    
    // Getter để kiểm tra trạng thái freeze (dùng cho UI hoặc animation)
    public bool IsFrozen() => isFrozen;
    public bool IsBurning() => isBurning;
}
