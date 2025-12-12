using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public float moveSpeed;

    [Header("Material Flash")]
    public Material defaultMaterial;
    public Material flashMaterial;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Attack()
    {
    }

    public virtual void TakeDamage(int damage)
    {
        // Kiểm tra nếu có khiên thì không nhận sát thương
        if (PlayerController.Instance != null && PlayerController.Instance.isShieldActive)
        {
            return;
        }
        
        PlayerHealthManager.TakeDamage(damage);
        StartCoroutine(FlashMaterial());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Plant"))
        {
            Instantiate(PlayerController.Instance.effectCollectKey, transform.position, Quaternion.identity);
            PlayerHealthManager.Heal(10);
            if(PlayerController.Instance.hasNinjaItem && PlayerController.Instance.ninjaTimeLeft <= 20f)
            {
                PlayerController.Instance.ninjaTimeLeft += 2f;
                Destroy(collision.gameObject);
            }
            if(PlayerController.Instance.hasKnightItem && PlayerController.Instance.knightTimeLeft <= 20f)
            {
                PlayerController.Instance.knightTimeLeft += 2f;
                Destroy(collision.gameObject);
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }
        else if(collision.gameObject.CompareTag("Key"))
        {   
            Instantiate(PlayerController.Instance.effectCollectKey, transform.position, Quaternion.identity);
            if(ManagePlayer.Instance != null)
            {
                ManagePlayer.Instance.AddKey();
                Destroy(collision.gameObject);
            }
            else
            {
                Debug.LogWarning("ManagePlayer Instance is null!");
            }
        }
        else if (collision.gameObject.CompareTag("Ruong_Keys"))
        {
            ChestAnimatorController chestAnimator = collision.gameObject.GetComponent<ChestAnimatorController>();
            if (chestAnimator != null && !chestAnimator.isOpened)
            {
                if (ManagePlayer.Instance != null && ManagePlayer.Instance.keysCollected > 0)
                {
                    ManagePlayer.Instance.AddChestOpened(chestAnimator);
                }
                else
                {
                    Debug.Log("Bạn không có chìa khóa để mở rương!");
                }
            }
        }
        else if (collision.gameObject.CompareTag("Ruong_Items"))
        {
            ChestItemAnimation chestItemAnimation = collision.gameObject.GetComponent<ChestItemAnimation>();
            if (chestItemAnimation != null && !chestItemAnimation.isOpened)
            {
                chestItemAnimation.OpenChest();
            }
        }
        else if (collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("Bạn đã vào cổng và hoàn thành cấp độ!");
        }
        else if (collision.gameObject.CompareTag("NinjaItem"))
        {
            PlayerController.Instance.CollectNinjaItem();
            Instantiate(PlayerController.Instance.effectCollectKey, transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("KnightItem"))
        {
            PlayerController.Instance.CollectKnightItem();
            Instantiate(PlayerController.Instance.effectCollectKey, transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("ShieldItem"))
        {
            PlayerController.Instance.CollectShieldItem();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("UpgradeItem"))
        {
            PlayerController.Instance.CollectUpgradeItem();
            Destroy(collision.gameObject);
        }
        
    }

    private System.Collections.IEnumerator FlashMaterial()
    {
        if (spriteRenderer != null && flashMaterial != null)
        {
            spriteRenderer.material = flashMaterial;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.material = defaultMaterial;
        }
    }

    public virtual void Die()
    {
        // xử lý chết nếu cần (chung hoặc riêng)
    }
}
