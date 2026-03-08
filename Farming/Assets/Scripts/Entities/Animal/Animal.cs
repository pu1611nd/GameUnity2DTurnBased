using System;
using UnityEngine;

[Serializable]
public class Animal
{
    // 🔹 ID
    public int IdAnimal;
    public int AnimalDataId;

    // 🔹 Trạng thái
    public float Hunger;               // 0 = no, 1 = đói
    public GrowthStage GrowthStage;

    // 🔹 Thời gian
    public long CreatedTime;
    public long LastFedTime;
    public long LastProduceTime;

    // ⚙️ Constructor Firebase
    public Animal() { }

    public Animal(int animalDataId)
    {
        AnimalDataId = animalDataId;
        Hunger = 0f;
        CreatedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        LastFedTime = CreatedTime;
        GrowthStage = GrowthStage.Baby;
        IdAnimal = Guid.NewGuid().GetHashCode();
    }

    // ======================
    // 🧠 Core Logic
    // ======================

    public float AgeSeconds => (float)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - CreatedTime);

    public void UpdateGrowthStage(AnimalData data)
    {
        if (data == null) return;

        if (AgeSeconds < data.matureTime * 0.5f)
            GrowthStage = GrowthStage.Baby;
        else if (AgeSeconds < data.matureTime)
            GrowthStage = GrowthStage.Teen;
        else
            GrowthStage = GrowthStage.Adult;
    }

    public float HungerNormalized(AnimalData data)
    {
        if (data == null) return Hunger;

        float decay = data.hungerDecayRate > 0 ? data.hungerDecayRate : 1f / 7200f;
        return Mathf.Clamp01((float)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - LastFedTime) * decay);
    }

    public void UpdateHunger(AnimalData data)
    {
        Hunger = HungerNormalized(data);
    }

    public bool CanBeSold(AnimalData data)
    {
        if (data == null) return false;
        return AgeSeconds >= data.matureTime && AgeSeconds <= data.lifeTime;
    }

    public bool IsDead(AnimalData data)
    {
        if (data == null) return false;
        return AgeSeconds > data.lifeTime;
    }

    public void Feed()
    {
        Hunger = 0f;
        LastFedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
