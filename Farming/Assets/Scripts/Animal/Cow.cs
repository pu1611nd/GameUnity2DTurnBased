using UnityEngine;

public class Cow : AnimalBase
{
    [Header("🥛 Sản phẩm")]
    public float defaultMilkInterval = 25f;  // Nếu AnimalData không có giá trị thì dùng cái này
    private float milkTimer;

    protected override void Update()
    {
        base.Update();

        if (Data == null || TemplateData == null)
            return;

        // 🧭 Chỉ cho sữa khi đã trưởng thành
        if (Data.GrowthStage == GrowthStage.Adult)
        {
            milkTimer += Time.deltaTime;

            // Lấy khoảng thời gian từ AnimalData hoặc fallback sang default
            float interval = TemplateData.produceInterval > 0 ? TemplateData.produceInterval : defaultMilkInterval;

            if (milkTimer >= interval)
            {
                ProduceMilk();
                milkTimer = 0f;
            }
        }
    }

    private void ProduceMilk()
    {
        Debug.Log($"🐄 Bò (ID: {Data.IdAnimal}) đã cho sữa! [Type={TemplateData.type}]");

        // 👉 Ở đây bạn có thể:
        // - Tăng item "Milk" vào kho:
        //     FarmInventoryManager.Instance.AddItem(TemplateData.idItem, FarmItemCategory.FarmProduce, 1);
        // - Sinh prefab chai sữa trong thế giới nếu muốn:
        //     Instantiate(milkPrefab, spawnPoint.position, Quaternion.identity);
        // - Gửi event cho hệ thống FarmManager nếu có.
    }
}
