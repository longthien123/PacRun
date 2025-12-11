using UnityEngine;

public class ShieldEffect : MonoBehaviour
{
    private ParticleSystem[] particleSystems;
    private Animator animator;
    private Transform player;

    void Start()
    {
        // Lấy player transform
        player = transform.parent;
        
        // Lấy tất cả Particle Systems
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        animator = GetComponent<Animator>();
        
        // Đảm bảo particle systems chạy liên tục
        foreach (var ps in particleSystems)
        {
            if (ps != null)
            {
                var main = ps.main;
                main.loop = true;
                main.playOnAwake = true;
                
                if (!ps.isPlaying)
                    ps.Play();
            }
        }
    }

    void Update()
    {
        // Theo player position
        if (player != null)
        {
            transform.position = player.position;
        }
        
        // Kiểm tra và restart particle nếu dừng
        foreach (var ps in particleSystems)
        {
            if (ps != null && !ps.isPlaying)
            {
                ps.Play();
            }
        }
    }

    void OnDestroy()
    {
        // Dừng tất cả particles khi destroy
        foreach (var ps in particleSystems)
        {
            if (ps != null)
            {
                ps.Stop();
            }
        }
    }
}
