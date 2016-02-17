using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Action<T> : System.Object {

    #region Definition

    private static T instance;

    public static T GetActionInstance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)System.Activator.CreateInstance(typeof(T), true);
            }
            return instance;
        }
    }

    protected struct Properties
    {
        public string name;
        public string layer;
        public uint priority;
        public bool queueable;
    }
    /// <summary>
    /// static properties of the action
    /// </summary>
    protected Properties properties;

    /// <summary>
    /// Is the reference to the current instance which is using the action (it change everytime a different instance use the action)
    /// </summary>
    protected Instance work_space;

    abstract public void Start();

    abstract public void Update();

    abstract public void Queued();

    abstract public void End();

    abstract public bool Goal();

    abstract public bool Fail();

    #endregion

    #region UserExternal

    public struct Instance
    {
        public enum State { Queued, Start, Updating, Failed, Succeed }

        public ActionController controller;
        public Action<T> action;
        public State state;

        public GameObject target_gameObject;
        public Dictionary<string, System.Object> resources;

        public float time;
        private float start_time;

        private void CallStep ()
        {
            action.Step(this);
        }
        private bool Ended ()
        {
            return state == State.Succeed || state == State.Failed;
        }
        private void StateChange ()
        {

        }

        private IEnumerator Update ()
        {
            while (!Ended())
            {
                State last_state = state;
                CallStep();
                if (last_state != state) StateChange();

                yield return new WaitForFixedUpdate();
            }
        }
    }

    #endregion

    #region Step

    public void Step(Instance instance)
    {
        work_space = instance;

        switch (instance.state)
        {
            case Instance.State.Queued:
                Queued();
                break;
            case Instance.State.Start:
                Start();
                break;
            case Instance.State.Updating:
                Update();
                break;
            case Instance.State.Failed:
                End();
                break;
            case Instance.State.Succeed:
                End();
                break;
        }

        instance.state = UpdateState();
    }

    #endregion

    #region Util 

    /// <summary>
    /// Unique method which is able to change the instance state
    /// </summary>
    /// <returns>next state</returns>
    private Instance.State UpdateState()
    {
        if (work_space.controller == null)
        {
            if (work_space.state == Instance.State.Queued) return Instance.State.Start;
            if (work_space.state == Instance.State.Start) return Instance.State.Updating;
            if (work_space.state == Instance.State.Updating)
            {
                if (Goal())
                {
                    return Instance.State.Succeed;
                }
                if (Fail())
                {
                    return Instance.State.Failed;
                }
            }
        }
        return work_space.controller.UpdateState(work_space);
    }

    #endregion
}

#region BaseAction

public class BaseAction : Action<BaseAction>
{
    public override void End()
    {
        throw new NotImplementedException();
    }

    public override bool Fail()
    {
        throw new NotImplementedException();
    }

    public override bool Goal()
    {
        throw new NotImplementedException();
    }

    public override void Queued()
    {
        throw new NotImplementedException();
    }

    public override void Start()
    {
        throw new NotImplementedException();
    }

    public override void Update()
    {
        throw new NotImplementedException();
    }
}

#endregion

public class ActionController : System.Object
{
    private List<BaseAction.Instance> active_actions = new List<BaseAction.Instance>();

    public void Suscribe (BaseAction.Instance instance)
    {

    }
    public Action<Action>.Instance.State UpdateState  (Action<Action>.Instance instance)
    {
        return 0;
    }
}
