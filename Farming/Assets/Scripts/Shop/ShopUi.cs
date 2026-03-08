using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 🛒 Shop UI — hiển thị tất cả Seed, Fertilizer, Animal trong cùng 1 bảng.
/// </summary>
public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform itemGrid;
    public GameObject itemSlotPrefab;

    [Header("Tooltip")]
    public GameObject tooltip;
    public Text tooltipText;
    public Button btnBuy;

    [Header("Popup Input")]
    public GameObject quantityPanel;
    public InputField quantityInput;
    public Button btnConfirm;
    public Button btnCancel;

    [Header("Confirm Dialog")]
    public ConfirmDialog confirmDialog;

    [Header("Optional")]
    public ItemDatabase itemDatabase; // nếu có database icon nội bộ

    private List<GameObject> slotPool = new List<GameObject>();

    private object selectedItem; // Seed / Fertilizer / Animal
    private int selectedPrice = 0;

    private FarmItemCategory category;

    private void Start()
    {
        if (FarmIShopManager.Instance != null)
        {
            FarmIShopManager.Instance.OnShopLoaded += RefreshUI;
        }

        if (tooltip != null) tooltip.SetActive(false);
        if (quantityPanel != null) quantityPanel.SetActive(false);

        btnConfirm.onClick.AddListener(OnConfirmQuantity);
        btnCancel.onClick.AddListener(() =>
        {
            quantityPanel.SetActive(false);
            tooltip.SetActive(true);
        });
    }

    // ============================================================
    // 🔹 Hiển thị toàn bộ vật phẩm shop
    // ============================================================
    private void RefreshUI()
    {
        if (FarmIShopManager.Instance == null)
            return;

        // Gộp toàn bộ item vào 1 danh sách
        List<(object item, ShopItem priceData)> allItems = new();

        foreach (var seed in FarmIShopManager.Instance.SeedItems)
        {
            var shop = FarmIShopManager.Instance.SeedShopItems.Find(x => x.idItem == seed.idItem);
            allItems.Add((seed, shop));
        }

        foreach (var fert in FarmIShopManager.Instance.FertilizerItems)
        {
            var shop = FarmIShopManager.Instance.FertilizerShopItems.Find(x => x.idItem == fert.idItem);
            allItems.Add((fert, shop));
        }

        foreach (var animalData in FarmIShopManager.Instance.AnimalItems)
        {
            var shop = FarmIShopManager.Instance.AnimalShopItems.Find(x => x.idItem == animalData.idItem);
            allItems.Add((animalData, shop));
        }


        if (allItems.Count == 0)
        {
            Debug.LogWarning("🛒 Shop trống!");
            foreach (var slot in slotPool)
                slot.SetActive(false);
            return;
        }

        // Pooling UI
        for (int i = 0; i < slotPool.Count; i++)
            slotPool[i].SetActive(i < allItems.Count);

        for (int i = 0; i < allItems.Count; i++)
        {
            GameObject slot;
            if (i < slotPool.Count)
            {
                slot = slotPool[i];
            }
            else
            {
                slot = Instantiate(itemSlotPrefab, itemGrid);
                slotPool.Add(slot);
            }

            var (obj, shopData) = allItems[i];
            int price = shopData?.buyPrice ?? 0;

            string name = "";
            int id = 0;

            if (obj is Seed seed)
            {
                name = seed.itemName;
                id = seed.idItem;
                category = FarmItemCategory.Seeds;
            }
            else if (obj is Fertilizer fert)
            {
                name = fert.itemName;
                id = fert.idItem;
                category = FarmItemCategory.Fertilizer;
            }
            else if (obj is ItemAnimal animalData)
            {
                name = animalData.itemName;
                id = animalData.idItem;
                category = FarmItemCategory.Animals;
            }

            Image iconImg = slot.transform.Find("Icon")?.GetComponent<Image>();
         

            // Gán icon nếu có
            if (iconImg != null && itemDatabase != null)
            {
                var data = itemDatabase.GetItemData(id,category);
                if (data != null && data.icon != null)
                    iconImg.sprite = data.icon;
            }

           

            // Click để mở tooltip
            Button btn = slot.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                object capturedItem = obj;
                int capturedPrice = price;
                btn.onClick.AddListener(() => ShowTooltip(capturedItem, capturedPrice, category.ToString()));
            }
        }

        Debug.Log($"✅ Shop hiển thị {allItems.Count} vật phẩm (Seed + Fertilizer + Animal).");
    }

    private void ShowTooltip(object item, int price, string category)
    {
        selectedItem = item;
        selectedPrice = price;

        string name = GetItemName(item);
        string des = GetDescription(item);

        tooltip.SetActive(true);
        tooltipText.text = $"{name}\nGiá: {price} vàng\n: {des}";
        btnBuy.gameObject.SetActive(true);

        btnBuy.onClick.RemoveAllListeners();
        btnBuy.onClick.AddListener(OpenQuantityPopup);
    }

    private void OpenQuantityPopup()
    {
        if (selectedItem == null) return;

        tooltip.SetActive(false);
        quantityPanel.SetActive(true);
        quantityInput.text = "1";
    }

    private void OnConfirmQuantity()
    {
        if (selectedItem == null) return;

        if (!int.TryParse(quantityInput.text, out int qty))
        {
            confirmDialog.ShowAlert("⚠️ Số lượng nhập không hợp lệ!");
            return;
        }

        if (qty <= 0)
        {
            confirmDialog.ShowAlert("⚠️ Số lượng phải > 0!");
            return;
        }

        int totalPrice = qty * selectedPrice;
        confirmDialog.Show(
            $"Bạn có chắc muốn mua {qty} x {GetItemName(selectedItem)} với giá {totalPrice} vàng không?",
            () => ExecuteBuy(qty)
        );
    }

    private void ExecuteBuy(int qty)
    {
        if (selectedItem == null) return;
        FarmIShopManager.Instance.BuyItem(selectedItem, qty);
        tooltip.SetActive(false);
        quantityPanel.SetActive(false);
    }

    private string GetItemName(object item)
    {
        if (item is Seed s) return s.itemName;
        if (item is Fertilizer f) return f.itemName;
        if (item is ItemAnimal a) return a.itemName;
        return "Vật phẩm";
    }

    private string GetDescription(object item)
    {
        if (item is Seed s) return s.description;
        if (item is Fertilizer f) return f.description;
        if (item is ItemAnimal a) return a.description;
        return "Vật phẩm";
    }
}
