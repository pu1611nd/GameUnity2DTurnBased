using UnityEngine;

[System.Serializable]
public class FarmProduce : Item
{

    public int sellPrice;            // gia ban cua nong san

    // Constructor mặc định với giá trị khởi tạo
    public FarmProduce()
    {
        sellPrice = 0;
    }
}
