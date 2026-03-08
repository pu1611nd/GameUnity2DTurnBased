using UnityEngine;
using System;
using static CropDatabase;

[RequireComponent(typeof(Collider2D))]
public class FarmPlot : MonoBehaviour
{
    [Header("Data")]
    public PlotData data;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public GameObject actionMenuPrefab;
    private GameObject currentMenu;

    [Header("Soil Sprites")]
    public Sprite rawSprite;
    public Sprite plowedSprite;
    public Sprite seededSprite;
    public Sprite growingSprite;

    [Header("Crop Settings")]
    public SpriteRenderer cropRenderer;
    public CropDatabase cropDatabase;
    private CropData currentCrop;
    private CropItemData cropImages;
    private int currentStageIndex = 0;
    private float growthPerStage;

    private void Start()
    {
        // Load dữ liệu từ PlotData
        LoadFromData(data);
    }

    private void OnMouseDown()
    {
        if (currentMenu != null) { CloseMenu(); return; }

        PlayerMovementPoint.Instance.canClickToMove = false;
        Vector2 targetPos = (Vector2)transform.position + new Vector2(-0.5f, 0f);
        PlayerMovementPoint.Instance.MoveTo(targetPos, () =>
        {
            Vector3 pos = transform.position + new Vector3(0, 1f, 0);
            currentMenu = Instantiate(actionMenuPrefab, pos, Quaternion.identity);
            currentMenu.GetComponent<PlotActionMenu>().ShowAt(this);
        });
    }

    public void CloseMenu()
    {
        if (currentMenu != null)
        {
            PlayerMovementPoint.Instance.canClickToMove = true;
            Destroy(currentMenu);
            currentMenu = null;
        }
    }

    public void UpdateVisual()
    {
        switch (data.stage)
        {
            case SoilStage.Raw: spriteRenderer.sprite = rawSprite; break;
            case SoilStage.Plowed: spriteRenderer.sprite = plowedSprite; break;
            case SoilStage.Seeded: spriteRenderer.sprite = seededSprite; break;
            case SoilStage.Growing: spriteRenderer.sprite = growingSprite; break;
            case SoilStage.Mature: spriteRenderer.sprite = growingSprite; break;
        }
    }

    public void Plow()
    {
        if (data.stage == SoilStage.Raw)
        {
            data.stage = SoilStage.Plowed;
            UpdateVisual();
            CloseMenu();
        }
    }

    public void Water()
    {
        if (data.stage == SoilStage.Seeded || data.stage == SoilStage.Growing)
        {
            data.isWatered = true;
            data.stage = SoilStage.Watered;
            UpdateVisual();
            CloseMenu();
        }
    }

    public void PlantSeed(int cropId)
    {
        currentCrop = FarmManager.Instance.GetCropByID(cropId);
        if (currentCrop == null)
        {
            Debug.LogWarning($"❌ Không tìm thấy cây {cropId}!");
            return;
        }

        cropImages = cropDatabase.GetCrop(cropId);
        if (cropImages == null || cropImages.GrowthSprites.Length < 7)
        {
            Debug.LogWarning($"❌ CropImages của cây {cropId} chưa đủ sprite!");
            return;
        }

        data.PlantSeed(cropId, currentCrop.TotalGrowthTime);
        growthPerStage = currentCrop.TotalGrowthTime / 6f;
        currentStageIndex = 0;
        data.stage = SoilStage.Seeded;

        UpdateVisual();
        UpdateCropVisual();
        CloseMenu();
    }

    private void Update()
    {
        if (data.HasCrop())
        {
            CalculateGrowthTimer();
            UpdateCropVisual();
        }
    }

    private void CalculateGrowthTimer()
    {
        if (string.IsNullOrEmpty(data.plantedAt) || currentCrop == null) return;

        if (DateTime.TryParse(data.plantedAt, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime plantedTime))
        {
            double elapsedSeconds = (DateTime.UtcNow - plantedTime).TotalSeconds;
            data.growthTimer = Mathf.Clamp((float)elapsedSeconds, 0f, data.requiredGrowthTime);

            if (data.growthTimer >= data.requiredGrowthTime)
                data.stage = SoilStage.Mature;
            else if (data.growthTimer > 5f)
                data.stage = SoilStage.Growing;
            else
                data.stage = SoilStage.Seeded;
        }
    }

    private void LoadCropData()
    {
        if (!data.HasCrop()) return;

        currentCrop = FarmManager.Instance.GetCropByID(data.CropId);
        if (currentCrop != null)
        {
            cropImages = cropDatabase.GetCrop(data.CropId);
            if (cropImages != null && cropImages.GrowthSprites.Length >= 7)
                growthPerStage = currentCrop.TotalGrowthTime / 6f;
        }
    }

    private void UpdateCropVisual()
    {
        if (currentCrop == null || cropImages == null || cropImages.GrowthSprites.Length < 7)
        {
            cropRenderer.enabled = false;
            return;
        }

        cropRenderer.enabled = true;

        int newStage = 0;
        float seedStageTime = 5f;

        // Xác định stage hiện tại
        if (data.stage == SoilStage.Seeded)
        {
            newStage = 0;
            spriteRenderer.sprite = seededSprite;
            if (data.growthTimer >= seedStageTime)
                data.stage = SoilStage.Growing;
        }
        else if (data.stage == SoilStage.Growing)
        {
            spriteRenderer.sprite = growingSprite;
            float growthProgress = Mathf.Max(0, data.growthTimer - seedStageTime);
            newStage = Mathf.Clamp(1 + Mathf.FloorToInt(growthProgress / growthPerStage), 1, 5);
        }
        else if (data.stage == SoilStage.Mature)
        {
            newStage = 5;
        }

        // Cập nhật sprite nếu stage thay đổi
        if (newStage != currentStageIndex)
        {
            currentStageIndex = newStage;
            cropRenderer.sprite = cropImages.GrowthSprites[currentStageIndex];
        }

        // 🔹 Căn cây vào trung tâm ô đất
        if (cropRenderer.sprite != null)
        {
            Bounds spriteBounds = cropRenderer.sprite.bounds;

            // Đặt cropRenderer tại trung tâm ô đất, gốc chạm đất
            // Nếu cropRenderer nằm trên cùng GameObject với FarmPlot, localPosition = 0
            cropRenderer.transform.localPosition = new Vector3(
                0f,                      // X trung tâm
                spriteBounds.extents.y,   // Y = nửa chiều cao sprite
                0f                        // Z trung tâm
            );
        }
    }


    public void Harvest()
    {
        if (!data.HasCrop()) return;

        if (FarmInventoryManager.Instance != null && currentCrop != null)
        {
            FarmInventoryManager.Instance.AddItem(currentCrop.ItemID, FarmItemCategory.FarmProduct, currentCrop.Quantity);
            Debug.Log($"🌾 {currentCrop.CropName} đã được thêm vào balo!");
        }

        data.Harvest();
        cropRenderer.sprite = cropImages != null && cropImages.GrowthSprites.Length > 6 ? cropImages.GrowthSprites[6] : null;
        currentCrop = null;

        UpdateVisual();
        CloseMenu();
    }

    public void LoadFromData(PlotData plot)
    {
        data = plot;
        Debug.Log($"Loading plot {data.plotID}, CropId={data.CropId}, plantedAt={data.plantedAt}");

        LoadCropData();

        if (data.HasCrop() && currentCrop != null)
        {
            Debug.Log($"Found crop {currentCrop.CropName}, totalGrowthTime={currentCrop.TotalGrowthTime}");
            CalculateGrowthTimer();
            cropRenderer.enabled = true;
            growthPerStage = currentCrop.TotalGrowthTime / 6f;
        }
        else
        {
            cropRenderer.enabled = false;
            Debug.Log("No crop found for this plot.");
        }

        UpdateVisual();
        UpdateCropVisual();
    }



    public CropData GetCurrentCrop()
    {
        if (currentCrop == null)
            currentCrop = FarmManager.Instance.GetCropByID(data.CropId);
        return currentCrop;
    }
}
