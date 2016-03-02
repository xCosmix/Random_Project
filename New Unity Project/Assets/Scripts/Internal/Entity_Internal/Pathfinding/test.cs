using UnityEngine;
using System.Diagnostics;
using Pathfinding;

public class test : MonoBehaviour {
    public static double averageTime;
    public static int iterations;
    public Transform[] a;
     Vector3[][] paths;
    public Transform b;
	// Use this for initialization
	void Start () {
        paths = new Vector3[a.Length][];
    }
	
	// Update is called once per frame
	void Update () {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        for (int i = 0; i < a.Length; i++)
        {
            paths[i] = Path.FindPath(a[i].position, b.position);
        }
        //debug
        sw.Stop();
        averageTime += sw.Elapsed.TotalMilliseconds;
        iterations++;
        UnityEngine.Debug.Log(averageTime / (double)iterations);
        //end debug
    }

    void OnDrawGizmos ()
    {
        if (paths == null) return;
        foreach (Vector3[] path in paths)
        {
            for (int i = 1; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
        if (Grid.GetGrid() != null)
        {
            foreach (Node n in Grid.GetGrid())
            {
                Gizmos.color = (n.walkable) ? Color.black : Color.red;
                Gizmos.DrawCube(n.position, Vector3.one*0.2f);
            }
        }
    }
}
