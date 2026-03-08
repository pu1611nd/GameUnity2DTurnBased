using System;
using UnityEngine;

[Serializable]
public enum GrowthStage
{
    Baby,
    Teen,
    Adult
}

[Serializable]
public class AnimalData
{
    public int idAnimalData;
    public string type;                // "Chicken", "Cow", "Pig"
    public float matureTime;           // giây để trưởng thành
    public float lifeTime;             // giây sống tối đa
    public int sellPrice;              // giá bán khi trưởng thành
    public float hungerDecayRate;      // tốc độ đói / giây
    public int idItem;                 // id vật phẩm tạo ra (trứng, sữa…)
    public float produceInterval;      // thời gian tạo sản phẩm
}

