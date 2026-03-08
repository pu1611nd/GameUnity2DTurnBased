using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 🧺 Quản lý inventory nông trại: Seeds / Fertilizer / FarmProduct / Food
/// Lấy chi tiết từ LoadDataManager thay vì gọi Firebase trực tiếp.
/// </summary>
public class FarmInventoryManager : MonoBehaviour
{
    public static FarmInventoryManager Instance;

    private Dictionary<(FarmItemCategory, int), InventorySlot> inventorySlots = new();

    private Dictionary<int, FarmProduce> produceCache = new();
    private Dictionary<int, Seed> seedCache = new();
    private Dictionary<int, Fertilizer> fertilizerCache = new();
    private Dictionary<int, Food> foodCache = new();

    public Action OnInventoryChanged;
    public Action OnInventoryDataLoaded;

    private User currentUser => UserManager.Instance?.userInGame;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadUserInventory();
    }

    // ============================================================
    // 🔹 Load inventory từ user + lấy chi tiết từ LoadDataManager
    // ============================================================
    public void LoadUserInventory()
    {
        inventorySlots.Clear();

        var itemFarm = currentUser?.Inventory?.ItemFarms;
        if (itemFarm == null)
        {
            Debug.LogWarning("⚠️ Inventory trống hoặc user null!");
            UpdateUI();
            return;
        }

        AddItemsFromList(itemFarm.Seeds, FarmItemCategory.Seeds);
        AddItemsFromList(itemFarm.FarmProduct, FarmItemCategory.FarmProduct);
        AddItemsFromList(itemFarm.Fertilizer, FarmItemCategory.Fertilizer);
        AddItemsFromList(itemFarm.Food, FarmItemCategory.Food);

        Debug.Log($"🧺 Load {inventorySlots.Count} item từ user. Bắt đầu load chi tiết từ LoadDataManager...");
        LoadItemDetailsFromCache();
    }

    private void AddItemsFromList(List<InventorySlot> list, FarmItemCategory category)
    {
        if (list == null) return;
        foreach (var slot in list)
        {
            slot.category = category;
            var key = (category, slot.idItem);
            inventorySlots[key] = slot;
        }
    }

    // ============================================================
    // 🔸 Load chi tiết từng item từ LoadDataManager
    // ============================================================
    private void LoadItemDetailsFromCache()
    {
        if (inventorySlots.Count == 0)
        {
            UpdateUI();
            return;
        }

        foreach (var slot in inventorySlots.Values)
        {
            switch (slot.category)
            {
                case FarmItemCategory.Seeds:
                    var seed = LoadDataManager.Instance.SeedItems.Find(x => x.idItem == slot.idItem);
                    if (seed != null) seedCache[slot.idItem] = seed;
                    break;

                case FarmItemCategory.FarmProduct:
                    var prod = LoadDataManager.Instance.FarmProduceItems.Find(x => x.idItem == slot.idItem);
                    if (prod != null) produceCache[slot.idItem] = prod;
                    break;

                case FarmItemCategory.Fertilizer:
                    var fer = LoadDataManager.Instance.FertilizerItems.Find(x => x.idItem == slot.idItem);
                    if (fer != null) fertilizerCache[slot.idItem] = fer;
                    break;

                case FarmItemCategory.Food:
                    var food = LoadDataManager.Instance.FoodItems.Find(x => x.idItem == slot.idItem);
                    if (food != null) foodCache[slot.idItem] = food;
                    break;
            }
        }

        Debug.Log("✅ Đã load chi tiết item từ LoadDataManager");
        OnInventoryDataLoaded?.Invoke();
        UpdateUI();
    }

    // ============================================================
    // 🔸 Lấy item theo loại + id
    // ============================================================
    public object GetItemByCategoryAndId(FarmItemCategory category, int idItem)
    {
        return category switch
        {
            FarmItemCategory.Seeds => seedCache.TryGetValue(idItem, out var seed) ? seed : null,
            FarmItemCategory.FarmProduct => produceCache.TryGetValue(idItem, out var prod) ? prod : null,
            FarmItemCategory.Fertilizer => fertilizerCache.TryGetValue(idItem, out var fer) ? fer : null,
            FarmItemCategory.Food => foodCache.TryGetValue(idItem, out var food) ? food : null,
            _ => null
        };
    }

    // ============================================================
    // 🔸 Lấy danh sách item theo loại
    // ============================================================
    public List<(object itemData, int quantity)> GetItemsByCategory(FarmItemCategory category)
    {
        var result = new List<(object, int)>();
        foreach (var kvp in inventorySlots)
        {
            if (kvp.Key.Item1 != category) continue;
            var slot = kvp.Value;
            var data = GetItemByCategoryAndId(slot.category, slot.idItem);
            if (data != null) result.Add((data, slot.quantity));
        }
        return result;
    }

    // ============================================================
    // 🔸 Thêm item
    // ============================================================
    public void AddItem(int idItem, FarmItemCategory category, int quantity)
    {
        var key = (category, idItem);
        if (inventorySlots.TryGetValue(key, out var existing))
            existing.quantity += quantity;
        else
            inventorySlots[key] = new InventorySlot(idItem, category, quantity);

        SyncToUserInventory();
        UpdateUI();

        // Nếu item chưa có cache thì load từ LoadDataManager
        if (GetItemByCategoryAndId(category, idItem) == null)
            LoadSingleItemFromCache(idItem, category);
    }

    private void LoadSingleItemFromCache(int idItem, FarmItemCategory category)
    {
        switch (category)
        {
            case FarmItemCategory.Seeds:
                var seed = LoadDataManager.Instance.SeedItems.Find(x => x.idItem == idItem);
                if (seed != null) seedCache[idItem] = seed;
                break;
            case FarmItemCategory.FarmProduct:
                var prod = LoadDataManager.Instance.FarmProduceItems.Find(x => x.idItem == idItem);
                if (prod != null) produceCache[idItem] = prod;
                break;
            case FarmItemCategory.Fertilizer:
                var fer = LoadDataManager.Instance.FertilizerItems.Find(x => x.idItem == idItem);
                if (fer != null) fertilizerCache[idItem] = fer;
                break;
            case FarmItemCategory.Food:
                var food = LoadDataManager.Instance.FoodItems.Find(x => x.idItem == idItem);
                if (food != null) foodCache[idItem] = food;
                break;
        }
    }

    // ============================================================
    // 🔸 Xóa item
    // ============================================================
    public void RemoveItem(int idItem, FarmItemCategory category, int quantity)
    {
        var key = (category, idItem);
        if (!inventorySlots.TryGetValue(key, out var slot)) return;

        slot.quantity -= quantity;
        if (slot.quantity <= 0) inventorySlots.Remove(key);

        SyncToUserInventory();
        UpdateUI();
    }

    // ============================================================
    // 🔸 Đồng bộ inventory về user
    // ============================================================
    private void SyncToUserInventory()
    {
        if (currentUser == null) return;
        if (currentUser.Inventory.ItemFarms == null) currentUser.Inventory.ItemFarms = new ItemFarm();

        currentUser.Inventory.ItemFarms.Seeds.Clear();
        currentUser.Inventory.ItemFarms.FarmProduct.Clear();
        currentUser.Inventory.ItemFarms.Fertilizer.Clear();
        currentUser.Inventory.ItemFarms.Food.Clear();

        foreach (var slot in inventorySlots.Values)
        {
            switch (slot.category)
            {
                case FarmItemCategory.Seeds: currentUser.Inventory.ItemFarms.Seeds.Add(slot); break;
                case FarmItemCategory.FarmProduct: currentUser.Inventory.ItemFarms.FarmProduct.Add(slot); break;
                case FarmItemCategory.Fertilizer: currentUser.Inventory.ItemFarms.Fertilizer.Add(slot); break;
                case FarmItemCategory.Food: currentUser.Inventory.ItemFarms.Food.Add(slot); break;
            }
        }

        UserManager.Instance?.SaveUserData();
    }

    private void UpdateUI() => OnInventoryChanged?.Invoke();
}
