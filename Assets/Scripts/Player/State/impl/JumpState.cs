using UnityEngine;
using UnityEngine.InputSystem;

public class JumpState : InputState
{
    private readonly float jumpForce;
    private readonly float gravity;
    private readonly float moveSpeed;
    private float verticalVelocity;

    public JumpState(CharacterController controller, PlayerInput input, float jumpForce, float gravity, float moveSpeed) : base(controller, input)
    {
        this.jumpForce = jumpForce;
        this.gravity = gravity;
        this.moveSpeed = moveSpeed;
    }

    public override string Name => "JUMP";

    public override void Enter()
    {
        verticalVelocity = jumpForce;
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
        Vector2 moveInput = Input.actions["Move"].ReadValue<Vector2>();

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
}
