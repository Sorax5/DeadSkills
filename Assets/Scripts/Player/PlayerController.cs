using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public PlayerInput playerInput;
    public Transform cameraTransform;

    public StateMachine stateMachine;

    public IdleState idleState;
    public MoveState moveState;
    public SprintState sprintState;
    public CrouchState crouchState;
    public JumpState jumpState;
    public LongJumpState longJumpState;

    public float Speed = 5f;
    public float SprintSpeed = 8f;
    public float JumpForce = 5f;
    public float Gravity = -9.81f;
    public float SlideSpeed = 8f;
    public float CrouchHeight = 1f;

    public float MoveThreshold = 0.1f;

    public string CurrentState = "NONE";
    public bool IsEnabled = true;

    // cached input actions
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    // long jump config
    public float LongJumpForwardBoost = 10f;
    public float LongJumpUpwardBoost = 8f;

    // jump responsiveness
    public float CoyoteTime = 0.2f; // allow jump shortly after leaving ground
    public float JumpBufferTime = 0.15f; // allow jump pressed shortly before landing

    private float lastGroundedTime = -Mathf.Infinity;
    private float lastJumpPressedTime = -Mathf.Infinity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main != null ? Camera.main.transform : null;
        this.stateMachine = new StateMachine();
    }

    private void Start()
    {
        // create states
        idleState = new IdleState(characterController, playerInput, Gravity);
        moveState = new MoveState(characterController, playerInput, Speed, Gravity);
        sprintState = new SprintState(characterController, playerInput, SprintSpeed, Gravity);
        crouchState = new CrouchState(characterController, playerInput, CrouchHeight, characterController.height, Speed, Gravity);
        jumpState = new JumpState(characterController, playerInput, JumpForce, Gravity, Speed);
        longJumpState = new LongJumpState(characterController, playerInput, LongJumpForwardBoost, LongJumpUpwardBoost, Gravity, Speed);

        // register states
        stateMachine.AddState(idleState);
        stateMachine.AddState(moveState);
        stateMachine.AddState(sprintState);
        stateMachine.AddState(crouchState);
        stateMachine.AddState(jumpState);
        stateMachine.AddState(longJumpState);

        // cache actions
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];
        crouchAction = playerInput.actions["Crouch"];

        // Transitions (use state names)
        stateMachine.AddTransition(idleState.Name, moveState.Name, () => moveAction.ReadValue<Vector2>().magnitude > MoveThreshold);
        stateMachine.AddTransition(moveState.Name, idleState.Name, () => moveAction.ReadValue<Vector2>().magnitude <= MoveThreshold && characterController.isGrounded);

        stateMachine.AddTransition(moveState.Name, sprintState.Name, () => sprintAction.IsPressed() && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && characterController.isGrounded);
        stateMachine.AddTransition(sprintState.Name, moveState.Name, () => !sprintAction.IsPressed() && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && characterController.isGrounded);
        stateMachine.AddTransition(idleState.Name, sprintState.Name, () => sprintAction.IsPressed() && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && characterController.isGrounded);

        // Sprint -> Crouch (start slide) when crouch pressed while sprinting
        stateMachine.AddTransition(sprintState.Name, crouchState.Name, () =>
        {
            if (crouchAction.triggered && characterController.isGrounded)
            {
                // request crouch state to begin sliding with sprint momentum
                crouchState.BeginSlide(SprintSpeed * 1.2f);
                return true;
            }
            return false;
        });

        // Jump -> Crouch (allow mid-air crouch to prepare slide on landing)
        stateMachine.AddTransition(jumpState.Name, crouchState.Name, () =>
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

        // Any -> Jump (when jump pressed and grounded) - Prevent if crouch is held
        stateMachine.AddTransition(null, jumpState.Name, () =>
            (Time.time - lastJumpPressedTime <= JumpBufferTime) &&
            (characterController.isGrounded || Time.time - lastGroundedTime <= CoyoteTime) &&
            !crouchAction.IsPressed());

        stateMachine.AddTransition(jumpState.Name, idleState.Name, () => characterController.isGrounded && moveAction.ReadValue<Vector2>().magnitude <= MoveThreshold);
        stateMachine.AddTransition(jumpState.Name, moveState.Name, () => characterController.isGrounded && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && !sprintAction.IsPressed());
        stateMachine.AddTransition(jumpState.Name, sprintState.Name, () => characterController.isGrounded && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && sprintAction.IsPressed());

        // Any -> Crouch when crouch pressed - only if grounded and not jumping
        stateMachine.AddTransition(null, crouchState.Name, () => crouchAction.triggered && characterController.isGrounded && !jumpAction.IsPressed() && characterController.height != CrouchHeight);
        stateMachine.AddTransition(crouchState.Name, moveState.Name, () => crouchState.CanStand() && !crouchState.IsSliding() && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && !crouchAction.IsPressed());
        stateMachine.AddTransition(crouchState.Name, idleState.Name, () => crouchState.CanStand() && !crouchState.IsSliding() && moveAction.ReadValue<Vector2>().magnitude <= MoveThreshold && !crouchAction.IsPressed());

        // Crouch -> LongJump when sliding and jump pressed
        stateMachine.AddTransition(crouchState.Name, longJumpState.Name, () => crouchState.IsSliding() && jumpAction.triggered);

        // LongJump -> Move when grounded
        stateMachine.AddTransition(longJumpState.Name, moveState.Name, () => characterController.isGrounded);

        stateMachine.SetState(idleState);
    }

    private void Update()
    {
        if (!IsEnabled)
        {
            return;
        }
        // track jump press for buffer - update while holding so player can hold space to auto-jump on landing
        if (jumpAction != null && jumpAction.IsPressed())
        {
            lastJumpPressedTime = Time.time;
        }

        // track grounded time for coyote
        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        stateMachine.Update();
        CurrentState = stateMachine.CurrentState != null ? stateMachine.CurrentState.Name : "NONE";

        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            forward.y = 0;
            if (forward.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), 0.1f);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!IsEnabled)
        {
            return;
        }
        stateMachine.FixedUpdate();
    }
}
