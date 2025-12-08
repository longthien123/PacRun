using UnityEngine;

public class NinjaCharacter : CharacterBase
{
    PlayerController playerController;
    [SerializeField] private Transform throwPoint; // Vị trí ném từ player
    
    void Start()
    {
        moveSpeed = 4.5f;  // Ninja chạy nhanh
        playerController = GetComponent<PlayerController>();
    }

    public override void Attack()
    {
        Debug.Log("Ninja throws shuriken!");
    }
    
    // Hàm ném shuriken (gọi từ PlayerController)
    public void ThrowShuriken(Vector2 direction)
    {
        if (playerController.shurikenPrefab == null)
        {
            Debug.LogError("Shuriken prefab not assigned!");
            return;
        }
        playerController = GetComponent<PlayerController>();
        // Spawn vị trí từ player hoặc throwPoint
        Vector3 spawnPos = throwPoint != null ? throwPoint.position : transform.position;
        
        // Tạo shuriken
        GameObject shuriken = Instantiate(playerController.shurikenPrefab, spawnPos, Quaternion.identity);
        
        // Set hướng bay
        Shuriken shurikenScript = shuriken.GetComponent<Shuriken>();
        if (shurikenScript != null)
        {
            shurikenScript.SetDirection(direction);
        }
    }
}
