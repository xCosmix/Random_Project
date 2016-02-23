using UnityEngine;
using System.Collections.Generic;

public class NavigationController : MonoBehaviour {

    #region Definition

    protected static NavigationController instance;

    protected List<Transform> agents = new List<Transform>();

    public static NavigationController Get ()
    {
        Create();
        return instance;
    }
    private static void Create ()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<NavigationController>();
            if (instance == null)
            {
                GameObject go = new GameObject("Navigation Controller");
                instance = go.AddComponent<NavigationController>();
                go.hideFlags = HideFlags.HideAndDontSave;
                go.isStatic = true;     
            }
        }
    }

    #endregion

    #region Steps

    void Update()
    {
        RefreshAgents();
    }

    /// <summary>
    /// Obstuct and free nodes when subscribers move through the navigation grid
    /// </summary>
    protected void RefreshAgents ()
    {
        //Clean list of ocuped nodes
        NavigationGrid.navigation_grid.SetNodeListOcuped(false);

        foreach (Transform agent in agents)
        {
            NavigationGrid.Node step_node = NavigationGrid.navigation_grid.GetClosestNode(agent.position);
            NavigationGrid.navigation_grid.SetNodeOcuped(step_node.coords, true);
        }
    }

    #endregion

    #region Utils

    public void Subscribe (Transform t)
    {
        if (agents.Contains(t)) return;
        agents.Add(t);
    }

    #endregion
}
