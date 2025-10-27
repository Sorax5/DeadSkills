using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchState : InputState
{
    private readonly float crouchHeight;
    private readonly float standHeight;
    private readonly float speed;
    private readonly float gravity;

    private Vector3 originalCenter;

    public CrouchState(CharacterController controller, PlayerInput playerInput, float crouchHeight, float standHeight, float speed, float gravity) : base(controller, playerInput)
    {
        this.crouchHeight = crouchHeight;
        this.standHeight = standHeight;
        this.speed = speed;
        this.gravity = gravity;

        originalCenter = controller.center;
    }


    public override string Name => "CROUCH";

    public override void Enter()
    {
        Controller.height = crouchHeight;
        Controller.center = new Vector3(originalCenter.x, crouchHeight / 2f, originalCenter.z);
    }

    public override void Exit()
    {
        Controller.height = standHeight;
        Controller.center = originalCenter;
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
        Vector2 moveInput = Input.actions["Move"].ReadValue<Vector2>();

        Vector3 right = Controller.transform.right;
        Vector3 forward = Controller.transform.forward;
        Vector3 horizontal = (right * moveInput.x + forward * moveInput.y) * speed * Time.deltaTime;

        Vector3 vertical = Vector3.zero;
        if (!Controller.isGrounded)
        {
            vertical = Vector3.up * gravity * Time.deltaTime;
        }

        Controller.Move(horizontal + vertical);
    }

    public bool CanStand()
    {
        return !Physics.SphereCast(Controller.transform.position, Controller.radius, Vector3.up, out _, standHeight - crouchHeight);
    }
}
