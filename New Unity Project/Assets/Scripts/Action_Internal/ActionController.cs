using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionController : MonoBehaviour {

    private List<Action> active_actions = new List<Action>();

    /// <summary>
    /// suscribe to the events of action controller
    /// </summary>
    /// <param name="instance"></param>
    /// <returns>
    /// if the subscription succeeded return true
    /// </returns>
    public bool Subscribe(Action instance)
    {
        ///This is useless double check, because in the action before instantiating it, there is the same check
        //if (IsCloned(instance.name, instance.layer)) return false;

        active_actions.Add(instance);
        return true;
    }
    void Update ()
    {
        foreach (Action action in active_actions)
        {
            action.components.action_instance.CallStep();
            action.components.action_instance.UpdateState();
        }
    }
    public ActionInstance.State UpdateState(Action instance)
    {
        ActionInstance.State last_state = instance.components.action_instance.state;

        List<Action> layer_actions = new List<Action>();
        GetActionsInLayer(instance.components.action_properties.layer, ref layer_actions);

        foreach(Action action in layer_actions)
        {
            if (action.components.action_properties.priority > instance.components.action_properties.priority)
                return SuspendState(instance);
        }

        return NextState(instance);
    }
    /// <summary>
    /// return the natural next state for the action when is actually "updating" 
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    private ActionInstance.State NextState (Action instance)
    {
        if (instance.components.action_instance.state == ActionInstance.State.Queued) return ActionInstance.State.Start;
        if (instance.components.action_instance.state == ActionInstance.State.Start) return ActionInstance.State.Updating;
        if (instance.components.action_instance.state == ActionInstance.State.Suspended) return ActionInstance.State.Updating;
        if (instance.components.action_instance.state == ActionInstance.State.Updating)
        {
            if (instance.Goal())
            {
                return ActionInstance.State.Succeed;
            }
            if (instance.Fail())
            {
                return ActionInstance.State.Failed;
            }
        }
        return instance.components.action_instance.state;
    }
    private ActionInstance.State SuspendState (Action instance)
    {
        if (instance.components.action_instance.state != ActionInstance.State.Queued) return ActionInstance.State.Suspended;
        return instance.components.action_instance.state;
    }

    #region Utils

    protected void GetActionsInLayer (string layer, ref List<Action> layer_actions)
    {
        foreach (Action action in active_actions)
        {
            if (action.components.action_properties.layer == layer)
            {
                layer_actions.Add(action);
            }
        }
    }
    public bool IsCloned (string name, string layer)
    {
        List<Action> layer_actions = new List<Action>();
        GetActionsInLayer(layer, ref layer_actions);

        foreach (Action layer_action in layer_actions)
        {
            if (layer_action.components.action_properties.name == name) return true;
        }
        return false;
    }
    #endregion
}
