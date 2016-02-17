using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Force : System.Object
{

    public ForceProps properties;
     
    protected Vector3 goal_vector = Vector3.forward;
    protected Vector3 last_velocity = Vector3.zero;
    protected Vector3 init_in = Vector3.zero;
    protected Vector3 init_out = Vector3.zero;
    protected Vector3 current_output = Vector3.zero;
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

    public Force(string id, Vector3 target_direction, float target_speed, float inertia_in = 1.0f, float inertia_out = 1.0f)
    {
        properties = new ForceProps(id, target_speed, inertia_in, inertia_out);

        Call(target_direction);
    }
    public Force(ForceProps props, Vector3 target_direction)
    {
        properties = props;

        Call(target_direction);
    }
    public Vector3 Velocity()
    {
        Vector3 delta = Vector3.zero;
        Vector3 current_target_velocity = Vector3.zero;

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
    public void Call(Vector3 new_goal_vector)
    {
        last_call_time = Time.time;
        goal_vector = new_goal_vector.normalized * properties.target_speed;
    }
    /// <summary>
    /// Determines if the force is being used after reaching zero force
    /// </summary>
    /// <returns></returns>
    public bool Using ()
    {
        //Wait half a second to check if the velocity is null
        return (Time.time - last_call_time) < 0.5f || last_velocity != Vector3.zero;
    }
    /// <summary>
    /// Returns the value of the last call
    /// </summary>
    /// <returns></returns>
    public Vector3 LastVelocity ()
    {
        return last_velocity;
    }

    protected void Initialize()
    {
        init_time = Time.time;
    }

    protected void StartStep()
    {
        current_delta_step = last_step_time == 0.0f ? 0.0f : Time.time - last_step_time;
       
        if (last_call_time == 0.0f)
        {
            current_delta_call = 0.0f;
            current_delta_notCall = 0.0f;
        }
        else
        {
            if (last_step_time - last_call_time > current_delta_step)
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

    protected Vector3 FadeIn ()
    {
       // float fade_in_fact = 0.0f;
        Vector3 output = Vector3.zero;

        if (current_delta_call <= 0.0f) return Vector3.zero;

        //fade_in_fact = Mathf.Clamp01(time_calling / inertia_in);
        output = (goal_vector - last_velocity) * (current_delta_call / properties.inertia_in);

        return output;
    }
    protected Vector3 FadeOut ()
    {
        //float fade_out_fact = 0.0f;
        Vector3 output = Vector3.zero;

        if (current_delta_notCall <= 0.0f) return Vector3.zero;

        //fade_out_fact = Mathf.Clamp01(time_notCalling / inertia_in);
        output = (Vector3.zero - last_velocity) * (current_delta_notCall / properties.inertia_in);

        return output;
    }
    protected Vector3 Clamp (Vector3 vector, Vector3 max)
    {
        float max_magnitude = max.magnitude;
        float currentMagnitude = vector.magnitude;
        float dot = Vector3.Dot(vector, max);
        Vector3 output = vector;

        //check if it is calling to clamp
        if (currentMagnitude > max_magnitude && dot > 0.0f && current_delta_call != 0.0f)
        {
            output = max;
        }
        //checl if is not calling to clamp to zero
        else if (dot <= 0.0f && current_delta_call == 0.0f)
        {
            output = Vector3.zero;
        }

        return output;
    }
    protected void EndStep()
    {
        last_step_time = Time.time;
        last_velocity = current_output;
    }
}

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

    public Force2Call (Vector3 direction, Force force)
    {
        this.direction = direction;
        this.force = force;
    }
}
/// <summary>
/// Controls forces to be esay for the user to use
/// </summary>
public class ForceController : System.Object
{
    protected List<Force> active_forces = new List<Force>();
    protected List<Force2Call> call_forces = new List<Force2Call>();
    protected List<int> null_forces_index = new List<int>();
    protected CharacterController controller;

    public ForceController (CharacterController controller)
    {
        this.controller = controller;
    }

    public void AddForce (ForceProps forceProps, Vector3 direction)
    {
        Force target = Target(forceProps, direction);
        call_forces.Add(new Force2Call(direction, target));
    }

    public Force GetForce (ForceProps force)
    {
        foreach (Force f in active_forces)
        {
            if (f.properties.Name == force.Name) return f;
        }
        return null;
    }

    public void Actualize ()
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

    protected Force Target (ForceProps force, Vector3 direction)
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

