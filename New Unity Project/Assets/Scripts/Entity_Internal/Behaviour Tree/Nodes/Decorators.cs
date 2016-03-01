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
            State state = child.Tick(tick);

            if (child == null) return State.Error;

            if (state == State.Success) return State.Failure;
            if (state == State.Failure) return State.Success;

            return state;
        }
    }
}
