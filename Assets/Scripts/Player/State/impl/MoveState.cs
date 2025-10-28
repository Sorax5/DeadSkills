using UnityEngine;
using UnityEngine.InputSystem;

public class MoveState : InputState
{
    private float speed;
    private float gravity;

    private Vector2 moveInput = Vector2.zero;

    public MoveState(CharacterController controller, PlayerInput input, float speed, float gravity) : base(controller, input)
    {
        this.speed = speed;
        this.gravity = gravity;
    }

    public override string Name => "MOVE";

    public override void Enter()
    {
        Input.actions.FindAction("Move").performed += OnMove;
        Input.actions.FindAction("Move").canceled += OnMoveCanceled;
        animator.SetTrigger("Walk");
        Debug.Log("ahahahaha");
    }

    public override void Exit()
    {
        Input.actions.FindAction("Move").performed -= OnMove;
        Input.actions.FindAction("Move").canceled -= OnMoveCanceled;
        moveInput = Vector2.zero;
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
        // Read current value each frame to ensure single-key presses are always captured
        moveInput = Input.actions["Move"].ReadValue<Vector2>();

        // Convert input (x:right, y:forward) into world direction based on player rotation
        Vector3 right = Controller.transform.right;
        Vector3 forward = Controller.transform.forward;
        Vector3 horizontal = (right * moveInput.x + forward * moveInput.y).normalized * speed * Time.deltaTime;

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
