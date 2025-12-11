using UnityEngine;

public class Plant : MonoBehaviour
{
    [Header("Lifetime Settings")]
    [SerializeField] private float lifetime = 10f; // Thời gian tồn tại (giây)
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject disappearEffect; // Effect khi biến mất
    
    private float timer;
    
    void Start()
    {
        timer = lifetime;
    }
    
    void Update()
    {
        timer -= Time.deltaTime;
        
        // Khi hết thời gian thì biến mất
        if (timer <= 0f)
        {
            Disappear();
        }
    }
    
    void Disappear()
    {
        // Spawn effect nếu có
        if (disappearEffect != null)
        {
            Instantiate(disappearEffect, transform.position, Quaternion.identity);
        }
        
        Debug.Log($"Plant disappeared after {lifetime} seconds");
        Destroy(gameObject);
    }
    
    // Method để reset timer nếu cần (ví dụ: khi plant được refresh)
    public void ResetTimer()
    {
        timer = lifetime;
    }
    
    // Method để set lifetime tùy chỉnh
    public void SetLifetime(float newLifetime)
    {
        lifetime = newLifetime;
        timer = newLifetime;
    }
}
