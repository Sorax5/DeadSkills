using UnityEngine;
using UnityEngine.InputSystem;

public class IdleState : InputState
{
    private float gravity;
    
    // Cache input action to avoid repeated lookups
    private InputAction moveAction;

    public IdleState(CharacterController controller, PlayerInput input, float gravity) : base(controller, input)
    {
        this.gravity = gravity;
        // Cache action (though not actively used, caching for consistency)
        this.moveAction = input.actions["Move"];
    }

    public override string Name => "IDLE";

    public override void Enter()
    {
        Debug.Log("zjbdibdiz");
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
        // Cache moveAction, though not actively used in this state
        var moveInput = moveAction.ReadValue<Vector2>();

        if(!Controller.isGrounded)
        {
            Controller.Move(Vector3.down * Mathf.Abs(gravity) * Time.deltaTime);
        }
    }
}
