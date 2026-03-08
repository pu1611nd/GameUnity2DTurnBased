using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 🐮 Quản lý việc tải & spawn vật nuôi dựa trên dữ liệu người chơi.
/// - Cache toàn bộ AnimalData từ LoadDataManager
/// - Spawn các con vật từ dữ liệu UserFarm
/// - Tự động retry nếu UserManager hoặc LoadDataManager chưa sẵn sàng.
/// </summary>
public class FarmAnimalLoader : MonoBehaviour
{
    public static FarmAnimalLoader Instance { get; private set; }

    public event Action OnDataLoaded; // cho phép đăng ký event từ UI hoặc hệ thống khác

    private readonly Dictionary<int, AnimalData> animalDataCache = new();

    private bool dataLoaded;

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

    private void Start() => TryInitialize();

    /// <summary>
    /// Thử khởi tạo, nếu chưa có dữ liệu sẽ tự gọi lại sau 1s.
    /// </summary>
    private void TryInitialize()
    {
        if (UserManager.Instance?.userInGame == null || LoadDataManager.Instance == null)
        {
            Debug.LogWarning("⚠️ Chưa sẵn sàng (UserManager hoặc LoadDataManager), thử lại sau 1s...");
            Invoke(nameof(TryInitialize), 1f);
            return;
        }

        LoadAllAnimalData();
    }

    // ============================================================
    // 🔹 Tải toàn bộ AnimalData
    // ============================================================
    private void LoadAllAnimalData()
    {
        var list = LoadDataManager.Instance.AnimalDatas;
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("⚠️ LoadDataManager chưa có AnimalDatas, thử lại sau 1s...");
            Invoke(nameof(LoadAllAnimalData), 1f);
            return;
        }

        animalDataCache.Clear();
        foreach (var data in list)
            animalDataCache[data.idAnimalData] = data;

        dataLoaded = true;
        Debug.Log($"✅ Đã cache {list.Count} AnimalData từ LoadDataManager.");

        // Spawn động vật dựa trên dữ liệu người chơi
        LoadAnimalsFromUser();

        OnDataLoaded?.Invoke();
    }

    // ============================================================
    // 🔹 Lấy AnimalData theo ID
    // ============================================================
    public AnimalData GetAnimalData(int id)
    {
        if (!dataLoaded)
        {
            Debug.LogWarning($"⚠️ AnimalData chưa load xong, id={id}");
            return null;
        }

        animalDataCache.TryGetValue(id, out var data);
        return data;
    }

    // ============================================================
    // 🔹 Spawn vật nuôi từ dữ liệu người chơi
    // ============================================================
    private void LoadAnimalsFromUser()
    {
        var user = UserManager.Instance?.userInGame;
        var barns = user?.UserFarm?.Barns;

        if (barns == null || barns.Count == 0)
        {
            Debug.LogWarning("🐔 Không có dữ liệu chuồng trong UserFarm!");
            return;
        }

        if (AnimalManager.Instance == null)
        {
            Debug.LogWarning("⚠️ AnimalManager chưa sẵn sàng, thử spawn lại sau 1s...");
            Invoke(nameof(LoadAnimalsFromUser), 1f);
            return;
        }

        int total = 0;
        foreach (var barn in barns)
        {
            if (barn.Animals == null || barn.Animals.Count == 0) continue;

            foreach (var animal in barn.Animals)
            {
                var template = GetAnimalData(animal.AnimalDataId);
                if (template == null)
                {
                    Debug.LogWarning($"⚠️ Thiếu AnimalData cho ID={animal.AnimalDataId}, bỏ qua vật ID={animal.IdAnimal}");
                    continue;
                }

                AnimalManager.Instance.SpawnAnimal(animal);
                total++;
            }
        }

        Debug.Log($"✅ Đã spawn {total} động vật từ dữ liệu người chơi.");
    }

    // ============================================================
    // 🔹 Reload lại dữ liệu (khi user thay đổi)
    // ============================================================
    public void ReloadAnimals()
    {
        if (!dataLoaded)
        {
            Debug.LogWarning("⚠️ Không thể reload vì chưa load dữ liệu.");
            return;
        }

        AnimalManager.Instance?.ClearAllAnimals();
        LoadAnimalsFromUser();
    }
}
