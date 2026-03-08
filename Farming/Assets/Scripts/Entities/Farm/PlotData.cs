using System;
using UnityEngine;

public enum SoilStage
{
    Raw,        // Đất thô
    Plowed,     // Đã cuốc
    Watered,    // Đã tưới
    Seeded,     // Đã gieo hạt
    Growing,    // Cây đang phát triển
    Mature      // Cây trưởng thành
}

[Serializable]
public class PlotData
{
    public const int NoCrop = -1;

    public int plotID;
    public SoilStage stage;
    public bool isWatered;
    public bool isFertilized;
    public int CropId;
    public float growthTimer;          // Thời gian cây đã phát triển
    public float requiredGrowthTime;   // Tổng thời gian cần để trưởng thành
    public float matureTimer;          // Thời gian cây trưởng thành (đếm héo)
    public string plantedAt;           // Thời điểm gieo hạt (ISO)
    public string maturedAt;           // Thời điểm trưởng thành (ISO)

    public PlotData() { CropId = NoCrop; }

    public PlotData(int id)
    {
        plotID = id;
        stage = SoilStage.Raw;
        CropId = NoCrop;
        growthTimer = 0f;
        requiredGrowthTime = 0f;
        matureTimer = 0f;
        plantedAt = "";
        maturedAt = "";
    }

    // Gieo hạt
    public void PlantSeed(int cropId, float growthTime)
    {
        CropId = cropId;
        stage = SoilStage.Seeded;
        growthTimer = 0f;
        requiredGrowthTime = growthTime;
        plantedAt = DateTime.UtcNow.ToString("o");
        maturedAt = "";
    }

    // Cập nhật growth timer theo deltaTime
    public void UpdateGrowth(float deltaTime)
    {
        if (stage == SoilStage.Seeded || stage == SoilStage.Growing)
        {
            growthTimer += deltaTime;
            if (growthTimer >= requiredGrowthTime)
            {
                stage = SoilStage.Mature;
                maturedAt = DateTime.UtcNow.ToString("o");
            }
            else
            {
                stage = SoilStage.Growing;
            }
        }
    }


    public void Harvest()
    {
        CropId = NoCrop;
        growthTimer = 0f;
        requiredGrowthTime = 0f;
        matureTimer = 0f;
        isWatered = false;
        isFertilized = false;
        stage = SoilStage.Raw;
        plantedAt = "";
        maturedAt = "";
    }

    public void ResetPlot()
    {
        stage = SoilStage.Raw;
        isWatered = false;
        isFertilized = false;
        CropId = NoCrop;
        growthTimer = 0f;
        requiredGrowthTime = 0f;
        matureTimer = 0f;
        plantedAt = "";
        maturedAt = "";
    }

    public bool HasCrop() => CropId != NoCrop;
}

