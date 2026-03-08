using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Farm/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [Tooltip("Danh sách toàn bộ item có trong game (có icon Unity)")]
    public List<FarmItemData> items = new List<FarmItemData>();

    public FarmItemData GetItemData(int idItem, FarmItemCategory category)
    {
        var item = items.Find(i => i.idItem == idItem && i.category == category);
        if (item == null)
            Debug.LogWarning($"⚠️ Không tìm thấy item id={idItem}, category={category} trong ItemDatabase!");
        return item;
    }


    // Kiểm tra tồn tại
    public bool HasItem(int idItem)
    {
        return items.Exists(i => i.idItem == idItem);
    }
}

[System.Serializable]
public class FarmItemData
{
    public int idItem;
    public string itemName;
    public Sprite icon;                 // Icon UI, chỉ dùng trong Unity
    public FarmItemCategory category;
}
