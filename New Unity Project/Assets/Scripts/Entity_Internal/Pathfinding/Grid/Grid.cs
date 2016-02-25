using UnityEngine;
using System.Collections.Generic;

public static class Grid {

    private static Node[,] grid;
    private static float nodeSize;
    private static Coords gridSize;

    public static void Create (float nodeRadius, Vector2 worldSize, Vector3 center, LayerMask unwalkableMask)
    {
        Grid.nodeSize = nodeRadius*2;
        int gridSizeX = Mathf.RoundToInt(worldSize.x / nodeSize);
        int gridSizeY = Mathf.RoundToInt(worldSize.y / nodeSize);
        gridSize = new Coords(gridSizeX, gridSizeY);
        grid = new Node[gridSize.x, gridSize.y];

        Vector3 bottomLeft = center - (Vector3.right * (worldSize.x / 2)) - (Vector3.forward * (worldSize.y / 2));

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeSize + nodeRadius) + Vector3.forward * (y * nodeSize + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public static Node[,] GetGrid ()
    {
        return grid;
    }
    public static Coords GetSize ()
    {
        return gridSize;
    }
    public static int GetMaxSize ()
    {
        return gridSize.x * gridSize.y;
    }
    public static float GetNodeSize ()
    {
        return nodeSize;
    }
    public static Node GetClosestNode (Vector3 point)
    {
        Vector3 startPoint = grid[0, 0].position;
        float deltaX = point.x - startPoint.x;
        float deltaY = point.z - startPoint.z;

        int x = Mathf.RoundToInt(deltaX / nodeSize);
        int y = Mathf.RoundToInt(deltaY / nodeSize);

        return GetNode(x, y);
    }
    public static Node GetNode (int x, int y)
    {
        x = Mathf.Clamp(x, 0, gridSize.x - 1);
        y = Mathf.Clamp(y, 0, gridSize.y - 1);

        return grid[x, y];
    }
    public static Node GetNode(Coords coords)
    {
        int x = Mathf.Clamp(coords.x, 0, gridSize.x - 1);
        int y = Mathf.Clamp(coords.y, 0, gridSize.y - 1);

        return grid[x, y];
    }
    public static int GetDistance(Node from, Node to)
    {
        int distX = Mathf.Abs(from.gridPos.x - to.gridPos.x);
        int distY = Mathf.Abs(from.gridPos.y - to.gridPos.y);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }

    public static bool NodeExist (int x, int y)
    {
        return x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y;
    }
    public static bool NodeExist(Coords coords)
    {
        return coords.x >= 0 && coords.x < gridSize.x && coords.y >= 0 && coords.y < gridSize.y;
    }
     
}
