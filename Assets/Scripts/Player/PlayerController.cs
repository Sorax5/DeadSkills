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

    public float Speed = 5f;
    public float SprintSpeed = 8f;
    public float JumpForce = 5f;
    public float Gravity = -9.81f;
    public float SlideSpeed = 8f;
    public float CrouchHeight = 1f;

    public float MoveThreshold = 0.1f;

    public string CurrentState = "NONE";

    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    public float CoyoteTime = 0.2f;
    public float JumpBufferTime = 0.15f;

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

        // register states
        stateMachine.AddState(idleState);
        stateMachine.AddState(moveState);
        stateMachine.AddState(sprintState);
        stateMachine.AddState(crouchState);
        stateMachine.AddState(jumpState);

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
        stateMachine.AddTransition(crouchState.Name, moveState.Name, () => crouchState.CanStand() && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && !crouchAction.IsPressed());
        stateMachine.AddTransition(crouchState.Name, idleState.Name, () => crouchState.CanStand() && moveAction.ReadValue<Vector2>().magnitude <= MoveThreshold && !crouchAction.IsPressed());

        stateMachine.SetState(idleState);
    }

    private void Update()
    {
        if (jumpAction != null && jumpAction.IsPressed())
        {
            lastJumpPressedTime = Time.time;
        }

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
        stateMachine.FixedUpdate();
    }
}
