using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFarm
{
    public List<InventorySlot> FarmProduct;        // danh sach nong san
    public List<InventorySlot> Seeds;       // danh sách hat giong
    public List<InventorySlot> Fertilizer;        // danh sach phan bon va thuc an chan nuoi
    public List<InventorySlot> Food;       // danh sách do an

    public ItemFarm()
    {
        FarmProduct = new List<InventorySlot>();
        Seeds = new List<InventorySlot>();
        Fertilizer = new List<InventorySlot>();
        Food = new List<InventorySlot>();
    }
}
