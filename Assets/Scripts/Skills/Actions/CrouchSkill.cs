
using UnityEngine;

public class CrouchSkill : SkillAction
{
    private bool isActive = false;
    public CrouchSkill(SkillsActionController controller) : base(controller)
    {
    }

    public override Skills Identifier => Skills.Crouch;

    public override bool IsActive { get => isActive; set => isActive = value; }

    public override void Start()
    {
        var characterController = Controller.CharacterController;
        var playerInput = Controller.PlayerInput;
        var CrouchHeight = Controller.PlayerController.CrouchHeight;
        var Speed = Controller.PlayerController.Speed;
        var Gravity = Controller.PlayerController.Gravity;

        var crouchAction = playerInput.actions["Crouch"];
        var sprintAction = playerInput.actions["Sprint"];
        var moveAction = playerInput.actions["Move"];
        var jumpAction = playerInput.actions["Jump"];
        var SprintSpeed = Controller.PlayerController.SprintSpeed;
        var MoveThreshold = Controller.PlayerController.MoveThreshold;

        var crouchState = new CrouchState(characterController, playerInput, CrouchHeight, characterController.height, Speed, Gravity);
        var stateMachine = Controller.PlayerController.stateMachine;

        stateMachine.AddState(crouchState);
        stateMachine.AddTransition("SPRINT", "CROUCH", () =>
        {
            if (crouchAction.triggered && characterController.isGrounded)
            {
                crouchState.BeginSlide(SprintSpeed * 1.2f);
                return true;
            }
            return false;
        });

        stateMachine.AddTransition("JUMP", "CROUCH", () =>
        {
            if (crouchAction.triggered)
            {
                float inputMag = Mathf.Max(moveAction.ReadValue<Vector2>().magnitude, 0.2f);
                float baseSpeed = sprintAction.IsPressed() ? SprintSpeed : Speed;
                float initialVel = baseSpeed * inputMag * 1.2f;
                crouchState.BeginSlide(initialVel);
                return true;
            }
            return false;
        });

        stateMachine.AddTransition(crouchState.Name, "MOVE", () => crouchState.CanStand() && !crouchState.IsSliding() && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && !crouchAction.IsPressed());
        stateMachine.AddTransition(crouchState.Name, "IDLE", () => crouchState.CanStand() && !crouchState.IsSliding() && moveAction.ReadValue<Vector2>().magnitude <= MoveThreshold && !crouchAction.IsPressed());
        stateMachine.AddTransition(null, crouchState.Name, () => crouchAction.IsPressed() && characterController.isGrounded && !jumpAction.IsPressed() && characterController.height != CrouchHeight);

        Debug.Log("CrouchSkill initialized");
    }

    public override void Update()
    {
    }
}
