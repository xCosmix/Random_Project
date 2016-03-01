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

    public class ChangeColor : Leaf
    {
        private Color color;

        public ChangeColor(Color color) { this.color = color; }

        public override State Tick(Tick tick)
        {
            MeshRenderer renderer = tick.blackBoard.agent.GetComponent<MeshRenderer>();

            if (renderer == null) return State.Error;

            renderer.material.color = color;
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
}