using UnityEngine;
using Pathfinding;
using Forces;

namespace EnemyActions
{
    [ActionProperties("Random", "Movement", 1, false)]
    class RandomMovement : Action
    {
        public RandomMovement (float radius)
        {
            this.radius = radius;
        }
        float radius;

        Entity me;
        Vector2[] path;
        Vector2 currentGoal;
        int currentPoint = 0;
        float goalDistance = 0.0f;

        public override void End() {}
        public override void Suspended() { }
        public override void Queued() { }
        public override void Start()
        {
            me = components.action_target.GetComponent<Entity>();
            Vector2 myPosition = me.transform.position;
            Vector2 goal = myPosition;
            goal.x += Random.Range(-radius, radius);
            goal.y += Random.Range(-radius, radius);

            path = Path.FindPath(myPosition, goal);
            currentPoint = 0;
            goalDistance = Grid.GetNodeSize() * 0.5f;

            currentGoal = path[currentPoint];
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

            Vector2 myPosition = me.transform.position;

            if ((currentGoal - myPosition).magnitude < goalDistance)
            {
                currentPoint++;
                currentPoint = Mathf.Clamp(currentPoint, 0, path.Length - 1);
                currentGoal = path[currentPoint];
            }

            //Debug.DrawLine(currentGoal, currentGoal + Vector2.up * 10.0f, Color.red);

            Vector2 moveDir = currentGoal - (Vector2)me.transform.position;
            me.RigidBody.AddForce(moveDir * me.characterController.speed);
        }
    }

    [ActionProperties("Aim", "Input", 0, false)]
    class Aim : Action
    {
        /// <summary>
        /// Aim to player or target
        /// </summary>
        public Aim(float accuaracy)
        {
            this.accuaracy = accuaracy;
        }

        float accuaracy;
        Enemy me;

        public override void End() { }
        public override void Suspended() { }
        public override void Queued() { }
        public override void Start()
        {
            if (Player.player == null) return;

            me = components.action_target.GetComponent<Enemy>();
            Vector2 myPos = me.transform.position;
            Vector2 targetPos = Player.player.transform.position;
            Vector2 perfectShot = (targetPos - myPos).normalized;

            float percent = 1.0f - accuaracy;
            float angle = Random.Range(90.0f * -percent, 90 * percent);
            Vector2 realShot = Quaternion.AngleAxis(angle, Vector3.forward) * perfectShot;
            me.AimDir = realShot;
        }
        public override bool Fail() { return false; }
        public override bool Goal() { return true; }

        public override void Update() { }
    }

    [ActionProperties("Shoot", "Attack", 1, false)]
    class Shoot : Action
    {
        Enemy me;

        public override void End() { }
        public override void Suspended() { }
        public override void Queued() { }
        public override void Start()
        {
            me = components.action_target.GetComponent<Enemy>();
            me.weapon.Trigger(me.AimDir);
        }
        public override bool Fail() { return false; }
        public override bool Goal() { return true; }

        public override void Update() { }
    }
}
