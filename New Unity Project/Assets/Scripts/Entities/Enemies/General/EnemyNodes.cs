using UnityEngine;
using System.Collections;
using BehaviourTree;

namespace EnemyNodes
{
    public class WillShoot : Leaf
    {
        private float minWaitTime;
        private float maxWaitTime;
        private int shootProbability;

        public WillShoot(float minWaitTime, float maxWaitTime, int shootProbability)
        {
            this.minWaitTime = minWaitTime;
            this.maxWaitTime = maxWaitTime;
            this.shootProbability = shootProbability;
        }

        public override State Tick(Tick tick)
        {
            float lastAproveTime = tick.blackBoard.Get<float>("lastAproveTime", tick.tree, this);
            int randomPercent = Random.Range(0, 101);
            float time = Time.time - lastAproveTime;

            if ((time > minWaitTime && randomPercent < shootProbability) || time >= maxWaitTime)
            {
                tick.blackBoard.Set("lastAproveTime", Time.time, tick.tree, this);
                return State.Success;
            }

            return State.Failure;
        }
    }
}
