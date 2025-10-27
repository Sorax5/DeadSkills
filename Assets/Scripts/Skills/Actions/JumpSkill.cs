using UnityEngine;
using UnityEngine.InputSystem;

public class JumpSkill : SkillAction
{
    private bool isActive = true;
    private InputAction jumpAction;

    public JumpSkill(SkillsActionController controller, InputAction jumpAction) : base(controller)
    {
        this.jumpAction = jumpAction;
    }

    public override string Identifier => "jump";

    public override bool IsActive { get => isActive; set => isActive = value; }

    public override void Update()
    {
        HandleJump();
    }

    private void HandleJump()
    {
        if (jumpAction != null && jumpAction.triggered && Controller.CharacterController.isGrounded)
        {
            Controller.velocity.y = Mathf.Sqrt(2f * Controller.JumpHeight * Mathf.Abs(Controller.Gravity));
        }
    }
}
