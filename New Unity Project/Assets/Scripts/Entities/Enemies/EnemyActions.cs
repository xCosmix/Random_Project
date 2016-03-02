using UnityEngine;
using Pathfinding;
using Forces;

namespace EnemyActions
{
    [ActionProperties("Random", "Movement", 1, false)]
    class RandomMovement : Action
    {
        /// <summary>
        /// SOLVE THIS PARAMETERS SHIT
        /// </summary>
        float radius = 5.0f;
        ForceProps force = new ForceProps("randomTEST", 10.0f, 0.2f, 0.2f);

        Entity me;
        Vector3[] path;
        Vector3 currentGoal;
        int currentPoint = 0;
        float goalDistance = 0.0f;

        public override void End() { Debug.Log("wi"); }
        public override void Suspended() { }
        public override void Queued() { }
        public override void Start()
        {
            me = components.action_target.GetComponent<Entity>();
            Vector3 myPosition = me.transform.position;
            Vector3 goal = myPosition;
            goal.x += Random.Range(-radius, radius);
            goal.z += Random.Range(-radius, radius);

            path = Path.FindPath(myPosition, goal);
            currentPoint = 0;
            goalDistance = Grid.GetNodeSize() * 0.5f;

            currentGoal = path[currentPoint];
            currentGoal.y = myPosition.y;
        }

        public override bool Fail() { return path.Length <= 1; }
        public override bool Goal() { return currentPoint >= path.Length-1; }

        public override void Update()
        {
            /*
            for (int i = 1; i < path.Length; i++)
            {
                Debug.DrawLine(path[i - 1], path[i]);
            }
            */

            Vector3 myPosition = me.transform.position;

            if ((currentGoal - myPosition).magnitude < goalDistance)
            {
                currentPoint++;
                currentPoint = Mathf.Clamp(currentPoint, 0, path.Length - 1);
                currentGoal = path[currentPoint];
                currentGoal.y = myPosition.y;
                Debug.Log(currentPoint);
            }

            //Debug.DrawLine(currentGoal, currentGoal + Vector3.up * 10.0f, Color.red);

            Vector3 moveDir = currentGoal - me.transform.position;
            me.forceController().AddForce(force, moveDir);
        }
    }
}
