using System;
using UnityEngine;

[Serializable]
public class CropData
{
    public int CropId;                   // ID duy nhất cua cay trong
    public string CropName;              // Tên hiển thị
    public int ItemID;                // Item khi thu hoach
    public float TotalGrowthTime;        // Tổng thời gian phát triển
    public int Quantity;              // san luong

    public CropData(int id, string name, int itemId, float totalGrowthTime, int quantity)
    {
        CropId = id;
        CropName = name;
        ItemID = itemId;
        TotalGrowthTime = totalGrowthTime;
        Quantity = quantity;
    }
}
