using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cơ sở dữ liệu chứa toàn bộ loại cây trồng trong game.
/// Dùng ScriptableObject để lưu trữ nội bộ (Asset).
/// </summary>
[CreateAssetMenu(fileName = "CropDatabase", menuName = "Farm/Crop Database")]
public class CropDatabase : ScriptableObject
{
    [Tooltip("Danh sách toàn bộ loại cây trồng")]
    public List<CropItemData> crops = new List<CropItemData>();

    /// <summary>
    /// Lấy thông tin cây theo ID.
    /// </summary>
    public CropItemData GetCrop(int cropId)
    {
        return crops.Find(c => c.CropId == cropId);
    }

 
    /// <summary>
    /// Kiểm tra xem cây có tồn tại theo ID không.
    /// </summary>
    public bool HasCrop(int cropId)
    {
        return crops.Exists(c => c.CropId == cropId);
    }

    /// <summary>
    /// Lấy toàn bộ danh sách cây trồng (để load UI shop chẳng hạn)
    /// </summary>
    public List<CropItemData> GetAllCrops()
    {
        return new List<CropItemData>(crops);
    }

    [System.Serializable]
    public class CropItemData
    {
        public int CropId;                   // ID duy nhất cua cay trong
        public string CropName;              // Tên hiển thị
        public Sprite SeedIcon;              // Hình hạt giống (cho inventory / shop)
        public Sprite[] GrowthSprites;       // Sprite từng giai đoạn
        public Sprite RewardIcon;              // hinh thu hoach
    }
}
