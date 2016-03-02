using UnityEngine;
using System.Collections;

namespace BehaviourTree
{
    public class Node
    {
        public State Execute(Tick tick)
        {
            _Enter(tick);

            if (!tick.blackBoard.Get<bool>("isOpen", tick.tree, this))
                _Open(tick);

            State state = _Tick(tick);

            if (state != State.Running)
                _Close(tick);

            _Exit(tick);

            return state;
        }

        public void _Enter(Tick tick)
        {
            tick.EnterNode(this);
            Enter(tick);
        }
        public void _Open(Tick tick)
        {
            tick.OpenNode(this);
            tick.blackBoard.Set("isOpen", true, tick.tree, this);
            Open(tick);
        }
        public State _Tick(Tick tick)
        {
            tick.TickNode(this);
            return Tick(tick);
        }
        public void _Close(Tick tick)
        {
            tick.CloseNode(this);
            tick.blackBoard.Set("isOpen", false, tick.tree, this);
            Close(tick);
        }
        public void _Exit(Tick tick)
        {
            tick.ExitNode(this);
            Exit(tick);
        }

        /* override for behaviour */

        public virtual void Enter(Tick tick) { }
        public virtual void Open(Tick tick) { }
        public virtual State Tick(Tick tick) { return State.Error; }
        public virtual void Close(Tick tick) { }
        public virtual void Exit(Tick tick) { }
    }
}
