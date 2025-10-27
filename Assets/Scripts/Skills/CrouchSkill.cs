using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchSkill : SkillAction
{
    private bool isActive = true;
    private InputAction crouchAction;

    public CrouchSkill(SkillsActionController controller, InputAction crouchAction) : base(controller)
    {
        this.crouchAction = crouchAction;
    }

    public override string Identifier => "crouch";

    public override bool IsActive { get => isActive; set => isActive = value; }

    public override void Update()
    {
        HandleCrouch();
    }

    private void HandleCrouch()
    {
        bool requestedCrouch = (crouchAction != null && crouchAction.IsPressed());
        bool shouldCrouch = requestedCrouch || !canUncrouch();

        float transitionSpeed = Controller.CrouchTransitionSpeed * Time.deltaTime;
        float crouchHeight = Controller.CrouchHeight;
        float standingHeight = Controller.StandingHeight;

        float crouchStepHeight = Controller.CrouchStepHeight;
        float uncrouchStepHeight = Controller.UncrouchStepHeight;

        Controller.CharacterController.height = Mathf.Lerp(Controller.CharacterController.height, shouldCrouch ? crouchHeight : standingHeight, transitionSpeed);
        Controller.CharacterController.stepOffset = Mathf.Lerp(Controller.CharacterController.stepOffset, shouldCrouch ? crouchStepHeight : uncrouchStepHeight, transitionSpeed);

        if (Controller != null)
        {
            Controller.CrouchSpeedModifier = shouldCrouch ? Controller.CrouchSpeedMultiplier : 1f;
        }
    }

    private bool canUncrouch()
    {
        float standingHeight = Controller.StandingHeight;
        if (Controller.CharacterController.height >= standingHeight - 0.01f)
            return true;

        Vector3 worldCenter = Controller.transform.TransformPoint(Controller.CharacterController.center);

        Vector3 start = worldCenter + Vector3.up * (Controller.CharacterController.height / 2f);
        Vector3 end = worldCenter + Vector3.up * (standingHeight / 2f);

        float radius = Controller.CharacterController.radius;
        float clearanceMargin = 0.02f;

        int excluded = (1 << 2);
        int layerMaskExcludingIgnoreRaycast = ~excluded;
        bool blocked = Physics.CheckCapsule(start + Vector3.up * clearanceMargin,
                                            end + Vector3.up * clearanceMargin,
                                            radius,
                                            layerMaskExcludingIgnoreRaycast,
                                            QueryTriggerInteraction.Ignore);
        return !blocked;
    }
}
