using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public PlayerInput playerInput;
    public Transform cameraTransform;

    public StateMachine stateMachine;

    public Animator animator;
    public GameObject mesh;

    public IdleState idleState;
    public MoveState moveState;
    public JumpState jumpState;

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
        this.stateMachine.animator = animator;
    }

    private void Start()
    {
        // create states
        idleState = new IdleState(characterController, playerInput, Gravity);
        moveState = new MoveState(characterController, playerInput, Speed, Gravity);
        
        jumpState = new JumpState(characterController, playerInput, JumpForce, Gravity, Speed);
        

        // register states
        stateMachine.AddState(idleState);
        stateMachine.AddState(moveState);
        stateMachine.AddState(jumpState);

        // cache actions
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];
        crouchAction = playerInput.actions["Crouch"];

        // Transitions (use state names)
        stateMachine.AddTransition(idleState.Name, moveState.Name, () => moveAction.ReadValue<Vector2>().magnitude > MoveThreshold);
        stateMachine.AddTransition(moveState.Name, idleState.Name, () => moveAction.ReadValue<Vector2>().magnitude <= MoveThreshold && characterController.isGrounded);

        

        // Any -> Jump (when jump pressed and grounded) - Prevent if crouch is held
        stateMachine.AddTransition(null, jumpState.Name, () =>
            (Time.time - lastJumpPressedTime <= JumpBufferTime) &&
            (characterController.isGrounded || Time.time - lastGroundedTime <= CoyoteTime) &&
            !crouchAction.IsPressed());

        stateMachine.AddTransition(jumpState.Name, idleState.Name, () => characterController.isGrounded && moveAction.ReadValue<Vector2>().magnitude <= MoveThreshold);
        stateMachine.AddTransition(jumpState.Name, moveState.Name, () => characterController.isGrounded && moveAction.ReadValue<Vector2>().magnitude > MoveThreshold && !sprintAction.IsPressed());
        
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
        /*Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        mesh.transform.forward = (mesh.transform.position + new Vector3(moveInput.x,0,moveInput.y).normalized).normalized;
*/
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
