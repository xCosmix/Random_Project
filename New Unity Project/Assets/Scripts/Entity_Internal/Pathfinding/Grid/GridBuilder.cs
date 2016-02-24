using UnityEngine;
using System.Collections;

public class GridBuilder : MonoBehaviour {

    public LayerMask unwalkableMask;
    public Vector2 worldSize;
    public float nodeRadius;

    private float nodeSize;
    private Coords gridSize;
    private Node[,] grid;

    void Start()
    {
        Grid.Create(nodeRadius, worldSize, transform.position, unwalkableMask);
        Destroy(gameObject);
    }


    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(worldSize.x, 1, worldSize.y)); 
    }
}
