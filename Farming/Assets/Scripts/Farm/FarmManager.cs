using System;
using System.Collections.Generic;
using UnityEngine;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance;

    [Header("Danh sách cây trồng có sẵn (lấy từ LoadDataManager)")]
    public List<CropData> availableCrops = new List<CropData>();

    [Header("Database cây trồng (ScriptableObject, tùy chọn)")]
    public CropDatabase cropDatabase;

    private void Awake()
    {
        // 🧠 Đảm bảo singleton duy nhất
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
        LoadCropData();
    }

    /// 🔹 Lấy dữ liệu cây từ LoadDataManager
    public void LoadCropData(Action onComplete = null)
    {
        if (LoadDataManager.Instance == null)
        {
            Debug.LogError("❌ LoadDataManager chưa khởi tạo!");
            onComplete?.Invoke();
            return;
        }


        // Lấy dữ liệu từ LoadDataManager
        availableCrops.Clear();
        availableCrops.AddRange(LoadDataManager.Instance.Crops);
        Debug.Log($"✅ Đã load {availableCrops.Count} cây từ LoadDataManager.");
        onComplete?.Invoke();
    }

    /// 🔍 Lấy cây theo ID
    public CropData GetCropByID(int id)
    {
        return availableCrops.Find(c => c.CropId == id);
    }

    /// 🔄 Lấy toàn bộ danh sách cây
    public List<CropData> GetAllCrops()
    {
        return availableCrops;
    }
}
