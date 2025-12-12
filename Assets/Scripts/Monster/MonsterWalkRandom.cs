using UnityEngine;
using UnityEngine.UI;

public class MonsterWalkRandom : MonoBehaviour
{
    public float moveSpeed = 2f;           // Tốc độ di chuyển
    public LayerMask wallLayer;           // Layer của tường
    public float changeDirInterval = 2f;  // Khoảng thời gian tự đổi hướng

    [Header("Health System")]
    public int health = 40; // máu của WalkRandom
    private int maxHealth;
    [SerializeField] private Slider healthSlider;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material flashMaterial;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveDirection;
    private Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private float changeDirTimer;
    private float attackCooldownTimer = 0f;
    public float attackCooldown = 0.5f; // Thời gian hồi chiêu giữa các lần tấn công

    // Thêm biến để kiểm soát trạng thái bị đẩy lùi
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    [SerializeField] private GameObject monsterAI;
    [SerializeField] private GameObject plantEffect;
    
    // Item Drop System
    [Header("Item Drop Settings")]
    [SerializeField] private GameObject[] dropItems; // Mảng các item có thể drop
    [SerializeField] private float dropChance = 0.2f; // 20% tỷ lệ drop
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && defaultMaterial == null)
            defaultMaterial = spriteRenderer.material;

        rb.freezeRotation = true;

        // Khởi tạo health bar
        maxHealth = health;
        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>(true);
        if (healthSlider == null)
            Debug.LogWarning($"{gameObject.name}: Không tìm thấy Slider component!");
        UpdateHealthBar();

        ChooseNewDirection(); // Hướng ngẫu nhiên ban đầu
        changeDirTimer = changeDirInterval;
    }

    void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    void Update()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
                isKnockedBack = false;
            return;
        }

        changeDirTimer -= Time.deltaTime;
        if (changeDirTimer <= 0f)
        {
            ChooseNewDirection();
            changeDirTimer = Random.Range(1.5f, 3f);
        }

        if (IsWallAhead())
            ChooseNewDirection();

        animator.SetFloat("moveX", moveDirection.x);
        animator.SetFloat("moveY", moveDirection.y);

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isKnockedBack) return;
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    bool IsWallAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, 0.6f, wallLayer);
        return hit.collider != null;
    }

    void ChooseNewDirection()
    {
        Vector2 newDir;
        int safety = 0;
        do
        {
            newDir = directions[Random.Range(0, directions.Length)];
            safety++;
        } while (IsWallInDirection(newDir) && safety < 10);

        moveDirection = newDir;
    }

    bool IsWallInDirection(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.6f, wallLayer);
        return hit.collider != null;
    }

    public void ApplyKnockback(Vector2 knockbackDirection, float knockbackForce, float duration)
    {
        isKnockedBack = true;
        knockbackTimer = duration;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && attackCooldownTimer <= 0f 
        && PlayerController.Instance.isShieldActive == false)
        {
            AttackPlayer(collision.gameObject);
            attackCooldownTimer = attackCooldown;
        }
    }

    void AttackPlayer(GameObject playerObj)
    {
        // damage: nếu Player là Knight thì 10, else 20
        var cb = playerObj.GetComponent<CharacterBase>();
        if (cb != null)
        {
            int dmg = (cb is KnightCharacter) ? 10 : 20;
            cb.TakeDamage(dmg);
        }

        // animation attack theo hướng đến player
        Vector2 attackDirection = (playerObj.transform.position - transform.position).normalized;
        animator.SetFloat("lastMoveX", attackDirection.x);
        animator.SetFloat("lastMoveY", attackDirection.y);
        animator.SetBool("attack", true);
        StartCoroutine(ResetAttackAfterDelay(0.5f));
    }

    private System.Collections.IEnumerator ResetAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("attack", false);
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        health = Mathf.Max(0, health);
        UpdateHealthBar();
        StartCoroutine(FlashWhite());
        if (health <= 0) {
            ManagePlayer.Instance.AddMonsterKill(transform.position);
            Die();
        }
    }

    private System.Collections.IEnumerator FlashWhite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.material = flashMaterial != null ? flashMaterial : spriteRenderer.material;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.material = defaultMaterial;
        }
    }

    public void Die()
    {   
        animator.SetBool("attack", false);
        Debug.Log($"{gameObject.name} died.");
        animator.SetBool("dead", true);
        // Drop item với tỷ lệ 25%
        TryDropItem();
        Destroy(gameObject, 0.3f);
    }
    
    void TryDropItem()
    {
        // Kiểm tra xem có drop item không (25% chance)
        if (Random.value <= dropChance)
        {
            if (dropItems != null && dropItems.Length > 0)
            {
                // Chọn ngẫu nhiên một item từ mảng
                GameObject randomItem = dropItems[Random.Range(0, dropItems.Length)];
                
                if (randomItem != null)
                {
                    Instantiate(randomItem, transform.position, Quaternion.identity);
                }
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Plant"))
        {
            Instantiate(plantEffect, transform.position, Quaternion.identity);
            Instantiate(monsterAI, transform.position, Quaternion.identity);
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}
