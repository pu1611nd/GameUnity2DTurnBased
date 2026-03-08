using UnityEngine;

[System.Serializable]
public class Fertilizer : Item
{

    public int sellPrice;            // gia ban cua nong san

    // Constructor mặc định với giá trị khởi tạo
    public Fertilizer()
    {
        sellPrice = 0;
    }
}
