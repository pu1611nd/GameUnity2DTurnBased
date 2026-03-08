using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [Header("Tham chiếu đến UI Shop hoặc Shop Manager")]
    public GameObject shopUI; // 👈 Kéo UI Shop prefab hoặc panel vào đây
    public FarmIShopManager shopManager; // 👈 Tham chiếu đến script FarmIShopManager

    [Header("Tag của nhân vật player")]
    public string playerTag = "Player"; // 👈 Gắn tag "Player" cho nhân vật


    private void Start()
    {
        if (shopManager == null)
            shopManager = FindObjectOfType<FarmIShopManager>();

        if (shopUI != null)
            shopUI.SetActive(false); // Ẩn shop lúc đầu
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            Debug.Log("🧍‍♂️ Người chơi đã vào khu vực shop!");

            // ✅ Mở shop
            if (shopUI != null)
                shopUI.SetActive(true);

            // Nếu muốn load lại dữ liệu shop mỗi lần mở
            //shopManager?.LoadShopItems();
           
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            Debug.Log("🚶‍♂️ Người chơi đã rời khỏi shop!");

            // ✅ Đóng shop khi rời đi
            if (shopUI != null)
                shopUI.SetActive(false);
        }
    }
}
