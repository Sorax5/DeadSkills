using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class LongJumpSkill : SkillAction
{
    private bool isActive = false;
    public LongJumpSkill(SkillsActionController controller) : base(controller)
    {
    }

    public override Skills Identifier => Skills.LongJump;

    public override bool IsActive { get => isActive; set => isActive = value; }

    public override void Start()
    {
        var characterController = Controller.PlayerController.characterController;
        var playerInput = Controller.PlayerController.playerInput;
        var stateMachine = Controller.PlayerController.stateMachine;
        var moveState = Controller.PlayerController.moveState;
        var jumpAction = playerInput.actions["Jump"];
        var Gravity = Controller.PlayerController.Gravity;
        var Speed = Controller.PlayerController.Speed;
        var LongJumpForwardBoost = Controller.PlayerController.LongJumpForwardBoost;
        var LongJumpUpwardBoost = Controller.PlayerController.LongJumpUpwardBoost;

        var crouchState = stateMachine.GetStateByName("CROUCH") as CrouchState;

        var longJumpState = new LongJumpState(characterController, playerInput, LongJumpForwardBoost, LongJumpUpwardBoost, Gravity, Speed);
        stateMachine.AddState(longJumpState);

        stateMachine.AddTransition("CROUCH", longJumpState.Name, () => crouchState.IsSliding() && jumpAction.triggered);
        stateMachine.AddTransition(longJumpState.Name, "MOVE", () => characterController.isGrounded);
    }

    public override void Update()
    {
    }
}
