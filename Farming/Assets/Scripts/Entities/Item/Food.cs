using UnityEngine;

[System.Serializable]
public class Food : Item
{

    public int sellPrice;            // gia ban cua nong san

    // Constructor mặc định với giá trị khởi tạo
    public Food()
    {
        sellPrice = 0;
    }
}
