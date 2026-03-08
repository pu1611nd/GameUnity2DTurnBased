using UnityEngine;

public class InventoryTrigger2D : MonoBehaviour
{
    public InventoryClickToggle inventoryUI; // gán trong Inspector

    private void OnMouseDown()
    {
        if (inventoryUI != null)
            inventoryUI.ShowInventoryPanel();
    }
}
