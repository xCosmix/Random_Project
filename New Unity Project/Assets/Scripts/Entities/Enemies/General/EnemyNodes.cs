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

    public class TargetOnSight : Leaf
    {
        private const float lookForPlayerTime = 5.0f;

        private float fieldOfView;

        public TargetOnSight(float fieldOfView)
        {
            this.fieldOfView = fieldOfView;
        }
        public override State Tick(Tick tick)
        {
            float lastSeenTime = tick.blackBoard.Get<float>("lastSeenTime", tick.tree, this);
            float time = Time.time - lastSeenTime;

            Player p = Player.player;
            Vector2 pPosition = p.transform.position;
            Vector2 myPosition = tick.blackBoard.agent.transform.position;

            Vector2 delta = pPosition - myPosition;
            bool lookingFor = time < lookForPlayerTime;

            if (delta.sqrMagnitude <= fieldOfView || lookingFor)
            {
                if (!Physics2D.Linecast(myPosition, pPosition, 256) || lookingFor)
                {
                    if (!lookingFor)
                        tick.blackBoard.Set("lastSeenTime", Time.time, tick.tree, this);

                    return State.Success;
                }
            }

            return State.Failure;
        }
    }
}
