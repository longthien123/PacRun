using UnityEngine;

public class ChestAnimatorController : MonoBehaviour
{
    // Biến tham chiếu tới component Animator
    private Animator animator; 
    public bool isOpened = false;

    void Awake()
    {
        // Lấy component Animator từ chính GameObject này
        animator = GetComponent<Animator>(); 
    }

    // Hàm public để class khác gọi mở rương
    public void OpenChest()
    {
        // Kiểm tra an toàn
        if (animator != null)
        {
            // Thiết lập tham số "Open" thành true để kích hoạt animation mở
            animator.SetBool("open", true); 
            isOpened = true;
            // (Tùy chọn) Thêm logic spawn vật phẩm tại đây
            Debug.Log("Rương đã mở!");
            
            // Tắt script này hoặc Collider nếu không muốn mở lại
            // GetComponent<Collider2D>().enabled = false;
        }
    }
}