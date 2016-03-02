using UnityEngine;
using System.Collections;

namespace Forces
{
    /// <summary>
    /// Label force static information
    /// </summary>
    [System.Serializable]
    public struct ForceProps
    {
        /// <summary>
        /// indetify instance id
        /// </summary>
        public string Name;
        /// <summary>
        /// inertia value when starts in seconds (fade in)
        /// </summary>
        public float inertia_in;
        /// <summary>
        /// inertia value when ends in seconds (fade out)
        /// </summary>
        public float inertia_out;
        /// <summary>
        /// goal speed
        /// </summary>
        public float target_speed;

        public ForceProps(string Name, float target_speed, float inertia_in = 1.0f, float inertia_out = 1.0f)
        {
            this.Name = Name;
            this.target_speed = target_speed;
            this.inertia_in = inertia_in;
            this.inertia_out = inertia_out;
        }
    }
    /// <summary>
    /// Target force and new vector to apply
    /// </summary>
    public struct Force2Call
    {
        public Vector3 direction;
        public Force force;

        public Force2Call(Vector3 direction, Force force)
        {
            this.direction = direction;
            this.force = force;
        }
    }
}
