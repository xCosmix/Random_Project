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
            me.Move(moveDir);
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
            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
            this.minAngle = minAngle;
            this.maxAngle = maxAngle;
        }

        Entity me;

        readonly float minRadius;
        readonly float maxRadius;
        readonly float minAngle;
        readonly float maxAngle;

        Vector2[] path;
        Vector2 currentGoal;
        int currentPoint = 0;
        float goalDistance = 0.0f;

        public override void End() { me.Animator.SetBool("run", false); }
        
        public override void Start()
        {
            me = components.action_target.GetComponent<Entity>();

            currentPoint = 0;
            goalDistance = Grid.GetNodeSize() * 0.5f;

            UpdateFollow();          
        }

        public override bool Fail() { return path.Length <= 1; }
        public override bool Goal() { return currentPoint >= path.Length; }

        public override void Update()
        {
            //GENERIC ANIMATION 
            me.Animator.SetBool("run", true);
            //Check if target is way too far from last recorded target
            if (NeedToRepath())
                UpdateFollow();
            //Get Target
            GetNextTarget();
            //Move
            Move();
        }

        private bool NeedToRepath()
        {
            Vector2 playerPos = Player.player.transform.position;

            return (playerPos - path[path.Length - 1]).sqrMagnitude > 6.0f;
        }

        private void UpdateFollow()
        {
            Vector2 error = GetError();
            Vector2 goal = (Vector2)Player.player.transform.position + error;

            path = Path.FindPath(me.transform.position, goal);

            currentPoint = 0;
            goalDistance = Grid.GetNodeSize() * 0.5f;

            currentGoal = path[currentPoint];
        }

        private Vector2 GetError()
        {
            Vector2 target = Player.player.transform.position;
            Vector2 myPosition = me.transform.position;
            Vector2 dir = myPosition - target;
            float radius = Random.Range(minRadius, maxRadius);
            float angle = Random.Range(minAngle, maxAngle);
            Vector2 output = Quaternion.AngleAxis(angle, Vector3.forward) * dir.normalized * radius;

            return output;
        }

        private void GetNextTarget ()
        {
            Vector2 myPosition = me.transform.position;
            if ((currentGoal - myPosition).magnitude < goalDistance)
            {
                currentPoint++;
                if (currentPoint >= path.Length) return;

                currentGoal = path[currentPoint];
            }
        }

        private void Move ()
        {
            Vector2 moveDir = currentGoal - (Vector2)me.transform.position;
            me.Move(moveDir);
        }

        public override void Suspended() { }
        public override void Queued() { }
    }

    [ActionProperties("Follow", "Movement", 1, false)]
    class Follow : Action
    {
        Entity me;

        Vector2[] path;
        Vector2 currentGoal;
        int currentPoint = 0;
        float goalDistance = 0.0f;

        public override void End() { me.Animator.SetBool("run", false); }
        
        public override void Start()
        {
            me = components.action_target.GetComponent<Entity>();
            UpdateFollow();
        }

        public override bool Fail() { return path.Length <= 1; }
        public override bool Goal() { return currentPoint >= path.Length; }

        public override void Update()
        {
            //GENERIC ANIMATION 
            me.Animator.SetBool("run", true);
            //Check if target is way too far from last recorded target
            if (NeedToRepath())
                UpdateFollow();
            //Get Target
            GetNextTarget();
            //Move
            Move();
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

        private void GetNextTarget ()
        {
            Vector2 myPosition = me.transform.position;
            if ((currentGoal - myPosition).magnitude < goalDistance)
            {
                currentPoint++;
                if (currentPoint >= path.Length) return;

                currentGoal = path[currentPoint];
            }
        }

        private void Move ()
        {
            Vector2 moveDir = currentGoal - (Vector2)me.transform.position;
            me.AimDir = moveDir.normalized;
            me.Move(moveDir);
        }

        public override void Suspended() { }
        public override void Queued() { }
    }

    [ActionProperties("Dodge", "Movement", 2, false)]
    public class Dodge : Action
    {
        float speed = 0.0f;
        float fieldOfReaction = 10.0f;
        float duration = 1.0f;

        private float currentTime = 0.0f;

        public Dodge(float speed, float fieldOfReaction = 10.0f, float duration = 1.0f)
        {
            this.speed = speed;
            this.fieldOfReaction = fieldOfReaction;
            this.duration = duration;
        }

        public override void Start()
        {
            currentTime = 0.0f;
            Enemy enemy = components.action_target.GetComponent<Enemy>();
            Collider2D[] hazards = Physics2D.OverlapCircleAll(enemy.transform.position, fieldOfReaction, 4096);

            for (int i = 0; i < hazards.Length; i++)
            {
                Vector3 hazardPosition = hazards[i].transform.position;
                Vector3 delta = hazardPosition - enemy.transform.position;

                Vector3 optionA = Vector3.Cross(Vector3.forward, delta); optionA = optionA.normalized;
                Vector3 optionB = Vector3.Reflect(optionA, delta); optionB = optionB.normalized;
                Vector3 optionC = delta * -1; optionC = optionC.normalized;

                Vector2[] options = new Vector2[] { optionA, optionB, optionC };

                for (int c = 0; c < options.Length; c++)
                {
                    Ray ray = new Ray(enemy.transform.position, options[c].normalized);
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray, speed, 256);
                    if (hit)
                    {
                        if (hit.distance < speed * 0.8f) continue;

                        enemy.Impulse(options[c].normalized, speed);
                    }
                    else
                    {
                        enemy.Impulse(options[c].normalized, speed);
                    }
                }
            }
        }

        public override bool Goal() { return currentTime >= Time.deltaTime; }
        public override bool Fail() { return false; }

        public override void Update() { currentTime += Time.deltaTime; }

        public override void Queued() { }
        public override void Suspended() { }
        public override void End() { }
    }
}
