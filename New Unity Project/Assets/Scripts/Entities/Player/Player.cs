using UnityEngine;
using System.Collections;
using System;
using Forces;

public partial class Player : Entity {

    protected static Player internalPlayer;
    public static Player player
    {
        get { return internalPlayer; }
        set { internalPlayer = value; }
    }

    protected PlayerCamera playerCamera;
    protected PlayerInput playerInput;

    protected Vector2 lastAimDir = Vector2.up;
    public override Vector2 AimDir
    {
        get
        {
            if (Mathf.Abs(playerInput.rightHorizontalAxis) > 0.5 || Mathf.Abs(playerInput.rightVerticalAxis) > 0.5)
            {
                lastAimDir = new Vector2(playerInput.rightHorizontalAxis, playerInput.rightVerticalAxis);
            }
            return lastAimDir;
        }
        set { return; }
    }
    protected int score = 0;
    public int Score
    {
        get { return score; }
        set { score += value; }
    }

    private Weapon cannon;
    private Weapon blade;

    // Use this for initialization
    protected override void CustomStart () {
        animator = GetComponent<Animator>();
        playerCamera = PlayerCamera.Get();
        player = this;

        cannon = new NormalCannon(this);
        blade = new NormalBlade(this);

        StartActions();
	}
    protected void StartActions ()
    {
        Action.Create<InputController>(this.gameObject);
        Action.Create<CameraController>(this.gameObject);

        Action.Create<Cannon>(this.gameObject);
        Action.Create<Idle>(this.gameObject);
        Action.Create<Move>(this.gameObject);
    }
    // Update is called once per frame
    protected override void CustomUpdate() {  }
}

#region Structs

public struct PlayerInput
{
    public float horizontalAxis;
    public float verticalAxis;
    public float rightHorizontalAxis;
    public float rightVerticalAxis;
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
            player.playerInput.horizontalAxis = Input.GetAxis("Horizontal");
            player.playerInput.verticalAxis = Input.GetAxis("Vertical");
            player.playerInput.rightHorizontalAxis = Input.GetAxis("R_Horizontal");
            player.playerInput.rightVerticalAxis = Input.GetAxis("R_Vertical");
        }
    }

    [ActionProperties("Controller", "Camera", 0, false, ActionProperties.UpdateType.FixedUpdate)]
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
            Vector3 addition = Vector3.zero;
            addition = player.AimDir * 2.0f;
            player.playerCamera.Target(player.transform.position + addition);
        }
    }

    [ActionProperties("Move", "Movement", 1, false)]
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
            if (player.playerInput.verticalAxis != 0.0f || player.playerInput.horizontalAxis != 0.0f)
            {
                Vector2 walk_dir = new Vector2(player.playerInput.horizontalAxis, player.playerInput.verticalAxis);
                player.rigidBody.AddForce(walk_dir * player.characterController.Speed);
                player.animator.SetBool("run", true);
            }
        }
    }
    [ActionProperties("Idle", "Movement", 0, false)]
    class Idle : Action
    {
        public override void End() { }
        public override void Suspended() { }
        public override void Queued() { }
        public override void Start() { }

        public override bool Fail() { return false; }
        public override bool Goal() { return false; }

        public override void Update()
        {
            player.animator.SetBool("run", false);
        }
    }
    [ActionProperties("Cannon", "Weapons", 1, false)]
    class Cannon : Action
    {
        public bool usedRT = false;
        public bool usedLT = false;

        public override void End() { }
        public override void Suspended() { }
        public override void Queued() { }
        public override void Start() { }

        public override bool Fail() { return false; }
        public override bool Goal() { return false; }

        public override void Update()
        {
            if (Input.GetAxis("RT") < 0.2f)
                usedRT = false;

            if (Input.GetAxis("RT") > 0.8f && !usedRT)
            {
                player.cannon.Trigger(player.AimDir);
                usedRT = true;
            }

            if (Input.GetAxis("LT") < 0.2f)
                usedLT = false;

            if (Input.GetAxis("LT") > 0.8f && !usedLT)
            {
                player.blade.Trigger(player.AimDir);
                usedLT = true;
            }
        }
    }

}

#endregion