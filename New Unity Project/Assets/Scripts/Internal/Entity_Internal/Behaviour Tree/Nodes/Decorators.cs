using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
    public class Decorator : Node
    {
        public Node child;
        public Decorator(Node child)
        {
            this.child = child;
        }
    }

    public class Inverter : Decorator
    {
        public Inverter(Node child) : base(child) { }

        public override State Tick(Tick tick)
        {
            State state = child.Execute(tick);

            if (child == null) return State.Error;

            if (state == State.Success) return State.Failure;
            if (state == State.Failure) return State.Success;

            return state;
        }
    }

    public class TimeLimit : Decorator
    {
        private readonly float limit;

        public TimeLimit(float limit, Node child) : base(child) { this.limit = limit; }

        public override void Open(Tick tick)
        {
            tick.blackBoard.Set("startTime", Time.time, tick.tree, this);
        }
        public override State Tick(Tick tick)
        {
            State state = child.Execute(tick);

            float startTime = tick.blackBoard.Get<float>("startTime", tick.tree, this);
            if (Time.time - startTime > limit)
            {
                return State.Failure;
            }

            if (state != State.Running) return state;

            return state;
        }
    }
}
