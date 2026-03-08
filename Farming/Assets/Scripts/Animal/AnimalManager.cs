using UnityEngine;
using System.Collections.Generic;

public class AnimalManager : MonoBehaviour
{
    public static AnimalManager Instance;

    [Header("🐔 Prefab động vật")]
    public GameObject chickenPrefab;
    public GameObject cowPrefab;
    public GameObject pigPrefab;

    [Header("🏡 Vùng di chuyển trong chuồng")]
    public Collider2D chickenArea;
    public Collider2D cowArea;
    public Collider2D pigArea;


    private readonly Dictionary<int, GameObject> activeAnimals = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SpawnAnimal(Animal animal)
    {
        if (animal == null)
        {
            Debug.LogWarning("⚠️ Animal null khi spawn!");
            return;
        }

        if (activeAnimals.ContainsKey(animal.IdAnimal))
        {
            Debug.LogWarning($"⚠️ Animal {animal.IdAnimal} đã tồn tại trong scene!");
            return;
        }

        // 🔥 Lấy dữ liệu AnimalData từ Firebase cache
        AnimalData template = FarmAnimalLoader.Instance?.GetAnimalData(animal.AnimalDataId);
        if (template == null)
        {
            Debug.LogError($"❌ Không tìm thấy AnimalData ID={animal.AnimalDataId} trong FirebaseManager!");
            return;
        }

        GameObject prefab = null;
        Collider2D area = null;

        switch (template.type)
        {
            case "Chicken":
                prefab = chickenPrefab;
                area = chickenArea;
                break;
            case "Cow":
                prefab = cowPrefab;
                area = cowArea;
                break;
            case "Pig":
                prefab = pigPrefab;
                area = pigArea;
                break;
            default:
                Debug.LogError($"❌ Loại động vật không hợp lệ: {template.type}");
                return;
        }

        Vector3 spawnPos = area != null ? GetRandomPositionInArea(area) : Vector3.zero;
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
        obj.name = $"{template.type}_{animal.IdAnimal}";

        var animalBase = obj.GetComponent<AnimalBase>();
        if (animalBase != null)
        {
            animalBase.Data = animal;
            animalBase.TemplateData = template;
            animalBase.areaCollider = area;


        }

        activeAnimals[animal.IdAnimal] = obj;
        Debug.Log($"✅ Spawn {template.type} (ID={animal.IdAnimal}) tại {spawnPos}");
    }

    private Vector3 GetRandomPositionInArea(Collider2D area)
    {
        Bounds b = area.bounds;
        Vector2 pos;
        int safety = 0;

        do
        {
            pos = new Vector2(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y));
            safety++;
        } while (!area.OverlapPoint(pos) && safety < 30);

        return pos;
    }

    public void ClearAllAnimals()
    {
        foreach (var go in activeAnimals.Values)
            if (go != null) Destroy(go);
        activeAnimals.Clear();
    }

    public void RespawnAllAnimalsFromUser()
    {
        ClearAllAnimals();

        var user = UserManager.Instance?.userInGame;
        if (user?.UserFarm?.Barns == null)
        {
            Debug.LogWarning("⚠️ Không có dữ liệu chuồng trong user!");
            return;
        }

        foreach (var barn in user.UserFarm.Barns)
            foreach (var animal in barn.Animals)
                SpawnAnimal(animal);

        Debug.Log($"🐮 Respawn xong {activeAnimals.Count} động vật từ dữ liệu user.");
    }

    public void RemoveAnimalFromScene(int idAnimal)
    {
        if (activeAnimals.TryGetValue(idAnimal, out GameObject obj))
        {
            if (obj != null) Destroy(obj);
            activeAnimals.Remove(idAnimal);
            Debug.Log($"🗑️ Đã xóa vật nuôi ID={idAnimal} khỏi scene.");
        }
    }

    public void SyncWithUserData()
    {
        HashSet<int> idsInScene = new(activeAnimals.Keys);

        var user = UserManager.Instance?.userInGame;
        if (user?.UserFarm?.Barns == null) return;

        HashSet<int> idsInData = new();
        foreach (var barn in user.UserFarm.Barns)
            foreach (var a in barn.Animals)
                idsInData.Add(a.IdAnimal);

        // Xóa các con không còn
        foreach (int id in idsInScene)
            if (!idsInData.Contains(id))
                RemoveAnimalFromScene(id);

        // Spawn con mới
        foreach (var barn in user.UserFarm.Barns)
            foreach (var a in barn.Animals)
                if (!activeAnimals.ContainsKey(a.IdAnimal))
                    SpawnAnimal(a);

        Debug.Log($"🔁 Đồng bộ scene vật nuôi xong. Tổng: {activeAnimals.Count}");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit == null) return;

            AnimalBase animalBase = hit.GetComponent<AnimalBase>() ?? hit.GetComponentInParent<AnimalBase>();
            if (animalBase == null || animalBase.Data == null) return;

            var user = UserManager.Instance?.userInGame;
            var barns = user?.UserFarm?.Barns;
            if (barns == null) return;

            AnimalBarn barn = null;
            foreach (var b in barns)
            {
                if (b.Animals.Exists(a => a.IdAnimal == animalBase.Data.IdAnimal))
                {
                    barn = b;
                    break;
                }
            }

            if (barn == null)
            {
                Debug.LogWarning("❌ Không tìm thấy chuồng chứa con vật này!");
                return;
            }

            AnimalActionMenu.Instance?.Show(animalBase, barn);
            Debug.Log($"🐾 Click vào {animalBase.TemplateData.type} ID={animalBase.Data.IdAnimal}");
        }
    }
}
