public class InventorySlot
{
    public int idItem;
    public FarmItemCategory category;
    public int quantity;

    public InventorySlot(int id, FarmItemCategory cat, int qty)
    {
        idItem = id;
        category = cat;
        quantity = qty;
    }
}
