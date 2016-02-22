using UnityEngine;
using System.Collections;

public class BuildGrid : MonoBehaviour {

    #region Definition

    public float node_size = 2.0f;

    protected GameObject target;
    protected Mesh target_mesh;
    protected Bounds target_bounds;
    protected Vector3 max;
    protected Vector3 min;

    protected int node_mount;
    protected int node_x;
    protected int node_y;

    protected Vector3 start_point;

    #endregion

    // Use this for initialization
    void Start()
    {
        Define();
        Create();
    }

    #region Steps 

    protected void Define ()
    {
        target = this.gameObject;
        target_mesh = target.GetComponent<MeshFilter>().mesh;
        target_bounds = target_mesh.bounds;
        max = transform.position + Vector3.Scale(target_bounds.max, transform.localScale);
        min = transform.position + Vector3.Scale(target_bounds.min, transform.localScale);

        node_x = Mathf.CeilToInt((max.x - min.x) / node_size);
        node_y = Mathf.CeilToInt((max.z - min.z) / node_size);
        node_mount = node_x * node_y;

        start_point = new Vector3(min.x + node_size * 0.5f, transform.position.y, min.z + node_size * 0.5f);
    }

    protected void Create ()
    {

        NavigationGrid.Node[,] nodes = new NavigationGrid.Node[node_x, node_y];

        for (int y = 0; y < node_y; y++)
        {
            for (int x = 0; x < node_x; x++)
            {
                int id = y * node_y + x;
                Vector3 node_position = start_point;
                node_position.x += node_size * x;
                node_position.z += node_size * y;

                nodes[x, y] = new NavigationGrid.Node(id, node_position);
            }
        }

        new NavigationGrid(nodes);
    }

    #endregion
}

