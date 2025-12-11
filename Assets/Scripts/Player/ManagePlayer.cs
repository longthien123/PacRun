using UnityEngine;

public class ManagePlayer : MonoBehaviour
{
    public static ManagePlayer Instance;
    [Header("Progress Counters")]
    public int keysCollected = 0;
    public int monstersKilled = 0;
    public int chestsOpened = 0;
    private int keysDropped = 0; // Số chìa đã rơi
    
    [Header("Level Settings")]
    public int currentLevel = 1; // Level hiện tại (1, 2, 3)
    
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
        
        // Lấy thông số theo level
        int monstersPerKey = GetMonstersPerKey();
        int maxKeys = GetMaxKeys();
        
        // Kiểm tra xem đã đến milestone chưa và chưa vượt quá max keys
        if (monstersKilled % monstersPerKey == 0 && keysDropped < maxKeys)
        {
            keysDropped++;
            Instantiate(fallKey, deathPosition, Quaternion.identity);
            Debug.Log($"Level {currentLevel}: Monster kill milestone reached at {monstersKilled} kills. Key {keysDropped}/{maxKeys} spawned.");
        }
    }
    
    // Lấy số quái cần giết để rơi 1 chìa theo level
    private int GetMonstersPerKey()
    {
        switch (currentLevel)
        {
            case 1: return 4;  // Level 1: 4 quái = 1 chìa
            case 2: return 6;  // Level 2: 6 quái = 1 chìa
            case 3: return 8;  // Level 3: 8 quái = 1 chìa
            default: return 4;
        }
    }
    
    // Lấy số chìa tối đa theo level
    private int GetMaxKeys()
    {
        switch (currentLevel)
        {
            case 1: return 4;  // Level 1: max 4 chìa
            case 2: return 6;  // Level 2: max 6 chìa
            case 3: return 8;  // Level 3: max 8 chìa
            default: return 4;
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
