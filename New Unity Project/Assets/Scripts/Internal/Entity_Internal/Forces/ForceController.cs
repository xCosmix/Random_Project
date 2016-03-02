using UnityEngine;
using System.Collections.Generic;

namespace Forces
{
    /// <summary>
    /// Controls forces to be esay for the user to use
    /// </summary>
    public class ForceController : System.Object
    {
        protected List<Force> active_forces = new List<Force>();
        protected List<Force2Call> call_forces = new List<Force2Call>();
        protected List<int> null_forces_index = new List<int>();
        protected CharacterController controller;

        public ForceController(CharacterController controller)
        {
            this.controller = controller;
        }

        public void AddForce(ForceProps forceProps, Vector3 direction)
        {
            Force target = Target(forceProps, direction);
            call_forces.Add(new Force2Call(direction, target));
        }

        public Force GetForce(ForceProps force)
        {
            foreach (Force f in active_forces)
            {
                if (f.properties.Name == force.Name) return f;
            }
            return null;
        }

        public void Actualize()
        {
            Vector3 final_velocity = Vector3.zero;

            ///ADD AND CALL STEP
            foreach (Force2Call f in call_forces)
            {
                f.force.Call(f.direction);
            }

            ///APPLY STEP
            for (int i = 0; i < active_forces.Count; i++)
            {
                Force f = active_forces[i];

                Vector3 add = f.Velocity();
                final_velocity += add;

                ///Add to null forces if the force isnt using
                if (!f.Using())
                    null_forces_index.Add(i);
            }

            ///MOVE STEP
            controller.Move(final_velocity * Time.deltaTime);

            ///CLEAR STEP
            foreach (int i in null_forces_index)
            {
                active_forces.RemoveAt(i);
            }

            call_forces.Clear();
            null_forces_index.Clear();
        }

        protected Force Target(ForceProps force, Vector3 direction)
        {
            foreach (Force f in active_forces)
            {
                if (f.properties.Name == force.Name)
                {
                    return f;
                }
            }
            Force out_ = new Force(force, direction);
            active_forces.Add(out_);

            return out_;
        }
    }
}
