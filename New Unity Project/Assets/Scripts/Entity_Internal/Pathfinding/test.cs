using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

    Vector3[] path;
    public Transform a;
    public Transform b;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        path = Pathfinding.FindPath(a.position, b.position);
	}

    void OnDrawGizmos ()
    {
        if (path == null) return;
        for (int i = 1; i < path.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(path[i-1], path[i]);
        }
        if (Grid.GetGrid() != null)
        {
            Node playerNode = Grid.GetClosestNode(a.transform.position);
            foreach (Node n in Grid.GetGrid())
            {
                Gizmos.color = (n.walkable) ? Color.black : Color.red;
                if (n == playerNode) Gizmos.color = Color.cyan;
                Gizmos.DrawCube(n.position, Vector3.one*0.2f);
            }
        }
    }
}
