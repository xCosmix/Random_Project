using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionController : MonoBehaviour {

    #region Definition

    private List<Action> updateActions = new List<Action>();
    private List<Action> fixedActions = new List<Action>();
    private List<int> removeActions = new List<int>();

    #endregion

    #region Subscribe

    /// <summary>
    /// subscribe to the events of action controller
    /// </summary>
    /// <param name="instance"></param>
    /// <returns>
    /// if the subscription succeeded return true
    /// </returns>
    public bool Subscribe(Action instance)
    {
        ///This is useless double check, because in the action before instantiating it, there is the same check
        //if (IsCloned(instance.name, instance.layer)) return false;
        if (instance.components.action_properties.updateType == ActionProperties.UpdateType.Update)
            updateActions.Add(instance);
        else
            fixedActions.Add(instance);

        return true;
    }

    #endregion

    #region Steps

    /// <summary>
    /// Main Update
    /// </summary>
    void Update ()
    {
        StartStep();
        ActiveActionsStep(updateActions);
        EndedActionsStep();
    }

    void FixedUpdate ()
    {
        StartStep();
        ActiveActionsStep(fixedActions);
        EndedActionsStep();
    }

    /// <summary>
    /// Start Step at every Update
    /// </summary>
    private void StartStep ()
    {
        removeActions.Clear();
    }

    /// <summary>
    /// Refresh active actions calls at every Update
    /// </summary>
    private void ActiveActionsStep (List<Action> update)
    {
        int index = 0;

        foreach (Action action in update)
        {
            ActionInstance.State last_state = action.components.action_instance.state;

            action.components.action_instance.CallStep();
            action.components.action_instance.UpdateState();

            ///Check if the action already ended (reached goal or failed) and remove it from curren actions updating
            if (last_state == ActionInstance.State.Failed || last_state == ActionInstance.State.Succeed || last_state == ActionInstance.State.Cancel)
            {
                removeActions.Add(index);
            }

            index++;
        }
    }

    /// <summary>
    /// Remove ended actions from updateActions list
    /// </summary>
    private void EndedActionsStep ()
    {
        foreach (int i in removeActions) updateActions.RemoveAt(i);
    }

    #endregion

    #region StateControl

    public ActionInstance.State UpdateState(Action instance)
    {
       
        List<Action> layer_actions = new List<Action>();
        GetActionsInLayer(instance.components.action_properties.layer, ref layer_actions);

        if (instance.components.action_properties.priority > 0)
        {
            foreach (Action action in layer_actions)
            {
                if (action.components.action_properties.priority > instance.components.action_properties.priority)
                    return SuspendState(instance);
            }
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

    #endregion

    #region Utils

    protected void GetActionsInLayer (string layer, ref List<Action> layer_actions)
    {
        foreach (Action action in updateActions)
        {
            if (action.components.action_properties.layer == layer)
            {
                layer_actions.Add(action);
            }
        }
    }
    public Action IsCloned (string name, string layer)
    {
        List<Action> layer_actions = new List<Action>();
        GetActionsInLayer(layer, ref layer_actions);

        foreach (Action layer_action in layer_actions)
        {
            if (layer_action.components.action_properties.name == name) return layer_action;
        }
        return null;
    }
    #endregion
}
