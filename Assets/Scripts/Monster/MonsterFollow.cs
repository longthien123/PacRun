using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterFollow : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.5f;
    public float chaseRange = 10f;
    public LayerMask wallLayer;

    [Header("A* Settings")]
    public float pathUpdateInterval = 0.5f;

    [Header("Stats")]
    public int health = 10;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private GameObject deathEffect;

    private Rigidbody2D rb;
    private Animator animator;
    private GameObject player;
    private Tilemap wallTilemap;
    private Vector2 moveDirection;
    private float pathUpdateTimer;

    [Header("Item Drop Settings")]
    [SerializeField] private GameObject[] dropItems; // Mảng các item có thể drop
    [SerializeField] private float dropChance = 0.15f;

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
        pathUpdateTimer = 0f;
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            
            if (distanceToPlayer <= chaseRange)
            {
                pathUpdateTimer -= Time.deltaTime;
                if (pathUpdateTimer <= 0f)
                {
                    Vector3 nextPos = GetNextAStarStep(transform.position, player.transform.position);
                    moveDirection = (nextPos - transform.position).normalized;
                    Debug.DrawLine(transform.position, nextPos, Color.green, 0.5f);
                    pathUpdateTimer = pathUpdateInterval;
                }

                // Cập nhật animation
                animator.SetFloat("moveX", moveDirection.x);
                animator.SetFloat("moveY", moveDirection.y);
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Kiểm tra khiên trước khi gây sát thương
            if (PlayerController.Instance.isShieldActive)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
                return;
            }
            
            var cb = collision.gameObject.GetComponent<CharacterBase>();
            if (cb != null)
            {
                int dmg = (cb is KnightCharacter) ? 10 : 15;
                cb.TakeDamage(dmg);
            }
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Plant"))
        {
            moveSpeed += 0.5f;
            Instantiate(PlayerController.Instance.effectCollectKey, transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        health = Mathf.Max(0, health);
        StartCoroutine(FlashWhite());
        if (health <= 0)
        {
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
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        TryDropItem();
        Destroy(gameObject);
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
