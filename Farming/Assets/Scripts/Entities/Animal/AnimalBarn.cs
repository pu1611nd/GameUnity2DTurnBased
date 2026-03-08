using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalBarn
{
    public int IdAnimalBarn;
    public string Type;
    public int Level;
    public int MaxAnimals => CalculateMaxAnimals();
    public List<Animal> Animals = new();

    public AnimalBarn() : this("", 1) { }
    public AnimalBarn(string type, int level = 1)
    {
        Type = type;
        Level = Mathf.Clamp(level, 1, 5);
    }

    private int CalculateMaxAnimals()
    {
        return Type switch
        {
            "Chicken" => 3 + (Level - 1) * 2,
            "Cow" => 2 + (Level - 1),
            "Pig" => 2 + (Level - 1),
            _ => 1
        };
    }

    public bool AddAnimal(Animal animal, AnimalData data)
    {
        if (animal == null || data == null) return false;
        if (!data.type.Equals(Type, StringComparison.OrdinalIgnoreCase)) return false;
        if (Animals.Count >= MaxAnimals) return false;

        if (animal.IdAnimal == 0)
            animal.IdAnimal = UnityEngine.Random.Range(100000, 999999);

        Animals.Add(animal);
        return true;
    }

    public bool RemoveAnimal(int idAnimal)
    {
        var a = Animals.Find(animal => animal.IdAnimal == idAnimal);
        if (a == null) return false;
        Animals.Remove(a);
        return true;
    }

    public Animal GetAnimalById(int idAnimal) => Animals.Find(a => a.IdAnimal == idAnimal);

    public bool Upgrade()
    {
        if (Level >= 5) return false;
        Level++;
        return true;
    }
}