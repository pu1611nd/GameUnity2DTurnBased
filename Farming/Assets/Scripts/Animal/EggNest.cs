using UnityEngine;

public class EggNest : MonoBehaviour
{
    public int eggCount = 0;                  // Tổng số trứng đã sinh
    public GameObject eggVisualPrefab;        // Prefab trứng hiển thị duy nhất
    private GameObject eggVisualInstance;
    private AnimalData animalData;

    public void AddEgg(int amount = 1)
    {
        eggCount += amount;

        if (eggVisualPrefab != null && eggVisualInstance == null)
        {
            eggVisualInstance = Instantiate(eggVisualPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    private void OnMouseDown()
    {
        if (eggCount == 0)
        {
            Debug.Log("🌾 Không có trứng để thu hoạch!");
            return;
        }

        // TODO: cộng số trứng vào Inventory/Firebase
        animalData = FarmAnimalLoader.Instance?.GetAnimalData(0);
        if (FarmInventoryManager.Instance != null && eggCount > 0)
        {

            FarmInventoryManager.Instance.AddItem(animalData.idItem, FarmItemCategory.FarmProduct, eggCount);
            Debug.Log($"🥚 Thu hoạch {eggCount} trứng từ ổ rơm!");
        }

        eggCount = 0;

        if (eggVisualInstance != null)
        {
            Destroy(eggVisualInstance);
            eggVisualInstance = null;
        }
    }
}
