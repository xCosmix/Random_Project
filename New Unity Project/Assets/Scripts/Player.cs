using UnityEngine;
using System.Collections;

public class Player : Entity {

    protected PlayerCamera player_camera;
	// Use this for initialization
	protected override void CustomStart () {
        player_camera = PlayerCamera.Get();
	}

    // Update is called once per frame
    protected override void CustomUpdate()
    {
        InputController();
        CameraController();
    }

    protected void InputController ()
    {
        float horizontal_axis = Input.GetAxis("Horizontal");
        float vertical_axis = Input.GetAxis("Vertical");

        if (vertical_axis != 0.0f || horizontal_axis != 0.0f)
        {
            Vector3 walk_dir = new Vector3(horizontal_axis, 0.0f, vertical_axis);
            force_controller.AddForce(default_forces[0], walk_dir);
        }
    }
    protected void CameraController ()
    {
        Force walk_force = force_controller.GetForce(default_forces[0]);
        Vector3 addition = Vector3.zero;
        if (walk_force != null) addition = walk_force.LastVelocity();
        player_camera.Target(transform.position + addition * 0.6f);
    }
}
