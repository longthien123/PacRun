using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.3f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float knockbackForce = 50f;
    [SerializeField] private GameObject hitEffect;

    private Transform playerTransform;
    private Vector2 direction;
    private float offsetDistance = 0.8f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (playerTransform != null)
        {
            Vector3 adjustedOffset = GetAdjustedOffset();
            transform.position = playerTransform.position + adjustedOffset;
        }
    }

    public void SetDirection(Vector2 dir, Transform player)
    {
        direction = dir.normalized;
        playerTransform = player;
        Vector3 adjustedOffset = GetAdjustedOffset();
        transform.position = playerTransform.position + adjustedOffset;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 270f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private Vector3 GetAdjustedOffset()
    {
        Vector3 offset = (Vector3)direction * offsetDistance;
        if (direction.x > 0) offset.y -= 0.25f;
        else if (direction.x < 0) offset.y -= 0.25f;
        else if (direction.y > 0) offset.x -= 0.2f;
        else if (direction.y < 0) offset.x -= 0.2f;
        return offset;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);

            // Tính sát thương (gấp đôi nếu upgrade active)
            int finalDamage = damage;
            if (PlayerController.Instance != null && PlayerController.Instance.isUpgradeActive)
            {
                finalDamage *= 2;
            }

            // Đẩy lùi Monster
            Rigidbody2D monsterRb = collision.GetComponent<Rigidbody2D>();
            if (monsterRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - playerTransform.position).normalized;
                monsterRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            // Kích hoạt trạng thái bị đẩy lùi
            var monsterAI = collision.GetComponent<MonsterAI>();
            var monsterWalkRandom = collision.GetComponent<MonsterWalkRandom>();
            var monsterFollow = collision.GetComponent<MonsterFollow>();
            var soulSpawner = collision.GetComponent<SoulSpawner>();
            var monsterShot = collision.GetComponent<MonsterShot>();
            var monsterShooter = collision.GetComponent<MonsterShooter>();
            
            if (monsterAI != null)
            {
                monsterAI.ApplyKnockback(0.5f);
                monsterAI.TakeDamage(finalDamage);
            }
            if (monsterWalkRandom != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - playerTransform.position).normalized;
                monsterWalkRandom.ApplyKnockback(knockbackDirection, knockbackForce, 0.5f);
                monsterWalkRandom.TakeDamage(finalDamage);
            }
            if (monsterFollow != null)
            {
                monsterFollow.TakeDamage(finalDamage);
            }
            if (soulSpawner != null)
            {
                soulSpawner.TakeDamage(finalDamage);
            }
            if (monsterShot != null)
            {
                monsterShot.TakeDamage(finalDamage);
            }
            if (monsterShooter != null)
            {
                monsterShooter.TakeDamage(finalDamage);
            }
        }
    }
}
