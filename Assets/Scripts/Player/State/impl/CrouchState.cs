using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchState : InputState
{
    private readonly float crouchHeight;
    private readonly float standHeight;
    private readonly float speed;
    private readonly float gravity;

    private Vector3 originalCenter;

    // sliding
    private bool sliding;
    private float currentSlideVelocity;
    private readonly float slideStartSpeedMultiplier = 1.2f; // start slide slightly faster than crouch move
    private readonly float slideDecay = 3f; // how fast slide slows when not holding forward
    private readonly float minSlideVelocity = 0.1f;

    // allow external request to begin slide at Enter()
    private float pendingSlideVelocity = 0f;

    public CrouchState(CharacterController controller, PlayerInput playerInput, float crouchHeight, float standHeight, float speed, float gravity) : base(controller, playerInput)
    {
        this.crouchHeight = crouchHeight;
        this.standHeight = standHeight;
        this.speed = speed;
        this.gravity = gravity;

        originalCenter = controller.center;
        sliding = false;
        currentSlideVelocity = 0f;
        pendingSlideVelocity = 0f;
    }


    public override string Name => "CROUCH";

    public override void Enter()
    {
        Controller.height = crouchHeight;
        Controller.center = new Vector3(originalCenter.x, crouchHeight / 2f, originalCenter.z);
        sliding = false;
        currentSlideVelocity = 0f;

        // if an external slide was requested before entering, apply it now
        if (pendingSlideVelocity > minSlideVelocity)
        {
            sliding = true;
            currentSlideVelocity = pendingSlideVelocity;
        }
        pendingSlideVelocity = 0f;
        
        animator.SetTrigger("Crouch");
    }

    public override void Exit()
    {
        Controller.height = standHeight;
        Controller.center = originalCenter;
        sliding = false;
        currentSlideVelocity = 0f;
        pendingSlideVelocity = 0f;
        animator.SetTrigger("Uncrouch");
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
        Vector2 moveInput = Input.actions["Move"].ReadValue<Vector2>();
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (moveInput.magnitude > 0.001 && !stateInfo.IsName("WalkCrouch"))
        {
            animator.SetTrigger("WalkCrouch");
        }
        else if(moveInput.magnitude < 0.001 && !stateInfo.IsName("IdleCrouch"))
        {
            animator.SetTrigger("IdleCrouch");
        }

        Vector3 right = Controller.transform.right;
        Vector3 forward = Controller.transform.forward;
        forward.y = 0f;
        right.y = 0f;

        Vector3 inputDir = (right * moveInput.x + forward * moveInput.y);
        float inputMag = inputDir.magnitude;

        Vector3 horizontal = Vector3.zero;

        if (sliding)
        {
            currentSlideVelocity = Mathf.MoveTowards(currentSlideVelocity, 0f, slideDecay * Time.deltaTime);
            if (currentSlideVelocity <= minSlideVelocity)
            {
                sliding = false;
                currentSlideVelocity = 0f;
            }
            else
            {
                Vector3 slideDir = Controller.transform.forward;
                slideDir.y = 0f;
                if (slideDir.sqrMagnitude <= 0.001f) slideDir = Vector3.forward;
                slideDir.Normalize();
                horizontal = slideDir * currentSlideVelocity * Time.deltaTime;
            }
        }
        else
        {
            if (inputMag > 0.01f)
            {
                horizontal = inputDir.normalized * speed * 0.5f * Time.deltaTime;
            }
            else
            {
                horizontal = Vector3.zero;
            }
        }

        Vector3 vertical = Vector3.zero;
        if (!Controller.isGrounded)
        {
            vertical = Vector3.up * gravity * Time.deltaTime;
        }

        Controller.Move(horizontal + vertical);
    }

    public bool CanStand()
    {
        return !Physics.SphereCast(Controller.transform.position, Controller.radius, Vector3.up, out _, standHeight - crouchHeight);
    }

    public bool IsSliding()
    {
        return sliding && currentSlideVelocity > minSlideVelocity;
    }

    public float GetSlideVelocity()
    {
        return currentSlideVelocity;
    }

    public void BeginSlide(float initialVelocity)
    {
        pendingSlideVelocity = initialVelocity;
    }
}
