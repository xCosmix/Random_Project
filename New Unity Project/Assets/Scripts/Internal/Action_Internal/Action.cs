using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class ActionProperties : Attribute
{
    public enum UpdateType { Update, FixedUpdate }
    public readonly string name;
    public readonly string layer;
    public readonly uint priority;
    public readonly bool queueable;
    public readonly UpdateType updateType;

    public ActionProperties(string name, string layer, uint priority, bool queueable, UpdateType updateType = UpdateType.Update)
    {
        this.name = name;
        this.layer = layer;
        this.priority = priority;
        this.queueable = queueable;
        this.updateType = updateType;
    }
}

public interface ActionInterface
{
    #region StaticStruct

    void Start();
    void Update();
    void Queued();
    void Suspended();
    void End();

    bool Goal();
    bool Fail();

    #endregion
}

public sealed class ActionComponents
{
    public ActionInterface action_interface;
    public ActionProperties action_properties;
    public ActionInstance action_instance;
    public ActionController action_controller;
    public GameObject action_target;
}

public sealed class ActionInstance : System.Object
{

    #region Definition

    public enum State
    {
        Queued, Start, Updating, Suspended, Failed, Succeed, Cancel
    }

    public Action action;

    public State state;
    public float time;
    //private float start_time;

    public ActionInstance(Action action)
    {
        this.action = action;
    }

    #endregion

    #region Steps

    public bool Subscribe()
    {
        state = State.Queued;
        //start_time = Time.time;
        time = 0.0f;

        return action.components.action_controller.Subscribe(action);
    }
    public void CallStep()
    {
        Step();
    }
    public void UpdateState()
    {
        state = UpdateStateInternal();
    }
    public void Cancel ()
    {
        state = State.Cancel;
    }

    private void Step()
    {
        switch (state)
        {
            case State.Queued:
                action.components.action_interface.Queued();
                break;
            case State.Start:
                action.components.action_interface.Start();
                break;
            case State.Updating:
                action.components.action_interface.Update();
                break;
            case State.Suspended:
                action.components.action_interface.Suspended();
                break;
            case State.Failed:
                action.components.action_interface.End();
                break;
            case State.Succeed:
                action.components.action_interface.End();
                break;
            case State.Cancel:
                break;
        }
    }
    private State UpdateStateInternal()
    {
        ///If an action enters on state cancel, it cannot be changed, the action will be removed at the next update of Action
        /// Controller at evaluation of state.
        if (state == State.Cancel) return state;
        return action.components.action_controller.UpdateState(action);
    }

    #endregion
}

[ActionProperties("", "", 0, false)]
public abstract class Action : ActionInterface
{
    #region Definition

    public static T Create<T> (GameObject target, object[] parameters = null) where T : Action
    {
        ActionController controller = GetController(target);
        Action alreadyExists = IsCloned<T>(controller);
        if (alreadyExists != null)
        {
            Debug.Log("this action already exists");
            return (T)alreadyExists;
        }

        T instance = Activator.CreateInstance(typeof(T), parameters) as T;
        instance.SetComponents(target);
        instance.Subscribe();

        return instance;
    }
    protected void SetComponents (GameObject target)
    {
        components = new ActionComponents();

        components.action_interface = this;
        components.action_target = target;
        components.action_properties = GetProperties();
        components.action_controller = GetController();
        components.action_instance = new ActionInstance(this);
    }
    /// <summary>
    /// Subscribe to controller, if it is not Subscribed correctly, the action will not execute
    /// </summary>
    protected void Subscribe ()
    {
        bool succeeded = components.action_instance.Subscribe();
        if (!succeeded) Debug.LogWarning("Action: " + components.action_properties.name + "Couldn't subscribe to the controller of: " + components.action_target.name);
    }

    public abstract void Start();
    public abstract void Update();
    public abstract void Queued();
    public abstract void Suspended();
    public abstract void End();
    public abstract bool Goal();
    public abstract bool Fail();

    public ActionComponents components;

    #endregion

    #region Utils

    protected ActionController GetController()
    {
        ActionController action_controller = components.action_controller;
        if (action_controller == null)
        {
            action_controller = components.action_target.GetComponent<ActionController>();
            if (action_controller == null)
            {
                action_controller = components.action_target.AddComponent<ActionController>();
            }
        }
        return action_controller;
    }
    protected static ActionController GetController(GameObject target, ActionController controller = null)
    {
        ActionController action_controller = controller;
        if (action_controller == null)
        {
            action_controller = target.GetComponent<ActionController>();
            if (action_controller == null)
            {
                action_controller = target.AddComponent<ActionController>();
            }
        }
        return action_controller;
    }
    protected ActionProperties GetProperties()
    {
        /*
        Type t = this.GetType();
        MethodInfo method_info = this.GetType().GetMethod("GetProperties_Util");
        MethodInfo generic_method = method_info.MakeGenericMethod(new Type[] { t });
        var return_value = generic_method.Invoke(null, new object[] { });
        */
        ActionProperties props = (ActionProperties)Attribute.GetCustomAttribute(GetType(), typeof(ActionProperties));
        return props;
    }
    protected static ActionProperties GetProperties_Util<T> () where T : Action
    {
        ActionProperties props = (ActionProperties)Attribute.GetCustomAttribute(typeof(T), typeof(ActionProperties));
        return props;
    }
    protected static Action IsCloned<T> (ActionController controller) where T : Action
    {
        ActionProperties props = GetProperties_Util<T>();
        return controller.IsCloned(props.name, props.layer);
    }
    #endregion
}
