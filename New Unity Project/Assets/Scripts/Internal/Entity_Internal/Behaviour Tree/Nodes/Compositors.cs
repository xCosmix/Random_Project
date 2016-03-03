using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
    public class Compositor : Node
    {
        protected Node[] children;

        public Compositor(Node[] children)
        {
            this.children = children;
        }
    }

    public class Sequence : Compositor
    {
        public Sequence(Node[] children) : base(children) { }

        public override State Tick(Tick tick)
        {
            for (int i = 0; i < children.Length; i++)
            {
                State state = children[i].Execute(tick);

                if (state != State.Success) return state;
            }

            return State.Success;
        }
    }

    public class Priority : Compositor
    {
        public Priority(Node[] children) : base(children) { }

        public override State Tick(Tick tick)
        {
            for (int i = 0; i < children.Length; i++)
            {
                State state = children[i].Execute(tick);

                if (state != State.Failure) return state;

            }

            return State.Failure;
        }
    }

    public class MemSequence : Compositor
    {
        public MemSequence(Node[] children) : base(children) { }

        public override void Open(Tick tick)
        {
            tick.blackBoard.Set("runningChild", 0, tick.tree, this);
        }
        public override State Tick(Tick tick)
        {
            int child = tick.blackBoard.Get<int>("runningChild", tick.tree, this);
            for (int i = child; i < children.Length; i++)
            {
                State state = children[i].Execute(tick);

                if (state != State.Success)
                {
                    if (state == State.Running) tick.blackBoard.Set("runningChild", i, tick.tree, this);

                    return state;
                }
            }

            return State.Success;
        }
    }

    public class MemPriority : Compositor
    {
        public MemPriority(Node[] children) : base(children) { }

        public override void Open(Tick tick)
        {
            tick.blackBoard.Set("runningChild", 0, tick.tree, this);
        }
        public override State Tick(Tick tick)
        {
            int child = tick.blackBoard.Get<int>("runningChild", tick.tree, this);
            for (int i = child; i < children.Length; i++)
            {
                State state = children[i].Execute(tick);

                if (state != State.Failure)
                {
                    if (state == State.Running) tick.blackBoard.Set("runningChild", i, tick.tree, this);

                    return state;
                }
            }

            return State.Failure;
        }
    }

    public class Probabilistic : Compositor
    {
        public Probabilistic(Node[] children) : base(children) { }

        public override State Tick(Tick tick)
        {
            int i = Random.Range(0, children.Length);
            State state = children[i].Execute(tick);

            return state;
        }
    }

    public class XProbabilistic : Compositor
    {
        public XProbabilistic(Node[] children) : base(children) { }

        public override void Open(Tick tick)
        {
            tick.blackBoard.Set("runningChild", -1, tick.tree, this);
        }
        public override State Tick(Tick tick)
        {
            int i;
            i = tick.blackBoard.Get<int>("runningChild", tick.tree, this);

            if (i == -1)
            {
                i = Random.Range(0, children.Length);
            }

            State state = children[i].Execute(tick);

            if (state == State.Running) tick.blackBoard.Set("runningChild", i, tick.tree, this);

            return state;
        }
    }
}
