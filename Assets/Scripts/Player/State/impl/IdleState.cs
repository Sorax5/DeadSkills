using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : InputState
{
    private float gravity;

    public IdleState(CharacterController controller, PlayerInput input, float gravity) : base(controller, input)
    {
        this.gravity = gravity;
    }

    public override string Name => "IDLE";

    public override void Enter()
    {
        Debug.Log("zjbdibdiz");
        animator.SetTrigger("Idle");

    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
        var moveInput = Input.actions["Move"].ReadValue<Vector2>();

        if(!Controller.isGrounded)
        {
            Controller.Move(Vector3.down * Mathf.Abs(gravity) * Time.deltaTime);
        }
    }
}
