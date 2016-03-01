using UnityEngine;
using System.Collections;
using System;
using Forces;

public partial class Player : Entity {

    protected PlayerCamera player_camera;
    protected PlayerInput player_input;
    protected static Player player;

	// Use this for initialization
	protected override void CustomStart () {
        player_camera = PlayerCamera.Get();
        player = this;

        StartActions();
	}
    protected void StartActions ()
    {
        Action.Create<InputController>(this.gameObject);
        Action.Create<CameraController>(this.gameObject);

        Action.Create<Move>(this.gameObject);
    }
    // Update is called once per frame
    protected override void CustomUpdate() {    }
}

#region Structs

public struct PlayerInput
{
    public float horizontal_axis;
    public float vertical_axis;
}

#endregion

#region PlayerActions

public partial class Player : Entity
{
    [ActionProperties("Controller", "Input", 0, false)]
    class InputController : Action
    {
        public override void End() { }
        public override void Suspended() { }
        public override void Queued() { }
        public override void Start() { }

        public override bool Fail() { return false; }
        public override bool Goal() { return false; }

        public override void Update()
        {
            player.player_input.horizontal_axis = Input.GetAxis("Horizontal");
            player.player_input.vertical_axis = Input.GetAxis("Vertical");
        }
    }

    [ActionProperties("Controller", "Camera", 0, false)]
    class CameraController : Action
    {
        public override void End() { }
        public override void Suspended() { }
        public override void Queued() { }
        public override void Start() { }

        public override bool Fail() { return false; }
        public override bool Goal() { return false; }

        public override void Update()
        {
            Force walk_force = player.force_controller.GetForce(player.default_forces[0]);
            Vector3 addition = Vector3.zero;
            if (walk_force != null) addition = walk_force.LastVelocity();
            player.player_camera.Target(player.transform.position + addition * 0.6f);
        }
    }

    [ActionProperties("Move", "Player", 1, false)]
    class Move : Action
    {
        public override void End() { }
        public override void Suspended() { }
        public override void Queued() { }
        public override void Start() { }

        public override bool Fail() { return false; }
        public override bool Goal() { return false; }

        public override void Update()
        {
            if (player.player_input.vertical_axis != 0.0f || player.player_input.horizontal_axis != 0.0f)
            {
                Vector3 walk_dir = new Vector3(player.player_input.horizontal_axis, 0.0f, player.player_input.vertical_axis);
                player.force_controller.AddForce(player.default_forces[0], walk_dir);
            }
        }
    }
}

#endregion