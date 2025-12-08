using UnityEngine;

public class ManagePlayer : MonoBehaviour
{
    public static ManagePlayer Instance;
    [Header("Progress Counters")]
    public int keysCollected = 0;
    public int monstersKilled = 0;
    public int chestsOpened = 0;
    

    [Header("Required to unlock portal")]
    
    public int requiredChests = 4;

    [Header("Portal Object")]
    public GameObject portal;
    public GameObject fallKey;

    void Awake()
    {
        // Kiểm tra xem đã có Instance nào chưa
        if (Instance != null && Instance != this)
        {
            // Nếu có rồi, hủy đối tượng hiện tại
            Destroy(this.gameObject);
        }
        else
        {
            // Thiết lập đối tượng hiện tại làm Instance
            Instance = this;
            portal.SetActive(true); // Cổng mặc định đóng
            // Nếu Manager cần tồn tại qua các Scene:
            // DontDestroyOnLoad(this.gameObject); 
        }
    }
    public void AddKey()
    {
        keysCollected++;
        Debug.Log("Keys collected: " + keysCollected);
        if (keysCollected == 4)
        {
            Debug.Log("All keys collected!");
        }
    }
    public void AddMonsterKill(Vector3 deathPosition)
    {
        monstersKilled++;
    if (monstersKilled == 4 || monstersKilled == 8 || monstersKilled == 12 || monstersKilled == 16)
    {
        Instantiate(fallKey, deathPosition, Quaternion.identity);
        Debug.Log($"Monster kill milestone reached: {monstersKilled}. Fall key spawned at {deathPosition}.");
    }
    }
    public void AddChestOpened()
    {
        if(keysCollected < 0){
            Debug.LogWarning("Keys collected is negative!");
            return;
        }else{
            
            Instantiate(PlayerController.Instance.effectCollectKey, transform.position, Quaternion.identity);
            chestsOpened++;
            keysCollected--;   
            Debug.Log("Chests opened: " + chestsOpened + " / " + requiredChests);
            if (chestsOpened >= requiredChests)
            {
                UnlockPortal();
            }
        }
    }
    public void AddChestOpened(ChestAnimatorController chestAnimator)
    {
            Instantiate(PlayerController.Instance.effectCollectKey, transform.position, Quaternion.identity);
            chestsOpened++;
            keysCollected--;
            Debug.Log("Chests opened: " + chestsOpened + " / " + requiredChests);
            if (chestsOpened >= requiredChests)
            {
                UnlockPortal();
            }
            // Mở rương tại đây
            if (chestAnimator != null && !chestAnimator.isOpened)
            {
                chestAnimator.OpenChest();
            }
    }
     private void UnlockPortal()
    {
        portal.SetActive(false);

        Debug.Log("Portal opened!");
    }
}
