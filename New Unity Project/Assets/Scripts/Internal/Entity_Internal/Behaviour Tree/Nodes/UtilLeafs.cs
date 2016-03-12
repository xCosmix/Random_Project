using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
    public class Leaf : Node
    {

    }

    public class Wait : Leaf
    {
        private float endTime = 0.0f;

        public Wait(float endTime) { this.endTime = endTime; }

        public override void Open(Tick tick)
        {
            tick.blackBoard.Set("startTime", Time.time, tick.tree, this);
        }
        public override State Tick(Tick tick)
        {
            float startTime = tick.blackBoard.Get<float>("startTime", tick.tree, this);
            float currentTime = Time.time - startTime;

            if (currentTime >= endTime) return State.Success;

            return State.Running;
        }
    }

    public class Probability : Leaf
    {
        private int percent = 0;
        private float percistence = 1.0f;

        public Probability(int percent, float percistence) { this.percent = percent; this.percistence = percistence; }

        public override State Tick(Tick tick)
        {
            float startTime = tick.blackBoard.Get<float>("changeTime", tick.tree, this);
            int value = tick.blackBoard.Get<int>("lastValue", tick.tree, this);

            if (Time.time - startTime > percistence)
            {
                value = Random.Range(0, 101);
                tick.blackBoard.Set("lastValue", value, tick.tree, this);
                tick.blackBoard.Set("changeTime", Time.time, tick.tree, this);
            }

            if (value <= percent) return State.Success;

            return State.Failure;
        }
    }

    public class WaitRandom : Leaf
    {
        private float min = 0.0f;
        private float max = 0.0f;

        public WaitRandom(float min, float max) { this.min = min; this.max = max; }

        public override void Open(Tick tick)
        {
            float endTime = Random.Range(min, max);
            tick.blackBoard.Set("startTime", Time.time, tick.tree, this);
            tick.blackBoard.Set("endTime", endTime, tick.tree, this);
        }
        public override State Tick(Tick tick)
        {
            float startTime = tick.blackBoard.Get<float>("startTime", tick.tree, this);
            float endTime = tick.blackBoard.Get<float>("endTime", tick.tree, this);
            float currentTime = Time.time - startTime;

            if (currentTime >= endTime) return State.Success;

            return State.Running;
        }
    }

    public class ChangeColor : Leaf
    {
        private Color color;

        public ChangeColor(Color color) { this.color = color; }

        public override State Tick(Tick tick)
        {
            SpriteRenderer renderer = tick.blackBoard.agent.GetComponent<SpriteRenderer>();
            if (renderer == null) renderer = tick.blackBoard.agent.GetComponentInChildren<SpriteRenderer>();

            if (renderer == null) return State.Error;

            renderer.color = color;
            return State.Success;
        }
    }

    public class ChangePosition : Leaf
    {
        private Vector3 min;
        private Vector3 max;

        public ChangePosition(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        public override State Tick(Tick tick)
        {
            Transform transform = tick.blackBoard.agent.transform;

            float randomX = Random.Range(min.x, max.x);
            float randomY = Random.Range(min.y, max.y);
            float randomZ = Random.Range(min.z, max.z);
            transform.position = new Vector3(randomX, randomY, randomZ);

            return State.Success;
        }
    }

    public class ButtonPressed : Leaf
    {
        private string button;

        public ButtonPressed(string button)
        {
            this.button = button;
        }

        public override State Tick(Tick tick)
        {
            if (Input.GetButton(button)) return State.Success;

            return State.Failure;
        }
    }

    public class XAction <T> : Leaf where T : Action
    {
        private object[] parameters;

        public XAction(params object[] parameters)
        {
            this.parameters = parameters;
        }
        public override void Open(Tick tick)
        {
            Action action = Action.Create<T>(tick.blackBoard.agent, parameters);
            tick.blackBoard.Set("Action", action, tick.tree, this);
        }

        public override State Tick(Tick tick)
        {
            Action action = tick.blackBoard.Get<Action>("Action", tick.tree, this);
            ActionInstance actionInstance = action.components.action_instance;

            State state = State.Running;

            if (actionInstance.state == ActionInstance.State.Failed) state = State.Failure;
            if (actionInstance.state == ActionInstance.State.Succeed) state = State.Success;

            return state;
        }

        public override void Close(Tick tick)
        {
            Action action = tick.blackBoard.Get<Action>("Action", tick.tree, this);
            ActionInstance actionInstance = action.components.action_instance;

            ///Cancel action if it isnt already ended
            if (actionInstance.state != ActionInstance.State.Failed && actionInstance.state != ActionInstance.State.Succeed)
                action.components.action_instance.Cancel();
        }
    }
}