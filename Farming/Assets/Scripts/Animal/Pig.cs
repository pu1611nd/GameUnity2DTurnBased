using UnityEngine;

public class Pig : AnimalBase
{
    [Header("💰 Thông tin bán")]
    public float sellCheckInterval = 5f; // Kiểm tra định kỳ xem có thể bán không
    private float checkTimer;

    protected override void Update()
    {
        base.Update();

        if (Data == null || TemplateData == null)
            return;

        // 🧭 Kiểm tra định kỳ xem có thể bán không
        checkTimer += Time.deltaTime;
        if (checkTimer >= sellCheckInterval)
        {
            checkTimer = 0f;

            if (Data.GrowthStage == GrowthStage.Adult)
            {
                Debug.Log($"🐖 Heo (ID: {Data.IdAnimal}) [{TemplateData.type}] đã trưởng thành — sẵn sàng để bán!");
                // 👉 Tại đây bạn có thể bật UI "Bán heo" hoặc hiệu ứng highlight
            }
        }
    }

    /// <summary>
    /// 💰 Bán heo nếu đủ điều kiện
    /// </summary>
    public void Sell()
    {
        if (Data == null || TemplateData == null)
        {
            Debug.LogWarning("🐷 Không có dữ liệu Animal hoặc AnimalData để bán!");
            return;
        }

        if (TrySell(out int price))
        {
            Debug.Log($"💰 Đã bán heo (ID: {Data.IdAnimal}) với giá {price} xu!");
            // 👉 Gợi ý: FarmManager.Instance.AddGold(price);
        }
        else
        {
            Debug.Log($"🐷 Heo (ID: {Data.IdAnimal}) chưa thể bán (chưa trưởng thành).");
        }
    }
}
