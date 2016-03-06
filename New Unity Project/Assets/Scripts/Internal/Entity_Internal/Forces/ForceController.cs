using UnityEngine;
using System.Collections.Generic;

namespace Forces
{
    /// <summary>
    /// Controls forces to be esay for the user to use
    /// </summary>
    public class ForceController : System.Object
    {
        protected Dictionary<string, Force> active_forces = new Dictionary<string, Force>();
        protected List<Force2Call> call_forces = new List<Force2Call>();
        protected List<string> null_forces_index = new List<string>();

        protected Rigidbody2D rigidBody;

        public ForceController(Rigidbody2D rigidBody)
        {
            this.rigidBody = rigidBody;
        }

        public void AddForce(ForceProps forceProps, Vector3 direction)
        {
            Force target = Target(forceProps, direction);
            call_forces.Add(new Force2Call(direction, target));
        }

        public Force GetForce(ForceProps force)
        {
            if (active_forces.ContainsKey(force.Name))
                return active_forces[force.Name];
            return null;
        }

        public void Actualize()
        {
            Vector2 final_velocity = Vector2.zero;

            ///ADD AND CALL STEP
            foreach (Force2Call f in call_forces)
            {
                f.force.Call(f.direction);
            }

            ///APPLY STEP
            foreach (string key in active_forces.Keys)
            {
                Force f = active_forces[key];

                Vector2 add = f.Velocity();
                final_velocity += add;

                ///Add to null forces if the force isnt using
                if (!f.Using())
                    null_forces_index.Add(f.properties.Name);
            }

            ///MOVE STEP
            rigidBody.velocity = final_velocity * Time.deltaTime * 100.0f;

            ///CLEAR STEP
            foreach (string i in null_forces_index)
            {
                active_forces.Remove(i);
            }

            call_forces.Clear();
            null_forces_index.Clear();
        }

        protected Force Target(ForceProps force, Vector3 direction)
        {
            Force out_ = GetForce(force);

            if (out_ == null)
            {
                out_ = new Force(force, direction);
                active_forces.Add(out_.properties.Name, out_);
            }

            return out_;
        }
    }
}
