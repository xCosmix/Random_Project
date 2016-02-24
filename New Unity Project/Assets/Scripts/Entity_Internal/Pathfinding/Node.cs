using UnityEngine;
using System.Collections;

public class Node {

    public readonly bool walkable;
    public readonly Vector3 position;
    public readonly Coords gridPos;
    public readonly Coords[] neighbours;

    #region Pathfinding

    public int gCost;
    public int hCost;
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    public Node parent;

    #endregion

    public Node (bool walkable, Vector3 position, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.position = position;
        this.gridPos = new Coords(gridX, gridY);
        this.neighbours = GetNeighbours();
    }

    private Coords[] GetNeighbours ()
    {
        Coords[] nb_holder = new Coords[8];
        int count = 0;

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0) continue;

                Coords neighbour_coords = new Coords(gridPos.x + x, gridPos.y + y);

                if (!Grid.NodeExist(neighbour_coords)) continue;

                nb_holder[count] = neighbour_coords;
                count++;
            }
        }

        Coords[] output = new Coords[count + 1];
        for (int i = 0; i < count; i++)
        {
            output[i] = nb_holder[i];
        }

        return output;
    }
}
