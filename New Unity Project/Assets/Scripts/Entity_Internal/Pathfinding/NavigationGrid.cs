using UnityEngine;
using System.Collections.Generic;

public class NavigationGrid : System.Object {

    #region Definition

    public static NavigationGrid navigation_grid;

    public struct GridInfo
    {
        public GridInfo (int x_length, int y_length, int long_length, float node_size)
        {
            this.x_length = x_length;
            this.y_length = y_length;
            this.long_length = long_length;
            this.node_size = node_size;
        }
        public readonly int x_length;
        public readonly int y_length;
        public readonly int long_length;
        public readonly float node_size;
    }
    public struct Coords
    {
        public Coords(int x, int y) { this.x = x; this.y = y; }
        public readonly int x;
        public readonly int y;
    }
    public struct Node
    {
        public Node(int id, Coords coords, Vector3 position)
        {
            this.id = id;
            this.position = position;
            this.coords = coords;

            ocuped = false;
            obsolete = IsObsolete(this.position);
        }

        public readonly Vector3 position;
        public readonly Coords coords;
        public readonly bool obsolete;
        public readonly int id;

        public bool ocuped;

        private static bool IsObsolete (Vector3 pos)
        {
            float height_value = 20.0f;
            Vector3 start_top = pos + new Vector3(0.0f, height_value, 0.0f);
            Vector3 start_bot = pos + new Vector3(0.0f, -height_value, 0.0f);

            RaycastHit hit;

            if (Physics.Linecast(start_top, start_bot, out hit))
            {
                if (hit.collider.isTrigger) return false;
                if (hit.collider.gameObject.isStatic) return true;
            }

            return false;
        }
    }

    protected Node[,] node_list;
    protected GridInfo grid_info;

    public NavigationGrid (Node[,] node_list)
    {
        navigation_grid = this;
        
        int x_length = node_list.GetLength(0);
        int y_length = node_list.GetLength(1);
        int long_length = x_length * y_length;

        Vector3 position_first = node_list[0, 0].position;
        Vector3 position_first_x = node_list[1, 0].position;
        Vector3 position_first_y = node_list[0, 1].position;

        float node_size = position_first_x - position_first != Vector3.zero ? 
            (position_first_x - position_first).magnitude 
            : 
            (position_first_y - position_first).magnitude;

        this.node_list = node_list;
        this.grid_info = new GridInfo(x_length, y_length, long_length, node_size);
    }

    #endregion

    #region Utils

    #region Get

    public GridInfo GetGridInfo ()
    {
        return grid_info;
    }
    public bool XExist (int x)
    {
        if (x < 0) return false;
        if (x >= grid_info.x_length) return false;

        return true;
    }
    public bool YExist (int y)
    {
        if (y < 0) return false;
        if (y >= grid_info.y_length) return false;

        return true;
    }
    public bool NodeExist (int x, int y)
    {
        return XExist(x) && YExist(y);
    }
    public bool NodeExist(Coords coords)
    {
        return XExist(coords.x) && YExist(coords.y);
    }
    public bool NodeIsObsolete(int x, int y)
    {
        return node_list[x, y].obsolete;
    }
    public bool NodeIsObsolete(Coords coords)
    {
        return node_list[coords.x, coords.y].obsolete;
    }
    public bool NodeIsObstructed(int x, int y)
    {
        return node_list[x, y].ocuped;
    }
    public bool NodeIsObstructed(Coords coords)
    {
        return node_list[coords.x, coords.y].ocuped;
    }
    public Node GetNode (int x, int y)
    {
        int out_x = Mathf.Clamp(x, 0, grid_info.x_length-1);
        int out_y = Mathf.Clamp(y, 0, grid_info.y_length-1);

        return node_list[out_x, out_y];
    }
    public Node GetNode(Coords coords)
    {
        int out_x = Mathf.Clamp(coords.x, 0, grid_info.x_length - 1);
        int out_y = Mathf.Clamp(coords.y, 0, grid_info.y_length - 1);

        return node_list[out_x, out_y];
    }
    public Node[] GetRow (int y)
    {
        Node[] row;

        if (!YExist(y)) { return row = new Node[0]; }

        row = new Node[grid_info.x_length];
        for (int i = 0; i < grid_info.x_length; i++)
        {
            row[i] = node_list[i, y];
        }

        return row;
    }
    public Node[] GetColumn(int x)
    {
        Node[] column;

        if (!XExist(x)) { return column = new Node[0]; }

        column = new Node[grid_info.y_length];
        for (int i = 0; i < grid_info.y_length; i++)
        {
            column[i] = node_list[x, i];
        }

        return column;
    }
    public Node GetClosestNode (Vector3 position)
    {
        Vector3 startPos = node_list[0, 0].position;
        Vector3 delta = position - startPos;

        int x_value = Mathf.RoundToInt(delta.x / grid_info.node_size);
        int y_value = Mathf.RoundToInt(delta.z / grid_info.node_size);

        Node node = GetNode(x_value, y_value);

        return node;
    }

    #endregion

    #region Set

    public void SetNodeOcuped (int x, int y, bool ocuped_value)
    {
        node_list[x, y].ocuped = ocuped_value;
    }

    public void SetNodeOcuped(Coords coords, bool ocuped_value)
    {
        node_list[coords.x, coords.y].ocuped = ocuped_value;
    }

    public void SetNodeListOcuped(bool ocuped_value)
    {
        for (int x = 0; x < grid_info.x_length; x++)
        {
            for (int y = 0; y < grid_info.y_length; y++)
            {
                SetNodeOcuped(x, y, ocuped_value);
            }
        }
    }

    #endregion

    #endregion
}
