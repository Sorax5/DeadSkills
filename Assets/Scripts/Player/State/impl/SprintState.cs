using UnityEngine;
using UnityEngine.InputSystem;

public class SprintState : InputState
{
    private readonly float sprintSpeed;
    private readonly float gravity;

    private Vector2 moveInput = Vector2.zero;
    private InputAction moveAction;

    public SprintState(CharacterController controller, PlayerInput input, float sprintSpeed, float gravity) : base(controller, input)
    {
        this.sprintSpeed = sprintSpeed;
        this.gravity = gravity;
        // Cache the action to avoid repeated FindAction calls
        this.moveAction = input.actions["Move"];
    }

    public override string Name => "SPRINT";

    public override void Enter()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMoveCanceled;
    }

    public override void Exit()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMoveCanceled;
        moveInput = Vector2.zero;
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
        // read value each frame to ensure single-key detection
        moveInput = moveAction.ReadValue<Vector2>();

        Vector3 right = Controller.transform.right;
        Vector3 forward = Controller.transform.forward;
        Vector3 horizontal = (right * moveInput.x + forward * moveInput.y).normalized * sprintSpeed * Time.deltaTime;
        Vector3 vertical = Vector3.zero;
        if (!Controller.isGrounded)
        {
            vertical = Vector3.up * gravity * Time.deltaTime;
        }

        Controller.Move(horizontal + vertical);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }
}
