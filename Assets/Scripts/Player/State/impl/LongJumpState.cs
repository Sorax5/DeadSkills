using UnityEngine;
using UnityEngine.InputSystem;

public class LongJumpState : InputState
{
    private readonly float forwardBoost;
    private readonly float upwardBoost;
    private readonly float gravity;
    private readonly float moveSpeed;

    private Vector3 initialVelocity;
    private float duration = 0.6f;
    private float startTime;

    public LongJumpState(CharacterController controller, PlayerInput input, float forwardBoost, float upwardBoost, float gravity, float moveSpeed) : base(controller, input)
    {
        this.forwardBoost = forwardBoost;
        this.upwardBoost = upwardBoost;
        this.gravity = gravity;
        this.moveSpeed = moveSpeed;
    }

    public override string Name => "LONGJUMP";

    public override void Enter()
    {
        startTime = Time.time;

        Vector3 dir = Controller.transform.forward;
        dir.y = 0f;
        dir.Normalize();

        initialVelocity = dir * forwardBoost + Vector3.up * upwardBoost;
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
        float elapsed = Time.time - startTime;

        Vector3 vertical = Vector3.up * (initialVelocity.y + gravity * elapsed) * Time.deltaTime;
        Vector3 horizontal = new Vector3(initialVelocity.x, 0, initialVelocity.z) * Time.deltaTime;

        Vector2 moveInput = Input.actions["Move"].ReadValue<Vector2>();
        Vector3 right = Controller.transform.right;
        Vector3 forward = Controller.transform.forward;
        forward.y = 0f;
        right.y = 0f;

        Vector3 airControl = (right * moveInput.x + forward * moveInput.y) * (moveSpeed * 0.5f) * Time.deltaTime;

        Controller.Move(horizontal + vertical + airControl);
    }
}
