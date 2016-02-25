using UnityEngine;
using System.Collections;
public class NodeValue : IHeapItem<NodeValue>
{
    private Coords parent;
    public Node Parent
    {
        get { return Grid.GetNode(parent); }
        set { parent = value.gridPos; }
    }

    public bool closed = false;

    public int gCost = 0;
    public int hCost = 0;
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
  
    private int heapIndex = 0;
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public int CompareTo(NodeValue a)
    {
        int compare = fCost.CompareTo(a.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(a.hCost);
        }
        return -compare;
    }

    public void Reset ()
    {
        gCost = 0;
        hCost = 0;
        closed = false;
        heapIndex = 0;
    }
}
public class Node { 

    public readonly bool walkable;
    public readonly Vector3 position;
    public readonly Coords gridPos;
    public readonly Coords[] neighbours;

    public NodeValue values;

    public Node (bool walkable, Vector3 position, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.position = position;
        this.gridPos = new Coords(gridX, gridY);
        this.neighbours = GetNeighbours();

        this.values = new NodeValue();
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

        Coords[] output = new Coords[count];
        for (int i = 0; i < count; i++)
        {
            output[i] = nb_holder[i];
        }

        return output;
    }
}
