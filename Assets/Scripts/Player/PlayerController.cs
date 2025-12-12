using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private Rigidbody2D rb;
    private Vector2 playerDirection;
    private Animator animator;

    private CharacterBase currentForm;
    private System.Type currentFormType;   // Chống spam

    [SerializeField] private GameObject smokeEffect;
    [SerializeField] private GameObject smokeKnight;
    [SerializeField] public GameObject effectCollectKey;

    [SerializeField] public GameObject shurikenPrefab;
    [SerializeField] public GameObject swordPrefab;
    [SerializeField] public GameObject shieldFx;
    [SerializeField] public GameObject upgradeFx;
    [SerializeField] public GameObject redSkullFx;
    [SerializeField] public GameObject blueSkullFx;
    

    private Vector2 lastDir = Vector2.down; // mặc định nhìn xuống

    // Cooldown cho shuriken
    private float shurikenCooldown = 0.5f; // 1 giây = 1 phi tiêu mỗi giây
    private float lastShurikenTime = -999f;

    [Header("Material")]
    public Material defaultMaterial;
    public Material flashMaterial;

    [Header("Transform System")]
    public bool hasNinjaItem = false;
    public bool hasKnightItem = false;
    public float ninjaTimeLeft = 0f;
    public float knightTimeLeft = 0f;
    private const float MAX_TRANSFORM_TIME = 20f;
    private bool isTransformActive = false;
    [Header("Shield System")]
    public bool isShieldActive = false;
    public float shieldTimeLeft = 0f;
    private const float MAX_SHIELD_TIME = 10f;
    private GameObject activeShieldFx;

    [Header("Upgrade System")]
    public bool isUpgradeActive = false;
    public float upgradeTimeLeft = 0f;
    private const float MAX_UPGRADE_TIME = 10f;
    private GameObject activeUpgradeFx;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        defaultMaterial = GetComponent<SpriteRenderer>().material;
        ChangeForm(typeof(NormalCharacter)); // Form mặc định
        UIController.Instance.UpdateKeyCount(ManagePlayer.Instance.keysCollected);
        UIController.Instance.UpdateHealthBar(PlayerHealthManager.CurrentHealth, PlayerHealthManager.MaxHealth);
        UIController.Instance.UpdateNinjaTimer(ninjaTimeLeft, MAX_TRANSFORM_TIME);
        UIController.Instance.UpdateKnightTimer(knightTimeLeft, MAX_TRANSFORM_TIME);
        UIController.Instance.UpdateShieldTimer(shieldTimeLeft, MAX_SHIELD_TIME);
        UIController.Instance.UpdateUpgradeTimer(upgradeTimeLeft, MAX_UPGRADE_TIME);
    }

    void Update()
    {
        float directionX = Input.GetAxisRaw("Horizontal");
        float directionY = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(directionX, directionY);
        
        // Cập nhật hướng cuối cùng khi có input
        if (input != Vector2.zero)
            lastDir = input.normalized;
            
        playerDirection = input.normalized;

        animator.SetFloat("moveX", directionX);
        animator.SetFloat("moveY", directionY);

        // ---- ATTACK ----
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }

        // ---- NHẤN PHÍM ----
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (hasNinjaItem && ninjaTimeLeft > 0)
                ChangeForm(typeof(NinjaCharacter));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (hasKnightItem && knightTimeLeft > 0)
                ChangeForm(typeof(KnightCharacter));
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeForm(typeof(NormalCharacter));

        // Đếm thời gian cho dạng đặc biệt
        if (currentForm is NinjaCharacter && ninjaTimeLeft > 0)
        {
            ninjaTimeLeft -= Time.deltaTime;
            if (ninjaTimeLeft <= 0)
            {
                ninjaTimeLeft = 0;
                ChangeForm(typeof(NormalCharacter));            }
        }
        else if (currentForm is KnightCharacter && knightTimeLeft > 0)
        {
            knightTimeLeft -= Time.deltaTime;
            if (knightTimeLeft <= 0)
            {
                knightTimeLeft = 0;
                ChangeForm(typeof(NormalCharacter));
            }
        }
        
        // Đếm thời gian cho khiên
        if (isShieldActive && shieldTimeLeft > 0)
        {
            shieldTimeLeft -= Time.deltaTime;
            if (shieldTimeLeft <= 0)
            {
                shieldTimeLeft = 0;
                isShieldActive = false;
                if (activeShieldFx != null)
                {
                    Destroy(activeShieldFx);
                }
            }
        }
        if (isUpgradeActive && upgradeTimeLeft > 0)
        {
            upgradeTimeLeft -= Time.deltaTime;
            if (upgradeTimeLeft <= 0)
            {
                upgradeTimeLeft = 0;
                isUpgradeActive = false;
                if (activeUpgradeFx != null)
                {
                    Destroy(activeUpgradeFx);
                }
            }
        }
        
        // Cập nhật UI (chỉ trong Update, không phải FixedUpdate)
        UIController.Instance.UpdateKeyCount(ManagePlayer.Instance.keysCollected);
        UIController.Instance.UpdateHealthBar(PlayerHealthManager.CurrentHealth, PlayerHealthManager.MaxHealth);
        UIController.Instance.UpdateNinjaTimer(ninjaTimeLeft, MAX_TRANSFORM_TIME);
        UIController.Instance.UpdateKnightTimer(knightTimeLeft, MAX_TRANSFORM_TIME);
        UIController.Instance.UpdateShieldTimer(shieldTimeLeft, MAX_SHIELD_TIME);
        UIController.Instance.UpdateUpgradeTimer(upgradeTimeLeft, MAX_UPGRADE_TIME);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = playerDirection * currentForm.moveSpeed;
    }

    // ATTACK SYSTEM
    void Attack()
    {
        // Chỉ cho phép Ninja và Knight attack
        if (currentForm is NormalCharacter)
            return;

        // Set hướng attack dựa trên lastDir
        animator.SetFloat("attackX", lastDir.x);
        animator.SetFloat("attackY", lastDir.y);

        // Trigger attack animation
        animator.SetBool("attack", true);

        // Nếu là Ninja thì ném shuriken (kiểm tra cooldown)
        if (currentForm is NinjaCharacter)
        {
            // Kiểm tra cooldown
            if (Time.time >= lastShurikenTime + shurikenCooldown)
            {
                NinjaCharacter ninja = (NinjaCharacter)currentForm;
                ninja.ThrowShuriken(lastDir);
                lastShurikenTime = Time.time;
            }
            else
            {
                Debug.Log("Shuriken on cooldown!");
            }
        }
        // Nếu là Knight thì chém kiếm
        else if (currentForm is KnightCharacter)
        {
            if (Time.time >= lastShurikenTime + shurikenCooldown)
            {
            KnightCharacter knight = (KnightCharacter)currentForm;
            knight.SlashSword(lastDir);
            lastShurikenTime = Time.time;
            }
            else
            {
                Debug.Log("Sword attack on cooldown!");
            }
        }

        // Reset attack sau khi animation xong (điều chỉnh thời gian phù hợp)
        Invoke("ResetAttack", 0.5f);
    }

    void ResetAttack()
    {
        animator.SetBool("attack", false);
    }
    // CHANGE FORM (ĐÃ FIX CHỐNG SPAM + SMOKE THEO PLAYER)
    void ChangeForm(System.Type formType)
    {
        if (currentFormType == formType)
            return;

        currentFormType = formType;
        SpawnFormSmoke(formType);

        if (currentForm != null)
            Destroy(currentForm);

        currentForm = (CharacterBase)gameObject.AddComponent(formType);

        // Gán material cho form mới
        currentForm.defaultMaterial = defaultMaterial;
        currentForm.flashMaterial = flashMaterial;

        ApplyFormAnimation();
    }
    // SMOKE TỰ THEO PLAYER
    void SpawnFormSmoke(System.Type formType)
    {
        GameObject prefab = null;
        if (formType == typeof(NinjaCharacter))
            prefab = smokeEffect;
        else if (formType == typeof(KnightCharacter))
            prefab = smokeKnight;
        else
            prefab = smokeEffect;  // hiệu ứng chung cho Boy
        GameObject smoke = Instantiate(prefab, transform.position, Quaternion.identity);
        // Để smoke theo player
        smoke.transform.SetParent(transform);
        Destroy(smoke, 1f);
    }
    // ANIMATIONS
    void ApplyFormAnimation()
    {
        RuntimeAnimatorController controller = null;

        if (currentForm is NinjaCharacter)
            controller = Resources.Load<RuntimeAnimatorController>("Animations/Player/Ninja/Ninja");

        else if (currentForm is KnightCharacter)
            controller = Resources.Load<RuntimeAnimatorController>("Animations/Player/Knight/Knight");

        else if (currentForm is NormalCharacter)
            controller = Resources.Load<RuntimeAnimatorController>("Animations/Player/Boy/Player");

        if (controller == null)
        {
            Debug.LogError("Animator Controller NOT FOUND in Resources!");
            return;
        }

        animator.runtimeAnimatorController = controller;
    }
    // TRANSFORM ITEM SYSTEM (THÊM MỚI)
    public void CollectNinjaItem()
    {
        hasNinjaItem = true;
        ninjaTimeLeft = MAX_TRANSFORM_TIME;
    }

    public void CollectKnightItem()
    {
        hasKnightItem = true;
        knightTimeLeft = MAX_TRANSFORM_TIME;
    }
    public void CollectShieldItem()
    {
        isShieldActive = true;
        shieldTimeLeft = MAX_SHIELD_TIME;
        
        // Hủy khiên cũ nếu có
        if (activeShieldFx != null)
        {
            Destroy(activeShieldFx);
        }
        
        // Tạo hiệu ứng khiên mới
        activeShieldFx = Instantiate(shieldFx, transform.position, Quaternion.identity, transform);
        
        // Thêm ShieldEffect component nếu chưa có
        if (activeShieldFx.GetComponent<ShieldEffect>() == null)
        {
            activeShieldFx.AddComponent<ShieldEffect>();
        } 
    }
    public void CollectUpgradeItem()
    {
        isUpgradeActive = true;
        upgradeTimeLeft = MAX_UPGRADE_TIME;
        
        // Hủy hiệu ứng cũ nếu có
        if (activeUpgradeFx != null)
        {
            Destroy(activeUpgradeFx);
        }
        
        // Tạo hiệu ứng nâng cấp mới
        activeUpgradeFx = Instantiate(upgradeFx, transform.position, Quaternion.identity, transform);
        
        // Thêm ShieldEffect component nếu chưa có
        if (activeUpgradeFx.GetComponent<ShieldEffect>() == null)
        {
            activeUpgradeFx.AddComponent<ShieldEffect>();
        }  
    }
    public float GetNinjaTimeLeft() => ninjaTimeLeft;
    public float GetKnightTimeLeft() => knightTimeLeft;
}
