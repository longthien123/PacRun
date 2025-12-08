using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] monsterPrefabs;
    public Transform spawnPoint;
    public float spawnInterval = 3f;
    [SerializeField] private int maxMonsters = 7;
    private float timer;
    private Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    void Start()
    {
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
        // Đếm số lượng quái vật hiện có
        int monsterCount = FindObjectsOfType<MonsterWalkRandom>().Length
                         + FindObjectsOfType<MonsterAI>().Length;
        if (monsterCount >= maxMonsters) return;

        if (monsterPrefabs.Length == 0) return;

        GameObject prefab = monsterPrefabs[0];
        GameObject monster = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        Vector2 dir = directions[Random.Range(0, directions.Length)];
        var animator = monster.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("moveX", dir.x);
            animator.SetFloat("moveY", dir.y);
            animator.SetFloat("lastMoveX", dir.x);
            animator.SetFloat("lastMoveY", dir.y);
        }
    }
}
