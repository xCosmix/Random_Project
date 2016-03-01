using UnityEngine;
using System.Collections;
using Forces;

[RequireComponent(typeof(CharacterController))]
public class Entity : MonoBehaviour {

    #region Components

    protected CharacterController character_controller;
    protected ForceController force_controller;
    protected ActionController action_controller;

    #endregion

    #region EntityProps

    public Stats stats;

    #endregion

    #region CharacterController

    public CCProperties characterController;
    protected ForceProps[] default_forces;
    
    protected void LookAtWalkForce ()
    {
        Force current = force_controller.GetForce(default_forces[0]);
        Vector3 current_direction = current == null ? Vector3.zero : current.LastVelocity();
        if (current_direction == Vector3.zero) return;

        Quaternion look_rotation = Quaternion.LookRotation(current_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, look_rotation, Time.deltaTime * characterController.rotation_speed);
    }

    #endregion

    #region Initialization

    void Awake ()
    {
        CustomAwake();
    }

    void Start ()
    {
        character_controller = GetComponent<CharacterController>();
        force_controller = new ForceController(character_controller);
        action_controller = gameObject.AddComponent<ActionController>();

        default_forces = new ForceProps[]{
            new ForceProps("Walk", characterController.speed, 0.2f, 0.2f)
        };

        CustomStart();
    }

    protected virtual void CustomStart ()
    {

    }

    protected virtual void CustomAwake()
    {

    }

    #endregion

    #region Updates

    void Update ()
    {
        CustomUpdate();
        LookAtWalkForce();
        force_controller.Actualize();
	}

    void FixedUpdate ()
    {
        CustomFixedUpdate();
    }

    protected virtual void CustomUpdate ()
    {

    }

    protected virtual void CustomFixedUpdate ()
    {

    }

    #endregion
}

#region Structs

    /// <summary>
    /// the stats of the entity
    /// </summary>
    [System.Serializable]
    public struct Stats
    {
        public int life;
        public int level;
    }

    [System.Serializable]
    public struct CCProperties
    {
        public float speed;
        public float rotation_speed;
        public float weight;
        public ForceProps[] forces;
    }

#endregion
