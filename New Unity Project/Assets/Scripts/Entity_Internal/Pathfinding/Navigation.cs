using UnityEngine;
using System.Collections.Generic;

public class Navigation : MonoBehaviour {

    #region Definition

    protected List<Transform> agents = new List<Transform>();

    #endregion

    #region Steps

    void Update()
    {
        RefreshAgents();
    }

    protected void RefreshAgents ()
    {
        foreach (Transform agent in agents)
        {

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
