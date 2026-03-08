using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FarmInventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform itemGrid;
    public GameObject itemSlotPrefab;
    public Button btnFarmProduce;
    public Button btnSeed;
    public Button btnFertilizer;
    public Button btnFood;

    [Header("Tooltip")]
    public GameObject tooltip;
    public TMP_Text tooltipText;
    public Button btnSell;
    public Button btnDiscard;

    [Header("Popup Input")]
    public GameObject quantityPanel;
    public TMP_InputField quantityInput;
    public Button btnConfirm;
    public Button btnCancel;

    [Header("Confirm Dialog")]
    public ConfirmDialog confirmDialog;

    [Header("Databases")]
    public ItemDatabase itemDatabase; // chứa icon

    private FarmItemCategory currentCategory;
    private readonly List<GameObject> slotPool = new();

    // dữ liệu item đang chọn
    private object selectedItemData;
    private int selectedItemQuantity;
    private FarmItemCategory selectedCategory;

    private enum ActionType { None, Sell, Discard }
    private ActionType currentAction;

    private void Start()
    {
        if (FarmInventoryManager.Instance != null)
        {
            FarmInventoryManager.Instance.OnInventoryChanged += RefreshUI;
            FarmInventoryManager.Instance.OnInventoryDataLoaded += RefreshUI;
        }

        btnFarmProduce.onClick.AddListener(() => ShowCategory(FarmItemCategory.FarmProduct));
        btnSeed.onClick.AddListener(() => ShowCategory(FarmItemCategory.Seeds));
        btnFertilizer.onClick.AddListener(() => ShowCategory(FarmItemCategory.Fertilizer));
        btnFood.onClick.AddListener(() => ShowCategory(FarmItemCategory.Food));

        ShowCategory(FarmItemCategory.FarmProduct);

        tooltip?.SetActive(false);
        quantityPanel?.SetActive(false);

        btnConfirm.onClick.AddListener(OnConfirmQuantity);
        btnCancel.onClick.AddListener(() =>
        {
            quantityPanel.SetActive(false);
            tooltip.SetActive(true);
        });

        btnSell.onClick.AddListener(() => OpenQuantityPopup(ActionType.Sell));
        btnDiscard.onClick.AddListener(() => OpenQuantityPopup(ActionType.Discard));
    }

    private void ShowCategory(FarmItemCategory category)
    {
        currentCategory = category;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (FarmInventoryManager.Instance == null || itemGrid == null) return;

        var items = FarmInventoryManager.Instance.GetItemsByCategory(currentCategory);

        // ẩn các slot thừa
        for (int i = 0; i < slotPool.Count; i++)
            slotPool[i].SetActive(i < items.Count);

        // hiển thị item
        for (int i = 0; i < items.Count; i++)
        {
            GameObject slot;
            if (i < slotPool.Count)
                slot = slotPool[i];
            else
            {
                slot = Instantiate(itemSlotPrefab, itemGrid);
                slotPool.Add(slot);
            }

            (object itemDataObj, int quantity) = items[i];
            if (itemDataObj == null)
            {
                Debug.LogWarning("⚠️ itemData null khi hiển thị inventory!");
                continue;
            }

            // ép kiểu sang Item để lấy dữ liệu chung
            Item itemData = itemDataObj as Item;
            if (itemData == null) continue;

            Image iconImg = slot.transform.Find("Icon")?.GetComponent<Image>();
            

            // Gán icon từ itemDatabase
            if (iconImg != null && itemDatabase != null)
            {
                var dbItem = itemDatabase.GetItemData(itemData.idItem, currentCategory);
                if (dbItem != null && dbItem.icon != null)
                    iconImg.sprite = dbItem.icon;
                else
                    iconImg.sprite = null;
            }

         

            Button btn = slot.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => ShowTooltip(itemData, quantity, currentCategory));
        }
    }

    private void ShowTooltip(Item itemData, int quantity, FarmItemCategory category)
    {
        selectedItemData = itemData;
        selectedItemQuantity = quantity;
        selectedCategory = category;
        currentAction = ActionType.None;

        tooltip.SetActive(true);
        tooltipText.text = $"{itemData.itemName}\nSố lượng: {quantity}\n: {itemData.description}";
        btnSell.gameObject.SetActive(true);
        btnDiscard.gameObject.SetActive(true);
    }

    private void OpenQuantityPopup(ActionType action)
    {
        currentAction = action;
        tooltip.SetActive(false);
        quantityPanel.SetActive(true);
        quantityInput.text = "1";
    }

    private void OnConfirmQuantity()
    {
        if (selectedItemData == null) return;

        if (!int.TryParse(quantityInput.text, out int qty))
        {
            confirmDialog.ShowAlert("⚠️ Số lượng nhập không hợp lệ!");
            return;
        }

        if (qty <= 0 || qty > selectedItemQuantity)
        {
            confirmDialog.ShowAlert("⚠️ Số lượng phải > 0 và ≤ số lượng đang có!");
            return;
        }

        string actionText = currentAction == ActionType.Sell ? "bán" : "bỏ";
        confirmDialog.Show(
            $"Bạn có chắc muốn {actionText} {qty} x {(selectedItemData as Item).itemName} không?",
            () => ExecuteAction(qty)
        );
    }

    private void ExecuteAction(int qty)
    {
        if (selectedItemData == null) return;

        Item item = selectedItemData as Item;
        if (item == null) return;

        // ⚙️ Gọi đúng RemoveItem theo id và category (theo bản Manager mới)
        FarmInventoryManager.Instance.RemoveItem(item.idItem, selectedCategory, qty);

        if (currentAction == ActionType.Sell)
            Debug.Log($"💰 Đã bán {qty} x {item.itemName}");
        else
            Debug.Log($"🗑️ Đã bỏ {qty} x {item.itemName}");

        quantityPanel.SetActive(false);
        tooltip.SetActive(false);
        RefreshUI();
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
        btnSell.gameObject.SetActive(false);
        btnDiscard.gameObject.SetActive(false);
    }
}
