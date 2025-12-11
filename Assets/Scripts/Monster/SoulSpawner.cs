using UnityEngine;

public class SoulSpawner : MonoBehaviour
{
    public GameObject[] monsterPrefabs;
    public Transform spawnPoint;
    public float spawnInterval = 3f;
    private float timer;
    private Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private Animator animator;
    private int health=150;
    void Start()
    {
        animator = GetComponent<Animator>();
        timer = spawnInterval;
        SpawnMonster();
        if (spawnPoint == null)
            spawnPoint = transform;
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
