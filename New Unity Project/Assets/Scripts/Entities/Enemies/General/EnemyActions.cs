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

        public override void End() { me.Animator.SetBool("run", false); }
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
        public override bool Goal() { return currentPoint >= path.Length; }

        public override void Update()
        {
            ///GENERIC ANIMATION 
            me.Animator.SetBool("run", true);

            ///move
            Vector2 myPosition = me.transform.position;
            if ((currentGoal - myPosition).magnitude < goalDistance)
            {
                currentPoint++;
                if (currentPoint >= path.Length) return;

                currentGoal = path[currentPoint];
            }

            Vector2 moveDir = currentGoal - (Vector2)me.transform.position;
            me.RigidBody.AddForce(moveDir.normalized * me.characterController.Speed);
        }
    }

    [ActionProperties("Cast", "Attack", 0, false)]
    class CastAttack : Action
    {
        Enemy me;
        float startTime;
        GameObject warning;

        static GameObject warningPrefab;

        public override void End() { ObjectPool.Pool.Destroy(warning); }
        public override void Suspended() { }
        public override void Queued() { }
        public override void Start()
        {
            if (warningPrefab == null)
            {
                warningPrefab = Resources.Load<GameObject>("Prefabs/WarningSign");
            }
            me = components.action_target.GetComponent<Enemy>();

            startTime = Time.time;

            warning = ObjectPool.Pool.New(warningPrefab, (Vector2)me.transform.position + (Vector2.up * me.transform.localScale.y * 0.5f), warningPrefab.transform.rotation, 10);
            //warning.transform.SetParent(me.transform);
        }
        public override bool Fail() { return false; }
        public override bool Goal() { return Time.time - startTime > me.CastTime; }

        public override void Update() { }
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

    [ActionProperties("Aproach", "Movement", 1, false)]
    class Aproach : Action
    {
        public Aproach(float minRadius, float maxRadius, float minAngle, float maxAngle)
        {
            this.target = Player.player.transform.position;
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
            this.minAngle = minAngle;
            this.maxAngle = maxAngle;
        }

        float minRadius;
        float maxRadius;
        float minAngle;
        float maxAngle;
        Vector2 target;

        Entity me;
        Vector2[] path;
        Vector2 currentGoal;
        int currentPoint = 0;
        float goalDistance = 0.0f;

        public override void End() { me.Animator.SetBool("run", false); }
        public override void Suspended() { }
        public override void Queued() { }
        public override void Start()
        {
            me = components.action_target.GetComponent<Entity>();

            Vector2 goal = Target();
            path = Path.FindPath(me.transform.position, goal);

            currentPoint = 0;
            goalDistance = Grid.GetNodeSize() * 0.5f;

            currentGoal = path[currentPoint];
        }

        public override bool Fail() { return path.Length <= 1; }
        public override bool Goal() { return currentPoint >= path.Length; }

        public override void Update()
        {
            ///GENERIC ANIMATION 
            me.Animator.SetBool("run", true);

            ///move
            Vector2 myPosition = me.transform.position;
            if ((currentGoal - myPosition).magnitude < goalDistance)
            {
                currentPoint++;
                if (currentPoint >= path.Length) return;

                currentGoal = path[currentPoint];
            }

            Vector2 moveDir = currentGoal - (Vector2)me.transform.position;
            me.RigidBody.AddForce(moveDir.normalized * me.characterController.Speed);
        }

        private Vector2 Target ()
        {
            Vector2 myPosition = me.transform.position;
            Vector2 dir = myPosition - target;
            float radius = Random.Range(minRadius, maxRadius);
            float angle = Random.Range(minAngle, maxAngle);
            Vector2 finalDir = Quaternion.AngleAxis(angle, Vector3.forward) * dir.normalized;

            return target + finalDir * radius;
        }
    }

    [ActionProperties("Follow", "Movement", 1, false)]
    class Follow : Action
    {
        public Follow(float maxTime)
        {
            this.maxTime = maxTime;
        }

        float maxTime;

        Entity me;
        Vector2[] path;
        Vector2 currentGoal;
        int currentPoint = 0;
        float goalDistance = 0.0f;

        float startTime = 0.0f;

        public override void End() { me.Animator.SetBool("run", false); }
        public override void Suspended() { }
        public override void Queued() { }
        public override void Start()
        {
            me = components.action_target.GetComponent<Entity>();
            startTime = Time.time;

            UpdateFollow();
        }

        public override bool Fail() { return path.Length <= 1 || Time.time - startTime > maxTime; }
        public override bool Goal() { return currentPoint >= path.Length; }

        public override void Update()
        {
            ///GENERIC ANIMATION 
            me.Animator.SetBool("run", true);

            ///evaluate if have to repath
            if (NeedToRepath())
                UpdateFollow();

            ///move
            Vector2 myPosition = me.transform.position;
            if ((currentGoal - myPosition).magnitude < goalDistance)
            {
                currentPoint++;
                if (currentPoint >= path.Length) return;

                currentGoal = path[currentPoint];
            }

            Vector2 moveDir = currentGoal - (Vector2)me.transform.position;
            me.AimDir = moveDir.normalized;

            me.RigidBody.AddForce(moveDir.normalized * me.characterController.Speed);
        }
        private bool NeedToRepath ()
        {
            Vector2 playerPos = Player.player.transform.position;

            return (playerPos - path[path.Length - 1]).sqrMagnitude > 6.0f;
        }
        private void UpdateFollow ()
        {
            Vector2 goal = Player.player.transform.position;
            path = Path.FindPath(me.transform.position, goal);

            currentPoint = 0;
            goalDistance = Grid.GetNodeSize() * 0.5f;

            currentGoal = path[currentPoint];
        }
    }
}
