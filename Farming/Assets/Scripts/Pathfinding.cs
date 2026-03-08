using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [Header("Grid Reference")]
    public PathfindingGrid grid;

    // ✅ Tìm đường giữa 2 vị trí thế giới
    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        if (grid == null)
        {
            Debug.LogError("❌ Pathfinder chưa được gán grid!");
            return null;
        }

        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node targetNode = grid.GetNodeFromWorldPoint(targetPos);

        // Nếu điểm đến không walkable → tìm điểm walkable gần nhất
        if (!targetNode.walkable)
            targetNode = FindNearestWalkable(targetNode);

        if (targetNode == null || !targetNode.walkable)
        {
            Debug.Log("⚠️ Không tìm được node đích hợp lệ!");
            return null;
        }

        // Reset dữ liệu pathfinding (tránh “nhiễm” node cũ)
        foreach (Node n in grid.GetAllNodes())
        {
            n.gCost = 0;
            n.hCost = 0;
            n.parent = null;
        }

        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // ✅ Đã đến đích → dựng lại đường đi
            if (currentNode == targetNode)
                return SmoothPath(RetracePath(startNode, targetNode)); // 💡 Làm mượt ngay tại đây

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newGCost = currentNode.gCost + GetDistance(currentNode, neighbour);

                if (newGCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newGCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    // ✅ Dựng lại đường đi (thô)
    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.worldPosition);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    // ✅ Làm mượt đường đi — loại bỏ node thẳng hàng (phiên bản tối ưu)
    private List<Vector3> SmoothPath(List<Vector3> rawPath)
    {
        if (rawPath == null || rawPath.Count < 3)
            return rawPath;

        List<Vector3> smoothPath = new List<Vector3>();
        smoothPath.Add(rawPath[0]);

        Vector3 oldDir = Vector3.zero;

        for (int i = 1; i < rawPath.Count; i++)
        {
            Vector3 newDir = (rawPath[i] - rawPath[i - 1]).normalized;

            // chỉ thêm node khi hướng thay đổi
            if (newDir != oldDir)
            {
                smoothPath.Add(rawPath[i - 1]);
                oldDir = newDir;
            }
        }

        smoothPath.Add(rawPath[rawPath.Count - 1]);
        return smoothPath;
    }

    // ✅ Tìm node walkable gần nhất nếu node đích bị chặn
    private Node FindNearestWalkable(Node node)
    {
        Queue<Node> searchQueue = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>();

        searchQueue.Enqueue(node);
        visited.Add(node);

        while (searchQueue.Count > 0)
        {
            Node current = searchQueue.Dequeue();

            foreach (Node neighbour in grid.GetNeighbours(current))
            {
                if (visited.Contains(neighbour)) continue;
                if (neighbour.walkable) return neighbour;

                visited.Add(neighbour);
                searchQueue.Enqueue(neighbour);
            }
        }

        return null;
    }

    // ✅ Tính khoảng cách (Manhattan + chéo)
    private int GetDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        int diag = Mathf.Min(dx, dy);
        int straight = Mathf.Abs(dx - dy);
        return diag * 14 + straight * 10;
    }
}
