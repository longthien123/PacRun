using UnityEngine;
using UnityEngine.UI;

public class SoulSpawner : MonoBehaviour
{
    public GameObject[] monsterPrefabs;
    public Transform spawnPoint;
    public float spawnInterval = 3f;
    private float timer;
    private Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private Animator animator;
    
    [Header("Health System")]
    private int health = 150;
    private int maxHealth;
    [SerializeField] private Slider healthSlider;
    void Start()
    {
        animator = GetComponent<Animator>();
        timer = spawnInterval;
        
        // Khởi tạo health bar
        maxHealth = health;
        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>(true);
        if (healthSlider == null)
            Debug.LogWarning($"{gameObject.name}: Không tìm thấy Slider component!");
        UpdateHealthBar();
        
        SpawnMonster();
        if (spawnPoint == null)
            spawnPoint = transform;
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
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnMonster();
            timer = spawnInterval;
        }
    }

    void SpawnMonster()
    {
        GameObject prefab = monsterPrefabs[0];
        GameObject monster = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        Vector2 dir = directions[Random.Range(0, directions.Length)];
        var animator = monster.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("moveX", dir.x);
            animator.SetFloat("moveY", dir.y);
        }
    }
    public void TakeDamage(int dmg)
    {
        animator.SetTrigger("attacked");
        health -= dmg;
        health = Mathf.Max(0, health);
        UpdateHealthBar();
        Debug.Log($"{gameObject.name} took {dmg} dmg. HP = {health}");
        if (health <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        Instantiate(ManagePlayer.Instance.fallKey, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
