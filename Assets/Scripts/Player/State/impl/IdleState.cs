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
        animator.SetBool("IsStatic", true);
    }

    public override void Exit()
    {
        animator.SetBool("IsStatic", false);
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
        if(!Controller.isGrounded)
        {
            Controller.Move(Vector3.down * Mathf.Abs(gravity) * Time.deltaTime);
        }
    }
}
