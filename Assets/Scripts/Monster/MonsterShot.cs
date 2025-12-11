using UnityEngine;

public enum ShotType
{
    Red,    // Đốt cháy
    Blue    // Làm chậm
}

public class MonsterShot : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 15;
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private float lifetime = 6f;
    [SerializeField] private float slideSpeedMultiplier = 0.3f;
    [SerializeField] private float slideTimeWindow = 2f;
    
    [Header("Shot Type")]
    [SerializeField] private ShotType shotType = ShotType.Red;
    
    private Vector2 direction;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isInitialized = false;
    private bool isSliding = false;
    private float spawnTime;
    public int health = 10;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    
    void Start()
    {
        // Tự hủy sau một thời gian
        Destroy(gameObject, lifetime);
        spawnTime = Time.time;
    }
    
    void FixedUpdate()
    {
        if (!isInitialized) return;
        
        if (!isSliding)
        {
            // Di chuyển bình thường
            rb.linearVelocity = direction * speed;
        }
    }
    
    // Set hướng bay của phi tiêu
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        isInitialized = true;
        
        // Set velocity ngay lập tức
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
        
        // Set animation direction nếu có
        if (animator != null)
        {
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
        }
    }
    
    public void SetShotType(ShotType type)
    {
        shotType = type;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Nếu va chạm với tường
        if (collision.gameObject.CompareTag("Wall"))
        {
            float timeSinceSpawn = Time.time - spawnTime;
            
            // Nếu đã quá 2 giây kể từ khi spawn thì destroy
            if (timeSinceSpawn > slideTimeWindow)
            {
                if (destroyEffect != null)
                    Instantiate(destroyEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
                return;
            }
            
            // Nếu chưa quá 2 giây thì trượt
            isSliding = true;
            
            // Lấy contact point để tính hướng trượt
            ContactPoint2D contact = collision.contacts[0];
            Vector2 wallNormal = contact.normal;
            
            // Tính hướng trượt dọc theo tường (perpendicular to normal)
            Vector2 slideDirection = Vector2.Perpendicular(wallNormal);
            
            // Chọn hướng trượt phù hợp với hướng ban đầu
            if (Vector2.Dot(slideDirection, direction) < 0)
            {
                slideDirection = -slideDirection;
            }
            
            // Áp dụng lực trượt nhẹ
            rb.linearVelocity = slideDirection * speed * slideSpeedMultiplier;
            
            Debug.Log($"MonsterShot sliding along wall. Time: {timeSinceSpawn:F2}s");
            return;
        }
        
        // Nếu va chạm với Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Kiểm tra khiên trước khi gây sát thương
            if (PlayerController.Instance != null && PlayerController.Instance.isShieldActive)
            {
                Debug.Log("Shield blocked MonsterShot!");
                if (destroyEffect != null)
                    Instantiate(destroyEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
                return;
            }
            
            var cb = collision.gameObject.GetComponent<CharacterBase>();
            if (cb != null)
            {
                // Gây sát thương cơ bản
                int dmg = (cb is KnightCharacter) ? 10 : 15;
                cb.TakeDamage(dmg);
                
                // Áp dụng hiệu ứng đặc biệt
                ApplySpecialEffect(collision.gameObject);
            }
            
            if (destroyEffect != null)
                Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    
    void ApplySpecialEffect(GameObject player)
    {
        var effectHandler = player.GetComponent<PlayerEffectHandler>();
        if (effectHandler == null)
        {
            effectHandler = player.AddComponent<PlayerEffectHandler>();
        }
        
        if (shotType == ShotType.Red)
        {
            // Đốt cháy: 10 damage/giây trong 3 giây
            effectHandler.ApplyBurnEffect(10, 3f);
            Debug.Log("Applied BURN effect!");
        }
        else if (shotType == ShotType.Blue)
        {
            // Đóng băng 1 giây
            effectHandler.ApplyFreezeEffect(1f);
            Debug.Log("Applied FREEZE effect!");
        }
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        // Tiếp tục trượt khi vẫn chạm tường
        if (collision.gameObject.CompareTag("Wall") && isSliding)
        {
            float timeSinceSpawn = Time.time - spawnTime;
            
            // Nếu đã quá 2 giây thì destroy
            if (timeSinceSpawn > slideTimeWindow)
            {
                if (destroyEffect != null)
                    Instantiate(destroyEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
                return;
            }
            
            ContactPoint2D contact = collision.contacts[0];
            Vector2 wallNormal = contact.normal;
            Vector2 slideDirection = Vector2.Perpendicular(wallNormal);
            
            if (Vector2.Dot(slideDirection, rb.linearVelocity) < 0)
            {
                slideDirection = -slideDirection;
            }
            
            rb.linearVelocity = slideDirection * speed * slideSpeedMultiplier;
        }
    }
    
    public void TakeDamage(int dmg)
    {
        health -= dmg;
        health = Mathf.Max(0, health);
        if (health <= 0)
        {
            Die();
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        // Khi rời khỏi tường, quay lại di chuyển bình thường
        if (collision.gameObject.CompareTag("Wall"))
        {
            isSliding = false;
            rb.linearVelocity = direction * speed;
        }
    }
    
    public void Die()
    {
        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
