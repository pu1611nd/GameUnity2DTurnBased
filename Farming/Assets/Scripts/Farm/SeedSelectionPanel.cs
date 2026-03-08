using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedSelectPanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject seedButtonPrefab;     // Prefab nút hiển thị hạt giống
    public Transform seedListContainer;     // Nơi chứa danh sách nút
    public ItemDatabase itemDatabase;       // Để lấy icon (nếu có)

    private FarmPlot currentPlot;           // Ô đất hiện tại

    private void OnEnable()
    {
        PopulateSeeds();
    }

    /// <summary>
    /// Hiển thị panel chọn hạt giống cho ô đất cụ thể
    /// </summary>
    public void Show(FarmPlot plot)
    {
        currentPlot = plot;
        gameObject.SetActive(true);
        PopulateSeeds();
    }

    /// <summary>
    /// Lấy danh sách hạt giống từ túi đồ (category == FarmItemCategory.Seed)
    /// </summary>
    private void PopulateSeeds()
    {
        // Xóa danh sách cũ
        foreach (Transform child in seedListContainer)
            Destroy(child.gameObject);

        var seeds = FarmInventoryManager.Instance.GetItemsByCategory(FarmItemCategory.Seeds);
        if (seeds == null || seeds.Count == 0)
        {
            Debug.Log("🌾 Không có hạt giống trong balo!");
            return;
        }

        foreach (var (itemDataObj, quantity) in seeds)
        {
            // ép kiểu để dùng thuộc tính FarmItem
            Seed itemData = itemDataObj as Seed;
            if (itemData == null) continue;

            GameObject btnObj = Instantiate(seedButtonPrefab, seedListContainer);

            // Cập nhật icon và text
            Text quantityText = btnObj.transform.Find("Quantity")?.GetComponent<Text>();
            Image icon = btnObj.transform.Find("Icon")?.GetComponent<Image>();
            Text nameText = btnObj.transform.Find("Name")?.GetComponent<Text>();

            if (quantityText != null)
                quantityText.text = $"{quantity}";

            if (nameText != null)
                nameText.text = itemData.itemName;

            if (icon != null && itemDatabase != null)
            {
                var dbItem = itemDatabase.GetItemData(itemData.idItem,FarmItemCategory.Seeds);
                if (dbItem != null && dbItem.icon != null)
                    icon.sprite = dbItem.icon;
            }

            // Gán sự kiện trồng cây
            btnObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (currentPlot != null)
                {
                    // ⚙️ Trồng cây theo ID cây trong hạt giống
                    currentPlot.PlantSeed(itemData.CropID);

                    // ⚙️ Trừ hạt giống trong túi
                    FarmInventoryManager.Instance.RemoveItem(itemData.idItem, FarmItemCategory.Seeds, 1);

                    // Ẩn panel chọn hạt giống
                    gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("⚠️ currentPlot chưa được gán!");
                }
            });
        }
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
