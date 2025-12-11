using UnityEngine;

public class MonsterShooter : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.5f;
    public LayerMask wallLayer;
    public float changeDirInterval = 2f;

    [Header("Combat Settings")]
    public int health = 300;
    public float detectionRange = 6f;
    public float shootCooldown = 2f;
    public float shootAnimationDuration = 2f;
    
    [Header("Shot Prefabs")]
    [SerializeField] private GameObject[] shotPrefabs; // Mảng chứa Red và Blue shot prefabs
    [SerializeField] private Transform shootPoint;

    [Header("Materials")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material flashMaterial;
    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Animator animator;
    private GameObject player;
    private Vector2 moveDirection;
    private Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private float changeDirTimer;
    private float shootCooldownTimer = 0f;
    private bool isShooting = false;
    private float shootAnimationTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (spriteRenderer != null && defaultMaterial == null)
            defaultMaterial = spriteRenderer.material;

        rb.freezeRotation = true;

        ChooseNewDirection();
        changeDirTimer = changeDirInterval;
        shootCooldownTimer = 0f;
    }

    void Update()
    {
        // Cập nhật timer animation bắn
        if (shootAnimationTimer > 0f)
        {
            shootAnimationTimer -= Time.deltaTime;
            if (shootAnimationTimer <= 0f)
            {
                isShooting = false;
            }
        }

        // Chỉ di chuyển khi không đang bắn
        if (!isShooting)
        {
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
        }

        // Cập nhật cooldown timer
        if (shootCooldownTimer > 0f)
        {
            shootCooldownTimer -= Time.deltaTime;
        }

        // Kiểm tra có thể bắn player không
        if (!isShooting && shootCooldownTimer <= 0f && player != null && CanShootPlayer())
        {
            ShootAtPlayer();
        }
    }

    void FixedUpdate()
    {
        // Chỉ di chuyển khi không đang bắn
        if (!isShooting)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
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

    bool CanShootPlayer()
    {
        if (player == null) return false;

        Vector2 dirToPlayer = player.transform.position - transform.position;
        float distance = dirToPlayer.magnitude;

        if (distance > detectionRange) return false;

        bool isHorizontalAligned = Mathf.Abs(dirToPlayer.y) < 0.5f;
        bool isVerticalAligned = Mathf.Abs(dirToPlayer.x) < 0.5f;

        if (!isHorizontalAligned && !isVerticalAligned) return false;

        Vector2 shootDir = Vector2.zero;
        if (isHorizontalAligned)
            shootDir = dirToPlayer.x > 0 ? Vector2.right : Vector2.left;
        else if (isVerticalAligned)
            shootDir = dirToPlayer.y > 0 ? Vector2.up : Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, shootDir, distance, wallLayer);
        if (hit.collider != null) return false;

        return true;
    }

    void ShootAtPlayer()
    {
        if (shotPrefabs == null || shotPrefabs.Length == 0)
        {
            Debug.LogWarning("No shot prefabs assigned!");
            return;
        }

        isShooting = true;
        shootAnimationTimer = shootAnimationDuration;
        shootCooldownTimer = shootCooldown;

        Vector2 dirToPlayer = player.transform.position - transform.position;
        
        Vector2 shootDir = Vector2.zero;
        bool isHorizontalAligned = Mathf.Abs(dirToPlayer.y) < 0.5f;
        bool isVerticalAligned = Mathf.Abs(dirToPlayer.x) < 0.5f;

        if (isHorizontalAligned)
            shootDir = dirToPlayer.x > 0 ? Vector2.right : Vector2.left;
        else if (isVerticalAligned)
            shootDir = dirToPlayer.y > 0 ? Vector2.up : Vector2.down;

        Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
        
        // Chọn ngẫu nhiên 1 prefab từ mảng
        int randomIndex = Random.Range(0, shotPrefabs.Length);
        GameObject selectedPrefab = shotPrefabs[randomIndex];
        
        if (selectedPrefab == null)
        {
            Debug.LogWarning($"Shot prefab at index {randomIndex} is null!");
            return;
        }
        
        GameObject shot = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
        MonsterShot shotScript = shot.GetComponent<MonsterShot>();
        
        if (shotScript != null)
        {
            shotScript.SetDirection(shootDir);
            Debug.Log($"Shot spawned: {selectedPrefab.name}");
        }

        animator.SetFloat("lastMoveX", shootDir.x);
        animator.SetFloat("lastMoveY", shootDir.y);
        animator.SetBool("attack", true);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerController.Instance != null && PlayerController.Instance.isShieldActive)
            {
                Debug.Log("Shield blocked MonsterShooter attack!");
                return;
            }

            var cb = collision.gameObject.GetComponent<CharacterBase>();
            if (cb != null)
            {
                int dmg = (cb is KnightCharacter) ? 10 : 15;
                cb.TakeDamage(dmg);
            }

            Vector2 attackDirection = (collision.gameObject.transform.position - transform.position).normalized;
            animator.SetFloat("lastMoveX", attackDirection.x);
            animator.SetFloat("lastMoveY", attackDirection.y);
            animator.SetTrigger("attack");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Plant"))
        {
            health += 10;
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        health = Mathf.Max(0, health);
        StartCoroutine(FlashWhite());
        Debug.Log($"{gameObject.name} took {dmg} dmg. HP = {health}");
        if (health <= 0)
        {
            if (ManagePlayer.Instance != null)
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
        animator.SetTrigger("dead");
        Debug.Log($"{gameObject.name} died.");
        Instantiate(ManagePlayer.Instance.fallKey, transform.position, Quaternion.identity);
        Destroy(gameObject, 2.5f);
    }
}
