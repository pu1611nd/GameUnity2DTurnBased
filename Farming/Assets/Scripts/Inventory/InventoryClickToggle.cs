using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryClickToggle : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject inventoryPanel; // panel balo

    private void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false); // ẩn mặc định
    }

    public void ShowInventoryPanel()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);
    }

    public void HideInventoryPanel()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Nếu click ra ngoài UI
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

                // Nếu không click vào vật thể trigger → đóng balo
                if (hit.collider == null || hit.collider.GetComponent<InventoryTrigger2D>() == null)
                {
                    HideInventoryPanel();
                }
            }
        }
    }
}
