using UnityEngine;
using UnityEngine.InputSystem;

public class SlideSkill : SkillAction
{
    private bool isActive = true;
    private InputAction crouchAction;
    private InputAction sprintAction;

    private float slideTimer = 0f;
    private bool isSliding = false;

    // captured slide parameters
    private Vector3 slideDirection = Vector3.zero;
    private float slideDesiredSpeed = 1f;

    public SlideSkill(SkillsActionController controller, InputAction crouchAction, InputAction sprintAction) : base(controller)
    {
        this.crouchAction = crouchAction;
        this.sprintAction = sprintAction;
    }

    public override string Identifier => "slide";

    public override bool IsActive { get => isActive; set => isActive = value; }

    public override void Update()
    {
        HandleSlide();
    }

    private void HandleSlide()
    {
        bool crouching = (crouchAction != null && crouchAction.WasPressedThisFrame());
        bool sprinting = (sprintAction != null && sprintAction.WasPressedThisFrame());

        if (!isSliding)
        {
            // start slide when both crouch + sprint pressed while grounded
            if (crouching && sprinting && Controller.CharacterController.isGrounded)
            {
                StartSlide();
            }
        }
        else
        {
            slideTimer += Time.deltaTime;
            if (slideTimer >= Controller.SlideDuration || !Controller.CharacterController.isGrounded)
            {
                EndSlide();
            }
            else
            {
                // compute a target world speed that decelerates over the slide duration
                float t = Mathf.Clamp01(slideTimer / Controller.SlideDuration);
                // smooth deceleration: initial burst -> base walk speed
                float currentWorldSpeed = Mathf.Lerp(slideDesiredSpeed, Controller.Speed, Mathf.SmoothStep(0f, 1f, t));

                // convert to controller base velocity (before modifiers)
                float appliedMultiplier = Controller.SpeedModifiers.ApplyAllModifiers();
                float baseVel = Mathf.Max(0.0001f, currentWorldSpeed / appliedMultiplier);

                Controller.velocity = new Vector3(slideDirection.x * baseVel,
                                                  Controller.velocity.y,
                                                  slideDirection.z * baseVel);
            }
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = 0f;
        Controller.IsSliding = true;

        // capture current horizontal velocity (base) and the combined multiplier to compute world speed
        Vector3 horizontalVel = new Vector3(Controller.velocity.x, 0f, Controller.velocity.z);
        float currentMultiplier = Controller.SpeedModifiers.ApplyAllModifiers();
        float worldHorizontalSpeed = horizontalVel.magnitude * currentMultiplier;

        // if almost stationary, use forward direction
        Vector3 forward = Controller.transform.forward;
        forward.y = 0f;

        if (horizontalVel.sqrMagnitude > 0.0001f)
        {
            slideDirection = horizontalVel.normalized;
        }
        else if (forward.sqrMagnitude > 0.0001f)
        {
            slideDirection = forward.normalized;
        }
        else
        {
            slideDirection = Vector3.forward;
        }

        // determine desired slide speed based on captured world speed, with a minimum of Controller.Speed
        float baseSpeed = Mathf.Max(worldHorizontalSpeed, Controller.Speed);
        slideDesiredSpeed = baseSpeed * Controller.SlideSpeedMultiplier;

        // small extra burst: optionally add a tiny additive boost to feel snappier
        slideDesiredSpeed += Controller.Speed * 0.1f;

        // set controller velocity such that after SpeedModifiers.ApplyAllModifiers() multiplication in movement we get slideDesiredSpeed
        float baseVelocityForController = slideDesiredSpeed / Mathf.Max(0.0001f, currentMultiplier);
        Controller.velocity = new Vector3(slideDirection.x * baseVelocityForController,
                                          Controller.velocity.y,
                                          slideDirection.z * baseVelocityForController);

        // reduce character height a bit to represent slide (instant)
        Controller.CharacterController.height = Controller.CrouchHeight;
    }

    private void EndSlide()
    {
        isSliding = false;
        slideTimer = 0f;
        slideDirection = Vector3.zero;
        slideDesiredSpeed = 0f;
        Controller.IsSliding = false;

        // ensure no lingering modifier
        Controller.SpeedModifiers.RemoveModifier(Identifier);
    }
}
