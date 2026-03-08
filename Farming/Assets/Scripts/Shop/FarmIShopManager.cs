using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 🛒 Quản lý shop nông trại: Seed / Fertilizer / Animal
/// Lấy dữ liệu từ LoadDataManager.
/// </summary>
public class FarmIShopManager : MonoBehaviour
{
    public static FarmIShopManager Instance;

    [Header("Danh sách item trong shop (chi tiết)")]
    public List<Seed> SeedItems = new();
    public List<Fertilizer> FertilizerItems = new();
    public List<ItemAnimal> AnimalItems = new();

    [Header("Thông tin giá bán (ShopItem)")]
    public List<ShopItem> SeedShopItems = new();
    public List<ShopItem> FertilizerShopItems = new();
    public List<ShopItem> AnimalShopItems = new();

    public System.Action OnShopLoaded;

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
            return;
        }
    }

    private void Start()
    {
        // Lấy dữ liệu từ LoadDataManager
        LoadDataFromManager();
    }

    /// <summary>
    /// Lấy toàn bộ dữ liệu shop từ LoadDataManager
    /// </summary>
    public void LoadDataFromManager()
    {
        if (LoadDataManager.Instance == null)
        {
            Debug.LogError("❌ LoadDataManager chưa được khởi tạo!");
            return;
        }

        // Copy dữ liệu item chi tiết
        SeedItems = new List<Seed>(LoadDataManager.Instance.SeedItems);
        FertilizerItems = new List<Fertilizer>(LoadDataManager.Instance.FertilizerItems);
        AnimalItems = new List<ItemAnimal>(LoadDataManager.Instance.AnimalItems);

        // Copy dữ liệu shop (giá bán)
        SeedShopItems = new List<ShopItem>(LoadDataManager.Instance.SeedShopItems);
        FertilizerShopItems = new List<ShopItem>(LoadDataManager.Instance.FertilizerShopItems);
        AnimalShopItems = new List<ShopItem>(LoadDataManager.Instance.AnimalShopItems);

        Debug.Log("✅ FarmIShopManager: Đã lấy dữ liệu từ LoadDataManager");
        OnShopLoaded?.Invoke();
    }

    // ============================================================
    // 🔸 Lấy item theo loại + ID
    // ============================================================
    public T GetItemById<T>(int id) where T : class
    {
        if (typeof(T) == typeof(Seed))
            return SeedItems.Find(x => (x as Seed).idItem == id) as T;
        if (typeof(T) == typeof(Fertilizer))
            return FertilizerItems.Find(x => (x as Fertilizer).idItem == id) as T;
        if (typeof(T) == typeof(ItemAnimal))
            return AnimalItems.Find(x => (x as ItemAnimal).idItem == id) as T;
        return null;
    }

    // ============================================================
    // 🔸 Mua item — tùy loại
    // ============================================================
    public void BuyItem(object itemObj, int quantity)
    {
        if (itemObj == null || UserManager.Instance?.userInGame == null)
        {
            Debug.LogWarning("⚠️ Không thể mua item!");
            return;
        }

        var user = UserManager.Instance.userInGame;
        int totalPrice = 0;
        int buyPrice = 0;
        ShopItem shopInfo = null;

        if (itemObj is Seed seed)
        {
            shopInfo = SeedShopItems.Find(i => i.idItem == seed.idItem);
            if (shopInfo == null) { Debug.LogWarning($"❌ Không tìm thấy giá của {seed.itemName}"); return; }
            buyPrice = shopInfo.buyPrice;
            totalPrice = buyPrice * quantity;

            if (user.Gold < totalPrice) { Debug.LogWarning("💸 Không đủ tiền!"); return; }
            user.Gold -= totalPrice;
            FarmInventoryManager.Instance.AddItem(seed.idItem, FarmItemCategory.Seeds, quantity);
            Debug.Log($"🛍️ Đã mua {quantity} x {seed.itemName}");
        }
        else if (itemObj is Fertilizer fertilizer)
        {
            shopInfo = FertilizerShopItems.Find(i => i.idItem == fertilizer.idItem);
            if (shopInfo == null) { Debug.LogWarning($"❌ Không tìm thấy giá của {fertilizer.itemName}"); return; }
            buyPrice = shopInfo.buyPrice;
            totalPrice = buyPrice * quantity;

            if (user.Gold < totalPrice) { Debug.LogWarning("💸 Không đủ tiền!"); return; }
            user.Gold -= totalPrice;
            FarmInventoryManager.Instance.AddItem(fertilizer.idItem, FarmItemCategory.Fertilizer, quantity);
            Debug.Log($"🛍️ Đã mua {quantity} x {fertilizer.itemName}");
        }
        else if (itemObj is ItemAnimal animal)
        {
            shopInfo = AnimalShopItems.Find(i => i.idItem == animal.idItem);
            if (shopInfo == null) { Debug.LogWarning($"❌ Không tìm thấy giá của {animal.itemName}"); return; }
            buyPrice = shopInfo.buyPrice;
            totalPrice = buyPrice * quantity;

            if (user.Gold < totalPrice) { Debug.LogWarning("💸 Không đủ tiền!"); return; }

            AnimalData template = FarmAnimalLoader.Instance?.GetAnimalData(animal.IdAnimal);
            var barn = user.UserFarm?.GetBarnByType(template.type);
            if (barn == null) { Debug.LogError($"❌ Không tìm thấy chuồng cho {template.type}"); return; }

            user.Gold -= totalPrice;
            int added = 0;
            for (int i = 0; i < quantity; i++)
            {
                Animal newAnimal = new Animal(template.idAnimalData);
                if (barn.AddAnimal(newAnimal, template))
                {
                    AnimalManager.Instance.SpawnAnimal(newAnimal);
                    added++;
                }
                else
                {
                    Debug.LogWarning("⚠️ Chuồng đầy!");
                    break;
                }
            }

            Debug.Log($"🐾 Đã mua {added}/{quantity} {template.type}");
        }

        // 💾 Lưu dữ liệu người chơi
        UserManager.Instance.SaveUserData();
    }
}
