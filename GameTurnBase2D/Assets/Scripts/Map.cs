using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MapPosition
{
    public Vector3 Position; // Vị trí trên bản đồ
    public bool IsOccupied; // Trạng thái có bị chiếm không
}

public class Map : MonoBehaviour
{
    public List<MapPosition> Positions; // Danh sách các vị trí trên bản đồ
    public GameObject positionMarkerPrefab; // Prefab marker

    private void Start()
    {
        InitializeMap();
    }

    private void InitializeMap()
    {
        // Khởi tạo các vị trí
        Positions = new List<MapPosition>
        {
            new MapPosition { Position = new Vector3(-3, -2.5f, 0), IsOccupied = false },
            new MapPosition { Position = new Vector3(-5, -2.5f, 0), IsOccupied = false },
            new MapPosition { Position = new Vector3(-3, -4, 0), IsOccupied = false },
            new MapPosition { Position = new Vector3(-5, -4, 0), IsOccupied = false },

            new MapPosition { Position = new Vector3(5, -2.5f, 0), IsOccupied = false },
            new MapPosition { Position = new Vector3(3, -2.5f, 0), IsOccupied = false },
            new MapPosition { Position = new Vector3(5, -4, 0), IsOccupied = false },
            new MapPosition { Position = new Vector3(3, -4, 0), IsOccupied = false }
        };

        // Tạo marker tại các vị trí
        foreach (var pos in Positions)
        {
            if (positionMarkerPrefab != null)
            {
                Instantiate(positionMarkerPrefab, pos.Position, Quaternion.identity);
            }
        }
    }
}
