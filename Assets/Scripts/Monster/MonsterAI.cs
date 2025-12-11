using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float randomSpeed = 2f;
    public float chaseSpeed = 2.4f;
    public float chaseRange = 4f;
    public LayerMask wallLayer;

    [Header("A* Settings")]
    public float pathUpdateInterval = 0.5f;

    public int health = 60;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material flashMaterial;

    private Rigidbody2D rb;
    private Animator animator;
    private GameObject player;
    private Tilemap wallTilemap;

    private Vector2 moveDirection;
    private Vector2[] directions = {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right
    };

    private float changeDirTimer;
    private float pathUpdateTimer;
    private bool isChasing = false;

    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    private float attackCooldownTimer = 0f;
    public float attackCooldown = 0.5f;

    [Header("Item Drop Settings")]
    [SerializeField] private GameObject[] dropItems; // Mảng các item có thể drop
    [SerializeField] private float dropChance = 0.25f; // 30% tỷ lệ drop

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && defaultMaterial == null)
            defaultMaterial = spriteRenderer.material;
        player = GameObject.FindGameObjectWithTag("Player");
        wallTilemap = GameObject.FindGameObjectWithTag("Wall").GetComponent<Tilemap>();

        rb.freezeRotation = true;

        ChooseNewDirection();
        changeDirTimer = Random.Range(1.5f, 3f);
        pathUpdateTimer = 0.5f;
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

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        pathUpdateTimer -= Time.deltaTime;
        changeDirTimer -= Time.deltaTime;

        if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;
            if (pathUpdateTimer <= 0f)
            {
                Vector3 nextPos = GetNextAStarStep(transform.position, player.transform.position);
                moveDirection = (nextPos - transform.position).normalized;
                Debug.DrawLine(transform.position, nextPos, Color.red, 0.5f);
                pathUpdateTimer = pathUpdateInterval;
            }
        }
        else
        {
            isChasing = false;
            if (changeDirTimer <= 0f || IsWallAhead())
            {
                ChooseNewDirection();
                changeDirTimer = Random.Range(1.5f, 3f);
            }
        }

        animator.SetFloat("moveX", moveDirection.x);
        animator.SetFloat("moveY", moveDirection.y);

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isKnockedBack) return;
        float currentSpeed = isChasing ? chaseSpeed : randomSpeed;
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
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

    Vector3 GetNextAStarStep(Vector3 startPos, Vector3 targetPos)
    {
        Vector3Int startCell = wallTilemap.WorldToCell(startPos);
        Vector3Int targetCell = wallTilemap.WorldToCell(targetPos);

        List<Vector3Int> path = AStarSearch(startCell, targetCell);
        if (path.Count > 1)
            return wallTilemap.GetCellCenterWorld(path[1]);
        return transform.position;
    }

    List<Vector3Int> AStarSearch(Vector3Int start, Vector3Int goal)
    {
        List<Vector3Int> openSet = new List<Vector3Int> { start };
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float> { [start] = 0 };
        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float> { [start] = Heuristic(start, goal) };

        while (openSet.Count > 0)
        {
            openSet.Sort((a, b) => fScore[a].CompareTo(fScore[b]));
            Vector3Int current = openSet[0];

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.RemoveAt(0);

            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if (IsWall(neighbor)) continue;

                float tentativeG = gScore[current] + 1;
                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<Vector3Int> { start };
    }

    List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> totalPath = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    List<Vector3Int> GetNeighbors(Vector3Int cell)
    {
        List<Vector3Int> result = new List<Vector3Int>
        {
            cell + new Vector3Int(1, 0, 0),
            cell + new Vector3Int(-1, 0, 0),
            cell + new Vector3Int(0, 1, 0),
            cell + new Vector3Int(0, -1, 0)
        };
        return result;
    }

    bool IsWall(Vector3Int cell)
    {
        return wallTilemap.HasTile(cell);
    }

    public void ApplyKnockback(float duration)
    {
        isKnockedBack = true;
        knockbackTimer = duration;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && attackCooldownTimer <= 0f)
        {
            // Kiểm tra khiên trước khi gây sát thương
            if (PlayerController.Instance.isShieldActive)
            {
                return;
            }
            
            // damage: Knight 20, others 30
            var cb = collision.gameObject.GetComponent<CharacterBase>();
            if (cb != null)
            {
                int dmg = (cb is KnightCharacter) ? 20 : 30;
                cb.TakeDamage(dmg);
            }

            // animation attack based on moveDirection
            animator.SetFloat("lastMoveX", moveDirection.x);
            animator.SetFloat("lastMoveY", moveDirection.y);
            animator.SetBool("attack", true);
            StartCoroutine(ResetAttackAfterDelay(0.5f));

            attackCooldownTimer = attackCooldown;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Plant"))
        {
            health += 10;
            Destroy(collision.gameObject);
        }
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
        StartCoroutine(FlashWhite());
        if (health <= 0){ Die();
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
        animator.SetBool("dead", true);
        TryDropItem();
        Destroy(gameObject,0.3f);
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

}
