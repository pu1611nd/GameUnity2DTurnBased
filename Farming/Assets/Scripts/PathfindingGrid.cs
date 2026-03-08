using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("Kích thước 1 ô tile (thường = 1 nếu tilemap chuẩn)")]
    public float tileSize = 1f;

    [Tooltip("Số node chia trên 1 ô tile. Càng cao → đi càng mịn.")]
    [Range(1, 5)] public int gridResolution = 4;

    [Tooltip("Layer của các vật thể có Collider (Cây, Tường, Đá...)")]
    public LayerMask obstacleLayer;

    [Header("Tilemap Settings")]
    public Tilemap walkableTilemap; // Chỉ 1 Tilemap

    private Node[,] grid;
    private Vector3 originPos;
    private int gridSizeX, gridSizeY;
    private float nodeSize;

    private void Awake()
    {
        CreateGrid();
    }

    private void Start()
    {
        if (grid == null)
            CreateGrid();
    }

    public void CreateGrid()
    {
        if (walkableTilemap == null)
        {
            Debug.LogError("❌ Chưa gán Tilemap có thể đi!");
            return;
        }

        nodeSize = tileSize / gridResolution;

        // Tính tổng vùng bao phủ của tilemap
        BoundsInt totalBounds = walkableTilemap.cellBounds;

        gridSizeX = totalBounds.size.x * gridResolution;
        gridSizeY = totalBounds.size.y * gridResolution;

        originPos = walkableTilemap.CellToWorld(totalBounds.min) + new Vector3(nodeSize / 2, nodeSize / 2, 0);
        grid = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPos = originPos + new Vector3(x * nodeSize, y * nodeSize, 0);
                Vector3Int cellPos = walkableTilemap.WorldToCell(worldPos);

                // ✅ Node walkable nếu có tile ở vị trí đó
                bool walkable = walkableTilemap.GetTile(cellPos) != null;

                // ❌ Nếu có collider thật trên layer vật cản → node không đi được
                float playerRadius = 0.1f; // nửa kích thước collider nhỏ hơn nodeSize
                Vector2 size = Vector2.one * (nodeSize * 0.45f + playerRadius);
                if (Physics2D.OverlapBox(worldPos, size, 0f, obstacleLayer))
                    walkable = false;

                grid[x, y] = new Node(walkable, worldPos, x, y);
            }
        }

        Debug.Log($"✅ Grid tạo thành công: {gridSizeX} x {gridSizeY} (nodeSize={nodeSize:F2})");
    }

    public Node GetNodeFromWorldPoint(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - originPos.x) / nodeSize);
        int y = Mathf.RoundToInt((worldPos.y - originPos.y) / nodeSize);
        x = Mathf.Clamp(x, 0, gridSizeX - 1);
        y = Mathf.Clamp(y, 0, gridSizeY - 1);
        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = node.gridX + dx;
                int ny = node.gridY + dy;

                if (nx < 0 || nx >= gridSizeX || ny < 0 || ny >= gridSizeY)
                    continue;

                // Không cho đi chéo xuyên góc vật thể
                if (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 1)
                {
                    if (!grid[node.gridX + dx, node.gridY].walkable ||
                        !grid[node.gridX, node.gridY + dy].walkable)
                        continue;
                }

                neighbours.Add(grid[nx, ny]);
            }
        }

        return neighbours;
    }

    public IEnumerable<Node> GetAllNodes()
    {
        foreach (var node in grid)
            yield return node;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (grid == null) return;
        foreach (Node n in grid)
        {
            Gizmos.color = n.walkable ? new Color(0.8f, 1f, 0.8f, 0.25f) : new Color(1f, 0f, 0f, 0.4f);
            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeSize - 0.02f));
        }
    }
#endif
}

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX, gridY;
    public int gCost, hCost;
    public Node parent;
    public int fCost => gCost + hCost;

    public Node(bool walkable, Vector3 worldPos, int x, int y)
    {
        this.walkable = walkable;
        this.worldPosition = worldPos;
        this.gridX = x;
        this.gridY = y;
        this.parent = null;
    }
}
