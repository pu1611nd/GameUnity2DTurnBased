using UnityEngine;

public enum FarmItemCategory
{
    FarmProduct,
    Seeds,
    Fertilizer,
    Food,
    Animals
}
[System.Serializable]
public class Item
{
    public int idItem;                // ID duy nhất
    public string itemName;           // Tên item
    public string description;          // mo ta

    // Constructor mặc định với giá trị khởi tạo
    public Item()
    {
        idItem = 0;
        itemName = "";
        description = "";
    }


}
