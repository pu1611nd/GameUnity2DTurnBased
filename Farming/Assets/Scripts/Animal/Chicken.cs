using UnityEngine;
using System;

/// <summary>
/// 🐔 Chicken — vật nuôi đẻ trứng.
/// Kế thừa từ AnimalBase, xử lý sinh sản offline & online.
/// </summary>
public class Chicken : AnimalBase
{
    [Header("🥚 Sinh sản")]
    public EggNest nest;                  // Gán EggNest trong scene
    private float eggTimer;
    private const float HungerLimitForLay = 0.8f;

    protected override void Start()
    {
        base.Start();

        // Tìm EggNest nếu chưa gán
        if (nest == null)
            nest = FindObjectOfType<EggNest>();

        if (nest == null)
        {
            Debug.LogWarning($"{name}: chưa gán Nest!");
            return;
        }

        // --- Sản xuất trứng offline ---
        int offlineEggs = ProduceOfflineEggs();
        if (offlineEggs > 0)
        {
            nest.AddEgg(offlineEggs);
            Debug.Log($"🥚 Chicken ID {Data.IdAnimal} sản xuất {offlineEggs} trứng offline!");
        }

        // --- Khởi tạo eggTimer dựa trên lastProduceTime ---
        float interval = TemplateData != null && TemplateData.produceInterval > 0
            ? TemplateData.produceInterval
            : 20f;

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long last = Data.LastProduceTime > 0 ? Data.LastProduceTime : Data.CreatedTime;
        eggTimer = Mathf.Min(interval, now - last);
    }

    protected override void Update()
    {
        base.Update();

        if (Data == null || TemplateData == null || nest == null) return;

        Data.UpdateHunger(TemplateData);

        if (CanLayEgg())
            HandleEggProduction();
    }

    /// <summary>
    /// Tính số trứng sản xuất offline (khi game load)
    /// </summary>
    public int ProduceOfflineEggs()
    {
        if (Data == null || TemplateData == null) return 0;

        float interval = TemplateData.produceInterval > 0 ? TemplateData.produceInterval : 20f;
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long last = Data.LastProduceTime > 0 ? Data.LastProduceTime : Data.CreatedTime;

        int eggs = Mathf.FloorToInt((now - last) / interval);
        if (eggs > 0)
            Data.LastProduceTime = last + (long)(eggs * interval);

        return eggs;
    }

    /// <summary>
    /// Kiểm tra điều kiện để gà đẻ trứng
    /// </summary>
    private bool CanLayEgg()
    {
        return Data.GrowthStage == GrowthStage.Adult &&
               Data.Hunger < HungerLimitForLay &&
               !Data.IsDead(TemplateData);
    }

    /// <summary>
    /// Xử lý sản xuất trứng theo thời gian
    /// </summary>
    private void HandleEggProduction()
    {
        float interval = TemplateData.produceInterval > 0 ? TemplateData.produceInterval : 20f;
        eggTimer += Time.deltaTime;

        if (eggTimer >= interval)
        {
            LayEgg();
            eggTimer = 0f;
            Data.LastProduceTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }

    /// <summary>
    /// Thêm trứng vào Nest
    /// </summary>
    private void LayEgg()
    {
        if (nest == null) return;

        nest.AddEgg(1);
        Debug.Log($"🥚 Gà (ID: {Data?.IdAnimal}) đẻ trứng! Tổng trứng trong Nest: {nest.eggCount}");
    }
}
