using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantSpawner : MonoBehaviour
{
    public GameObject plantPrefab;
    public Tilemap wallTilemap;
    public Vector2Int mapMin;
    public Vector2Int mapMax;

    public float spawnInterval = 5f;
    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnPlant();
            timer = spawnInterval;
        }
    }

    void SpawnPlant()
    {
        int safety = 0;
        while (safety < 100)
        {
            safety++;
            int x = Random.Range(mapMin.x, mapMax.x + 1);
            int y = Random.Range(mapMin.y, mapMax.y + 1);
            Vector3Int cell = new Vector3Int(x, y, 0);

            if (!wallTilemap.HasTile(cell))
            {
                Vector3 worldPos = wallTilemap.GetCellCenterWorld(cell);
                Instantiate(plantPrefab, worldPos, Quaternion.identity);
                break;
            }
        }
    }
}
