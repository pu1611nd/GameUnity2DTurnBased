using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;

public class LoadDataManager : MonoBehaviour
{
    public static LoadDataManager Instance { get; private set; }
    public static FirebaseUser firebaseUser;

    private FirebaseDataBaseManager firebaseData;

    private bool userLoaded;
    private bool itemsLoaded;
    private bool animalLoaded;
    private bool cropLoaded;

    public event Action<float> OnProgressChanged;

    [Header("Tên Scene Game sau khi load xong")]
    public string gameSceneName = "PlayScene";

    [Header("Danh sách item (chi tiết)")]
    public List<Seed> SeedItems = new();
    public List<Fertilizer> FertilizerItems = new();
    public List<ItemAnimal> AnimalItems = new();
    public List<FarmProduce> FarmProduceItems = new();
    public List<Food> FoodItems = new();

    [Header("Thông tin giá bán (ShopItem)")]
    public List<ShopItem> SeedShopItems = new();
    public List<ShopItem> FertilizerShopItems = new();
    public List<ShopItem> AnimalShopItems = new();

    [Header("Danh sách Crop")]
    public List<CropData> Crops = new();

    [Header("Danh sách AnimalData")]
    public List<AnimalData> AnimalDatas = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        firebaseData = FindObjectOfType<FirebaseDataBaseManager>();
        if (firebaseData == null)
        {
            Debug.LogError("❌ Không tìm thấy FirebaseDataBaseManager!");
            return;
        }

        firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
        if (firebaseUser == null)
        {
            Debug.LogError("❌ Không có người dùng đăng nhập!");
            return;
        }

        LoadAllData();
    }

    private void LoadAllData()
    {
        LoadUserData(() =>
        {
            LoadItems(() =>
            {
                LoadAnimalData(() =>
                {
                    LoadCropData(() =>
                    {
                        // ✅ Thêm load ShopItem ở đây
                        LoadShopItems(() =>
                        {
                            Debug.Log("✅ Tất cả dữ liệu load xong, bao gồm ShopItems, chuyển scene...");
                            SceneManager.LoadScene(gameSceneName);
                        });
                    });
                });
            });
        });
    }


    #region Load User
    private void LoadUserData(Action onComplete)
    {
        string userPath = $"Users/{firebaseUser.UserId}";

        firebaseData.ReadObject<User>(
            path: userPath,
            user =>
            {
                UserManager.Instance.userInGame = user;
                userLoaded = true;
                UpdateProgress();
                Debug.Log("✅ User load xong");
                onComplete?.Invoke();
            },
            onNotFound: () =>
            {
                UserManager.Instance.userInGame = new User();
                firebaseData.WriteDatabase(userPath, UserManager.Instance.userInGame);
                userLoaded = true;
                UpdateProgress();
                Debug.Log("🆕 User mới — tạo dữ liệu mặc định");
                onComplete?.Invoke();
            },
            onError: ex =>
            {
                Debug.LogError($"❌ Lỗi load user: {ex.Message}");
                userLoaded = true;
                UpdateProgress();
                onComplete?.Invoke();
            });
    }
    #endregion

    #region Load Item Details
    private void LoadItems(Action onComplete)
    {
        SeedItems.Clear();
        FertilizerItems.Clear();
        AnimalItems.Clear();

        int remainingCategories = 3;
        bool invoked = false;

        LoadItem<Seed>("Items/ItemFarms/Seeds/", SeedItems, CheckDone);
        LoadItem<Fertilizer>("Items/ItemFarms/Fertilizers/", FertilizerItems, CheckDone);
        LoadItem<ItemAnimal>("Items/ItemFarms/Animals/", AnimalItems, CheckDone);
        LoadItem<FarmProduce>("Items/ItemFarms/FarmProduct/", FarmProduceItems, CheckDone);
        LoadItem<Food>("Items/ItemFarms/Food/", FoodItems, CheckDone);

        void CheckDone()
        {
            remainingCategories--;
            if (remainingCategories <= 0 && !invoked)
            {
                itemsLoaded = true;
                UpdateProgress();
                invoked = true;
                Debug.Log("✅ Item details load xong");
                onComplete?.Invoke();
            }
        }
    }

    private void LoadItem<T>(string basePath, List<T> targetList, Action onFinish)
    {
        firebaseData.ReadList<T>(
            path: basePath,
            onSuccess: list =>
            {
                if (list != null) targetList.AddRange(list);
                onFinish?.Invoke();
            },
            onNotFound: () =>
            {
                Debug.LogWarning($"⚠️ Không tìm thấy item tại {basePath}");
                onFinish?.Invoke();
            },
            onError: ex =>
            {
                Debug.LogError($"❌ Lỗi load items tại {basePath}: {ex.Message}");
                onFinish?.Invoke();
            });
    }
    #endregion

    #region Load Animal Data
    private void LoadAnimalData(Action onComplete)
    {
        AnimalDatas.Clear();
        firebaseData.ReadList<AnimalData>(
            "Animal",
            list =>
            {
                if (list != null) AnimalDatas.AddRange(list);
                animalLoaded = true;
                UpdateProgress();
                Debug.Log($"✅ {list?.Count ?? 0} AnimalData load xong");
                onComplete?.Invoke();
            },
            onNotFound: () =>
            {
                animalLoaded = true;
                UpdateProgress();
                Debug.LogWarning("⚠️ Không tìm thấy AnimalData");
                onComplete?.Invoke();
            },
            onError: ex =>
            {
                animalLoaded = true;
                UpdateProgress();
                Debug.LogError($"❌ Lỗi load AnimalData: {ex.Message}");
                onComplete?.Invoke();
            });
    }
    #endregion

    #region Load Crop Data
    private void LoadCropData(Action onComplete)
    {
        Crops.Clear();
        firebaseData.ReadList<CropData>(
            "Crops",
            list =>
            {
                if (list != null) Crops.AddRange(list);
                cropLoaded = true;
                UpdateProgress();
                Debug.Log($"✅ {list?.Count ?? 0} CropData load xong");
                onComplete?.Invoke();
            },
            onNotFound: () =>
            {
                cropLoaded = true;
                UpdateProgress();
                Debug.LogWarning("⚠️ Không tìm thấy CropData");
                onComplete?.Invoke();
            },
            onError: ex =>
            {
                cropLoaded = true;
                UpdateProgress();
                Debug.LogError($"❌ Lỗi load CropData: {ex.Message}");
                onComplete?.Invoke();
            });
    }
    #endregion

    #region Load ShopItem Details
    public void LoadShopItems(Action onComplete = null)
    {
        SeedShopItems.Clear();
        FertilizerShopItems.Clear();
        AnimalShopItems.Clear();

        int remainingCategories = 3;
        bool invoked = false;

        LoadShopCategory("Shops/ItemFarms/Seeds", SeedShopItems, CheckDone);
        LoadShopCategory("Shops/ItemFarms/Fertilizers", FertilizerShopItems, CheckDone);
        LoadShopCategory("Shops/ItemFarms/Animals", AnimalShopItems, CheckDone);

        void CheckDone()
        {
            remainingCategories--;
            if (remainingCategories <= 0 && !invoked)
            {
                invoked = true;
                Debug.Log("✅ ShopItem load xong");
                onComplete?.Invoke();
            }
        }
    }

    private void LoadShopCategory(string path, List<ShopItem> targetList, Action onFinish)
    {
        firebaseData.ReadList<ShopItem>(
            path,
            list =>
            {
                if (list != null) targetList.AddRange(list);
                Debug.Log($"✅ {list?.Count ?? 0} ShopItem từ {path} load xong");
                onFinish?.Invoke();
            },
            onNotFound: () =>
            {
                Debug.LogWarning($"⚠️ Không tìm thấy ShopItem tại {path}");
                onFinish?.Invoke();
            },
            onError: ex =>
            {
                Debug.LogError($"❌ Lỗi load ShopItem tại {path}: {ex.Message}");
                onFinish?.Invoke();
            });
    }
    #endregion


    private void UpdateProgress()
    {
        float progress = 0f;
        if (userLoaded) progress += 0.25f;
        if (itemsLoaded) progress += 0.25f;
        if (animalLoaded) progress += 0.25f;
        if (cropLoaded) progress += 0.25f;
        OnProgressChanged?.Invoke(progress);
        Debug.Log($"[LoadProgress] user={userLoaded}, items={itemsLoaded}, animal={animalLoaded}, crop={cropLoaded} -> {(progress * 100):F0}%");
    }

   

    public List<CropData> GetAllCrops() => Crops;
    
}
