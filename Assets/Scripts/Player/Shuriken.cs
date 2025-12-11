using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 15;
    
    [SerializeField] private GameObject destroyEffect;
    
    private Vector2 direction;
    private Rigidbody2D rb;
    private bool isInitialized = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Tự hủy sau một thời gian
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        // Chỉ di chuyển khi đã set direction
        if (isInitialized)
        {
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
    }

    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Monster"))
        {
            if (collision.collider.CompareTag("Monster"))
            {
                // Tính sát thương (gấp đôi nếu upgrade active)
                int finalDamage = damage;
                if (PlayerController.Instance != null && PlayerController.Instance.isUpgradeActive)
                {
                    finalDamage *= 2;
                }
                
                var monsterAI = collision.collider.GetComponent<MonsterAI>();
                var monsterWalkRandom = collision.collider.GetComponent<MonsterWalkRandom>();
                var monsterFollow = collision.collider.GetComponent<MonsterFollow>();
                var soulSpawner = collision.collider.GetComponent<SoulSpawner>();
                var monsterShot = collision.collider.GetComponent<MonsterShot>();
                var monsterShooter = collision.collider.GetComponent<MonsterShooter>();
                if (monsterAI != null)
                    monsterAI.TakeDamage(finalDamage);
                if (monsterWalkRandom != null)
                    monsterWalkRandom.TakeDamage(finalDamage);
                if (monsterFollow != null)
                    monsterFollow.TakeDamage(finalDamage);
                if (soulSpawner != null)
                    soulSpawner.TakeDamage(finalDamage);
                if (monsterShot != null)
                    monsterShot.TakeDamage(finalDamage);
                if (monsterShooter != null)
                    monsterShooter.TakeDamage(finalDamage);
            }
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
