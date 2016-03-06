using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Forces
{
    public class Force : System.Object
    {

        public ForceProps properties;

        protected Vector2 goal_vector = Vector2.up;
        protected Vector2 last_velocity = Vector2.zero;
        protected Vector2 init_in = Vector2.zero;
        protected Vector2 init_out = Vector2.zero;
        protected Vector2 current_output = Vector2.zero;
        protected float init_time = 0.0f;

        /// <summary>
        /// last call from velocity method
        /// </summary>
        protected float last_step_time = 0.0f;
        /// <summary>
        /// last call from user (like AddForce) to now when start fading out
        /// </summary>
        protected float last_call_time = 0.0f;

        protected float current_delta_step = 0.0f;
        protected float current_delta_call = 0.0f;
        protected float current_delta_notCall = 0.0f;

        /// <summary>
        /// time since init calling
        /// </summary>
        protected float time_calling = 0.0f;
        /// <summary>
        /// absolute time since init
        /// </summary>
        protected float time_stepping = 0.0f;
        /// <summary>
        /// time since left calling
        /// </summary>
        protected float time_notCalling = 0.0f;

        public Force(string id, Vector2 target_direction, float target_speed, float inertia_in = 1.0f, float inertia_out = 1.0f, float impulse = 0.0f)
        {
            properties = new ForceProps(id, target_speed, inertia_in, inertia_out, impulse);

            Call(target_direction);
        }
        public Force(ForceProps props, Vector2 target_direction)
        {
            properties = props;

            Call(target_direction);
        }
        public Vector2 Velocity()
        {
            Vector2 delta = Vector2.zero;
            Vector2 current_target_velocity = Vector2.zero;

            if (init_time == 0.0f) Initialize();

            StartStep();

            current_target_velocity = FadeIn() + FadeOut();
            delta = current_target_velocity;
            current_output = Clamp(last_velocity + delta, goal_vector);

            EndStep();

            return current_output;
        }
        /// <summary>
        /// called by user to add force
        /// </summary>
        public void Call(Vector2 new_goal_vector)
        {
            if (properties.forceMode == ForceMode.Impulse)
                Initialize();

            last_call_time = Time.time;
            goal_vector = new_goal_vector.normalized * properties.target_speed;
        }
        /// <summary>
        /// Determines if the force is being used after reaching zero force
        /// </summary>
        /// <returns></returns>
        public bool Using()
        {
            //Wait half a second to check if the velocity is null
            return (Time.time - last_call_time) < 0.5f || last_velocity != Vector2.zero;
        }
        /// <summary>
        /// Returns the value of the last call
        /// </summary>
        /// <returns></returns>
        public Vector2 LastVelocity()
        {
            return last_velocity;
        }

        protected void Initialize()
        {
            init_time = Time.time;
        }

        protected void StartStep()
        {
            ///Is the force acting like an impulse? if it is, then stop calling if impulse time < currentTime
            bool impulse = (properties.impulse > 0.0f && Time.time - init_time < properties.impulse);

            current_delta_step = last_step_time == 0.0f ? 0.0f : Time.time - last_step_time;

            if (last_call_time == 0.0f)
            {
                current_delta_call = 0.0f;
                current_delta_notCall = 0.0f;
            }
            else
            {
                if (last_step_time - last_call_time > current_delta_step && !impulse)
                {
                    current_delta_notCall = current_delta_step;
                    ///set a start point to fade velocity
                    init_out = last_velocity;
                    current_delta_call = 0.0f;
                    time_calling = 0.0f;
                }
                else
                {
                    current_delta_call = current_delta_step;
                    ///set a start point to fade velocity
                    init_in = last_velocity;
                    current_delta_notCall = 0.0f;
                    time_notCalling = 0.0f;
                }
            }

            time_calling += current_delta_call;
            time_stepping += current_delta_step;
        }

        protected Vector2 FadeIn()
        {
            // float fade_in_fact = 0.0f;
            Vector2 output = Vector2.zero;

            if (current_delta_call <= 0.0f) return Vector2.zero;

            //fade_in_fact = Mathf.Clamp01(time_calling / inertia_in);
            output = (goal_vector - last_velocity) * (current_delta_call / properties.inertia_in);

            return output;
        }
        protected Vector2 FadeOut()
        {
            //float fade_out_fact = 0.0f;
            Vector2 output = Vector2.zero;

            if (current_delta_notCall <= 0.0f) return Vector2.zero;

            //fade_out_fact = Mathf.Clamp01(time_notCalling / inertia_in);
            output = (Vector2.zero - last_velocity) * (current_delta_notCall / properties.inertia_out);

            return output;
        }
        protected Vector2 Clamp(Vector2 vector, Vector2 max)
        {
            float max_magnitude = max.magnitude;
            float currentMagnitude = vector.magnitude;
            float dot = Vector2.Dot(vector, max);
            Vector2 output = vector;

            //check if it is calling to clamp
            if (currentMagnitude > max_magnitude && dot > 0.0f && current_delta_call != 0.0f)
            {
                output = max;
            }
            //checl if is not calling to clamp to zero
            else if (dot <= 0.0f && current_delta_call == 0.0f)
            {
                output = Vector2.zero;
            }

            return output;
        }
        protected void EndStep()
        {
            last_step_time = Time.time;
            last_velocity = current_output;
        }
    }
}

