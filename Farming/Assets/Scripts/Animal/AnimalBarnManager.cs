using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBarnManager : MonoBehaviour
{
    public static AnimalBarnManager Instance;
    public List<AnimalBarn> AllBarns = new();
    public event Action OnBarnsChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public List<AnimalBarn> GetPlayerBarns()
    {
        var userFarm = UserManager.Instance?.userInGame?.UserFarm;
        if (userFarm == null || userFarm.Barns == null) return new List<AnimalBarn>();
        return userFarm.Barns;
    }


    public AnimalBarn GetBarnByType(string type)
    {
        var barns = GetPlayerBarns();
        return barns.Find(b => b.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
    }


    public bool AddAnimalToBarn(Animal animal, AnimalData data)
    {
        if (animal == null || data == null) return false;

        var barn = GetBarnByType(data.type);
        if (barn == null) return false;

        bool added = barn.AddAnimal(animal, data);
        if (added) OnBarnsChanged?.Invoke();
        return added;
    }

    public bool SellAnimal(string type, int idAnimal)
    {
        var barn = GetBarnByType(type);
        if (barn == null) return false;

        var animal = barn.GetAnimalById(idAnimal);
        if (animal == null) return false;

        AnimalData data = FarmAnimalLoader.Instance?.GetAnimalData(animal.AnimalDataId);
        if (data == null || !animal.CanBeSold(data)) return false;

        UserManager.Instance.userInGame.Gold += data.sellPrice;
        barn.RemoveAnimal(idAnimal);

        AnimalManager.Instance?.RemoveAnimalFromScene(idAnimal);
        OnBarnsChanged?.Invoke();

        Debug.Log($"💰 Đã bán {data.type} ID:{idAnimal} giá {data.sellPrice}");
        return true;
    }
    public bool UpgradeBarn(string type)
    {
        var barn = GetBarnByType(type);
        if (barn == null) return false;

        bool upgraded = barn.Upgrade(); // gọi Upgrade() trong AnimalBarn
        if (upgraded)
            OnBarnsChanged?.Invoke();

        return upgraded;
    }

    public void UpdateAllBarns()
    {
        foreach (var barn in AllBarns)
        {
            if (barn.Animals == null) continue;

            for (int i = barn.Animals.Count - 1; i >= 0; i--)
            {
                var a = barn.Animals[i];
                AnimalData data = FarmAnimalLoader.Instance?.GetAnimalData(a.AnimalDataId);
                if (data == null) continue;

                a.UpdateHunger(data);
                a.UpdateGrowthStage(data);

                if (a.IsDead(data))
                {
                    barn.Animals.RemoveAt(i);
                    AnimalManager.Instance?.RemoveAnimalFromScene(a.IdAnimal);
                    Debug.Log($"☠️ {data.type} ID:{a.IdAnimal} đã chết.");
                }
            }
        }

        OnBarnsChanged?.Invoke();
    }
}