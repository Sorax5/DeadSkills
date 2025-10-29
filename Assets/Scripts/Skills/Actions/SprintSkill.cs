using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class SprintSkill : SkillAction
{
    private bool isActive = true;

    public SprintSkill(SkillsActionController controller) : base(controller)
    {
    }

    public override Skills Identifier => Skills.Sprint;

    public override bool IsActive { get => isActive; set => isActive = value; }

    public override void Start()
    {
        var stateMachine = Controller.PlayerController.stateMachine;

        var characterController = Controller.PlayerController.characterController;
        var playerInput = Controller.PlayerController.playerInput;
        var SprintSpeed = Controller.PlayerController.SprintSpeed;
        var Gravity = Controller.PlayerController.Gravity;

        var moveAction = playerInput.actions["MOVE"];
        var sprintAction = playerInput.actions["SPRINT"];
        var MoveThreshold = Controller.PlayerController.MoveThreshold;

        var sprintState = new SprintState(characterController, playerInput, SprintSpeed, Gravity);
        stateMachine.AddState(sprintState);

        stateMachine.AddTransition("MOVE", sprintState.Name, () => sprintAction.IsPressed() && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && characterController.isGrounded);
        stateMachine.AddTransition(sprintState.Name, "MOVE", () => !sprintAction.IsPressed() && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && characterController.isGrounded);
        stateMachine.AddTransition("IDLE", sprintState.Name, () => sprintAction.IsPressed() && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && characterController.isGrounded);

        stateMachine.AddTransition("JUMP", sprintState.Name, () => characterController.isGrounded && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && sprintAction.IsPressed());

        Debug.Log("SprintSkill started and transitions added.");
    }

    public override void Update()
    {
    }
}
