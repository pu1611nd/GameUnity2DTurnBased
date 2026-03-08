using UnityEngine;

/// <summary>
/// Base class cho mọi vật nuôi — xử lý animation, di chuyển, stage và bán.
/// Dữ liệu đồng bộ 2 chiều với class Animal (lưu Firebase).
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public abstract class AnimalBase : MonoBehaviour
{
    [Header("🎨 Sprites cho từng giai đoạn (4 frame mỗi giai đoạn)")]
    public Sprite[] babySprites;
    public Sprite[] teenSprites;
    public Sprite[] adultSprites;

    [Header("🎞️ Animation")]
    public float animSpeed = 4f;

    [Header("🚶 Di chuyển")]
    public Collider2D areaCollider;
    public float moveSpeed = 1f;
    public Vector2 idleTimeRange = new Vector2(2f, 5f);

    [Header("🧠 Dữ liệu con vật")]
    public Animal Data;                  // instance animal
    public AnimalData TemplateData;      // template dữ liệu chung

    protected SpriteRenderer sr;
    protected float animTimer;
    protected int animFrame;
    protected GrowthStage currentStage = GrowthStage.Baby;

    private Vector2 moveTarget;
    private bool isIdle;
    private float idleTimer;
    private float idleDuration;

    // =====================================================================
    // 🏁 UNITY LIFECYCLE
    // =====================================================================
    protected virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (Data == null)
        {
            Debug.LogWarning($"{name}: chưa gán dữ liệu Animal, tạo tạm...");
            Data = new Animal(0);
        }

        if (TemplateData == null)
        {
            Debug.LogWarning($"{name}: chưa có AnimalData tương ứng cho ID {Data.AnimalDataId}");
        }

        UpdateStage();
        ChooseNewTarget();
    }

    protected virtual void Update()
    {
        if (Data == null || TemplateData == null) return;

        animTimer += Time.deltaTime;

        // 🔄 Cập nhật stage dựa theo thời gian
        GrowthStage prevStage = currentStage;
        Data.UpdateGrowthStage(TemplateData);
        currentStage = Data.GrowthStage;
        if (currentStage != prevStage)
        {
            animFrame = 0;
            UpdateSpriteFrame();
        }

        // 🎞️ Animation
        if (animTimer >= 1f / animSpeed)
        {
            animTimer = 0f;
            animFrame = (animFrame + 1) % 4;
            UpdateSpriteFrame();
        }

        // 🚶 Di chuyển & nghỉ
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                isIdle = false;
                ChooseNewTarget();
            }
        }
        else
        {
            MoveAround();
        }
    }

    // =====================================================================
    // 🖼️ ANIMATION & STAGE
    // =====================================================================
    protected void UpdateStage()
    {
        if (Data == null || TemplateData == null)
        {
            currentStage = GrowthStage.Baby;
            return;
        }

        Data.UpdateGrowthStage(TemplateData);
        currentStage = Data.GrowthStage;
        UpdateSpriteFrame();
    }

    protected void UpdateSpriteFrame()
    {
        Sprite[] array = currentStage switch
        {
            GrowthStage.Baby => babySprites,
            GrowthStage.Teen => teenSprites,
            GrowthStage.Adult => adultSprites,
            _ => null
        };

        if (array?.Length > 0)
            sr.sprite = array[animFrame % array.Length];
    }

    // =====================================================================
    // 🚶 DI CHUYỂN
    // =====================================================================
    protected void MoveAround()
    {
        if (areaCollider == null) return;

        Vector2 pos = transform.position;
        Vector2 dir = (moveTarget - pos).normalized;

        // Lật sprite theo hướng
        sr.flipX = dir.x < -0.05f;

        transform.Translate(dir * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(pos, moveTarget) < 0.1f)
        {
            isIdle = true;
            idleTimer = 0f;
            idleDuration = Random.Range(idleTimeRange.x, idleTimeRange.y);
        }
    }

    protected void ChooseNewTarget()
    {
        if (areaCollider == null) return;

        Bounds b = areaCollider.bounds;
        int safety = 0;

        do
        {
            moveTarget = new Vector2(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y)
            );
            safety++;
        } while (!areaCollider.OverlapPoint(moveTarget) && safety < 30);
    }

    // =====================================================================
    // 💰 BÁN VẬT NUÔI
    // =====================================================================
    public bool TrySell(out int price)
    {
        price = 0;
        if (Data == null || TemplateData == null)
        {
            Debug.LogWarning($"{name}: chưa gán đủ dữ liệu Animal hoặc AnimalData!");
            return false;
        }

        if (!Data.CanBeSold(TemplateData))
        {
            Debug.Log($"{TemplateData.type} chưa thể bán (chưa trưởng thành hoặc đã hết tuổi thọ)!");
            return false;
        }

        price = TemplateData.sellPrice;
        Debug.Log($"💰 Bán {TemplateData.type} (ID: {Data.IdAnimal}) giá {price} vàng!");
        Destroy(gameObject);
        return true;
    }

    // =====================================================================
    // 🍽️ HÀNH VI KHI CHO ĂN (có thể override)
    // =====================================================================
    public virtual void OnFed()
    {
        Data?.Feed();
    }
}
