using UnityEngine;
using UnityEngine.InputSystem;

public class SprintSkill : SkillAction
{
    private bool isActive = true;
    private InputAction sprintAction;

    public SprintSkill(SkillsActionController controller, InputAction sprintAction) : base(controller)
    {
        this.sprintAction = sprintAction;
    }

    public override string Identifier => "sprint";

    public override bool IsActive { get => isActive; set => isActive = value; }

    public override void Update()
    {
        HandleSprint();
    }

    private void HandleSprint()
    {
        bool isSprinting = (sprintAction != null && sprintAction.IsPressed());

        if (isSprinting)
        {
            Controller.SpeedModifiers.SetModifier(Identifier, Controller.SprintSpeedMultiplier);
        }
        else
        {
            Controller.SpeedModifiers.RemoveModifier(Identifier);
        }
    }
}
