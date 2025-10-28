using UnityEngine;
using UnityEngine.InputSystem;

public class JumpState : InputState
{
    private readonly float jumpForce;
    private readonly float gravity;
    private readonly float moveSpeed;
    private float verticalVelocity;

    private bool jumpButtonHeld = false;
    private float jumpCutMultiplier = 0.5f;
    
    // Cache input actions to avoid repeated lookups
    private InputAction jumpAction;
    private InputAction moveAction;

    public JumpState(CharacterController controller, PlayerInput input, float jumpForce, float gravity, float moveSpeed) : base(controller, input)
    {
        this.jumpForce = jumpForce;
        this.gravity = gravity;
        this.moveSpeed = moveSpeed;
        // Cache actions
        this.jumpAction = input.actions["Jump"];
        this.moveAction = input.actions["Move"];
    }

    public override string Name => "JUMP";

    public override void Enter()
    {
        verticalVelocity = jumpForce;
        jumpButtonHeld = jumpAction.IsPressed();
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
        bool currentlyHeld = jumpAction.IsPressed();
        if (!currentlyHeld && jumpButtonHeld && verticalVelocity > 0)
        {
            verticalVelocity *= jumpCutMultiplier;
        }
        jumpButtonHeld = currentlyHeld;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        Vector3 right = Controller.transform.right;
        Vector3 forward = Controller.transform.forward;
        Vector3 horizontal = (right * moveInput.x + forward * moveInput.y).normalized * moveSpeed * Time.deltaTime;

        if (Controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 vertical = Vector3.up * verticalVelocity * Time.deltaTime;

        Controller.Move(horizontal + vertical);
    }

    public void CutJump()
    {
        if (verticalVelocity > 0)
        {
            verticalVelocity *= jumpCutMultiplier;
        }
    }
}
