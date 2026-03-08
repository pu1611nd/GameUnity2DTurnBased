using Newtonsoft.Json;
using System;

[Serializable]
public class User
{
    public string Id;
    public string Name;
    public int Gold;
    public int Diamond;

    // ✅ Thuộc tính sinh lực và thể lực
    public int HP;
    public int MaxHP;
    public int Stamina;
    public int MaxStamina;

    public Farm UserFarm;
    public UserItem Inventory;
    public long LastSaveTime;

    public User()
    {
        Id = "";
        Name = "New Farmer";
        Gold = 0;
        Diamond = 0;

        MaxHP = 100;
        HP = MaxHP;

        MaxStamina = 100;
        Stamina = MaxStamina;

        UserFarm = new Farm();
        Inventory = new UserItem();
        LastSaveTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public User(string id, string name, int gold, int diamond, Farm farm, UserItem userItem, long createdAt, int maxHP = 100, int maxStamina = 100)
    {
        Id = id;
        Name = string.IsNullOrEmpty(name) ? "New Farmer" : name;
        Gold = gold;
        Diamond = diamond;

        MaxHP = maxHP;
        HP = MaxHP;

        MaxStamina = maxStamina;
        Stamina = MaxStamina;

        UserFarm = farm ?? new Farm();
        Inventory = userItem ?? new UserItem();
        LastSaveTime = createdAt;
    }

    /// <summary>
    /// Chuyển sang JSON để lưu trữ
    /// </summary>
    public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
}
