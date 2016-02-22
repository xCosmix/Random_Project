using UnityEngine;
using System.Collections;

public class NavigationGrid : System.Object {

    #region Definition

    public static NavigationGrid navigation_grid;

    public struct Coords
    {
        public Coords(int x, int y) { this.x = x; this.y = y; }
        public readonly int x;
        public readonly int y;
    }
    public struct Node
    {
        public Node(int id, Vector3 position)
        {
            this.id = id;
            this.position = position;
        }
        public readonly Vector3 position;
        public readonly int id;
    }

    protected Node[,] grid;
    
    protected int x_length;
    protected int y_length;
    protected int long_length;

    protected float node_size;

    public NavigationGrid (Node[,] grid)
    {
        navigation_grid = this;
        this.grid = grid;

        x_length = grid.GetLength(0);
        y_length = grid.GetLength(1);
        long_length = x_length * y_length;

        Vector3 position_first = grid[0, 0].position;
        Vector3 position_first_x = grid[1, 0].position;
        Vector3 position_first_y = grid[0, 1].position;

        node_size = position_first_x - position_first != Vector3.zero ? 
            (position_first_x - position_first).magnitude 
            : 
            (position_first_y - position_first).magnitude;
    }

    #endregion

    #region Utils

    public bool XExist (int x)
    {
        if (x < 0) return false;
        if (x >= x_length) return false;

        return true;
    }
    public bool YExist (int y)
    {
        if (y < 0) return false;
        if (y >= y_length) return false;

        return true;
    }
    public bool NodeExist (int x, int y)
    {
        return XExist(x) && YExist(y);
    }
    public Node GetNode (int x, int y)
    {
        int out_x = Mathf.Clamp(x, 0, x_length-1);
        int out_y = Mathf.Clamp(y, 0, y_length-1);

        return grid[out_x, out_y];
    }
    public Node[] GetRow (int y)
    {
        Node[] row;

        if (!YExist(y)) { return row = new Node[0]; }

        row = new Node[x_length];
        for (int i = 0; i < x_length; i++)
        {
            row[i] = grid[i, y];
        }

        return row;
    }
    public Node[] GetColumn(int x)
    {
        Node[] column;

        if (!XExist(x)) { return column = new Node[0]; }

        column = new Node[y_length];
        for (int i = 0; i < y_length; i++)
        {
            column[i] = grid[x, i];
        }

        return column;
    }
    public Node GetClosestNode (Vector3 position)
    {
        Vector3 startPos = grid[0, 0].position;
        Vector3 delta = position - startPos;

        int x_value = Mathf.FloorToInt(delta.x / node_size);
        int y_value = Mathf.FloorToInt(delta.z / node_size);

        Node node = GetNode(x_value, y_value);

        return node;
    }

    #endregion
}
