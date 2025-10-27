using UnityEngine;
using UnityEngine.InputSystem;

public class MoveSkill : SkillAction
{
    private bool isActive = true;
    private InputAction moveAction;
    public MoveSkill(SkillsActionController controller, InputAction moveAction) : base(controller)
    {
        this.moveAction = moveAction;
    }

    public override string Identifier => "move";

    public override bool IsActive { get { return isActive; } set { isActive = value; } }

    public override void Update()
    {
        Vector2 input = GetInput();
        Vector3 horizontalVelocity = ComputeHorizontalVelocity(input);
        Controller.velocity = new Vector3(horizontalVelocity.x, Controller.velocity.y, horizontalVelocity.z);
    }

    private Vector3 ComputeHorizontalVelocity(Vector2 input)
    {
        Vector3 horizontal = Controller.transform.right * input.x + Controller.transform.forward * input.y;
        return horizontal * Controller.Speed;
    }

    private Vector2 GetInput()
    {
        if (moveAction != null)
        {
            return moveAction.ReadValue<Vector2>();
        }
        return Vector2.zero;
    }
}
