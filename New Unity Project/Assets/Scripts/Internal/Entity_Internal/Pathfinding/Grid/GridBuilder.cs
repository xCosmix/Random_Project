using UnityEngine;
using System.Collections;
namespace Pathfinding
{
    public class GridBuilder : MonoBehaviour
    {

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
            Gizmos.DrawWireCube(transform.position, new Vector3(worldSize.x, worldSize.y, 0.0f));
        }
    }
}
