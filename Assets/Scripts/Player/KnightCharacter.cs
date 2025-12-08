using UnityEngine;

public class KnightCharacter : CharacterBase
{
    PlayerController playerController;

    void Start()
    {
        moveSpeed = 2.5f;  // chậm nhất
              

        // Load sword prefab từ PlayerController
        playerController = GetComponent<PlayerController>();
    }

    public override void Attack()
    {
        Debug.Log("Knight slashes with sword!");
    }

    // Hàm tấn công bằng kiếm
    public void SlashSword(Vector2 direction)
    {
        if (playerController.swordPrefab == null)
        {
            Debug.LogError("Sword prefab not assigned!");
            return;
        }

        // Tạo sword
        GameObject sword = Instantiate(playerController.swordPrefab, transform.position, Quaternion.identity);

        // Set vị trí và hướng (truyền Transform của player)
        Sword swordScript = sword.GetComponent<Sword>();
        if (swordScript != null)
        {
            swordScript.SetDirection(direction, transform);
        }
    }
}
