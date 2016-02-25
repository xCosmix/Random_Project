using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(test))]
public class DebugAStar : Editor {
    void OnSceneGUI ()
    { 
        /*
        Node[,] nds = Grid.GetGrid();
        if (nds == null) return;
        foreach (Node n in nds) {
            Handles.color = Color.white;
            //Handles.Label(n.position + Vector3.forward * 2, "gCost: " + n.gCost);
            //Handles.Label(n.position + Vector3.forward, "hCost: " + n.hCost);
            Handles.Label(n.position + Vector3.forward, "fCost: " + n.fCost);
            Handles.RectangleCap(1, n.position, Quaternion.LookRotation(Vector3.up), Grid.GetNodeSize()*0.5f);
            if (n.parent != null)
            {
                Handles.color = Color.blue;
                Vector3 dir = n.position - n.parent.position;
                Handles.DrawPolyLine(n.parent.position, n.position);
            }
        }
        */
    }
}
