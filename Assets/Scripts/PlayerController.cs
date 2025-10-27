using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private const string MOVE_ACTION = "Move";
    private const string JUMP_ACTION = "Jump";
    private const string CROUCH_ACTION = "Crouch";
    private const string SPRINT_ACTION = "Sprint";

    public event Action OnJump;
    public event Action<bool> OnCrouch;
    public event Action<bool> OnSprint;

    public float Height => characterController.height;

    [Header("Movement")]
    [SerializeField] private float speed = 6.0f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float rotationSpeed = 0.1f;

    [Header("Crouch")]
    [SerializeField] public float crouchHeight = 1.0f;
    [SerializeField] public float standingHeight = 2.0f;

    [SerializeField] private float crouchStepHeight = 0.3f;
    [SerializeField] private float uncrouchStepHeight = 0.5f;

    [SerializeField] private float crouchTransitionSpeed = 5.0f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    [Header("Sprint")]
    [SerializeField] private float sprintSpeedMultiplier = 1.5f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private PlayerInput playerInput;

    #region Input Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction sprintAction;
    #endregion

    private Vector3 velocity;
    private bool isGrounded;
    private bool wasSprinting;
    private bool wasCrouching;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions.FindAction(MOVE_ACTION);
        jumpAction = playerInput.actions.FindAction(JUMP_ACTION);
        crouchAction = playerInput.actions.FindAction(CROUCH_ACTION);
        sprintAction = playerInput.actions.FindAction(SPRINT_ACTION);

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        wasSprinting = false;
        wasCrouching = characterController != null && characterController.height <= (crouchHeight + 0.01f);
    }

    private void Update()
    {
        ProcessGround();

        Vector2 input = ReadMoveInput();

        HandleSprint();

        Vector3 horizontalVelocity = ComputeHorizontalVelocity(input);

        HandleJump();
        HandleCrouch();
        HandleRotation();
        ApplyGravity();

        Move(horizontalVelocity);
    }

    private void ProcessGround()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private Vector2 ReadMoveInput()
    {
        if (moveAction != null)
        {
            return moveAction.ReadValue<Vector2>();
        }
        return Vector2.zero;
    }

    private Vector3 ComputeHorizontalVelocity(Vector2 input)
    {
        Vector3 horizontal = transform.right * input.x + transform.forward * input.y;

        bool isCrouching = characterController != null && characterController.height <= (crouchHeight + 0.01f);
        float crouchMultiplier = isCrouching ? crouchSpeedMultiplier : 1f;
        float sprintMultiplier = (sprintAction != null && sprintAction.IsPressed()) ? sprintSpeedMultiplier : 1f;
        float speedMultiplier = crouchMultiplier * sprintMultiplier;
        return horizontal * speed * speedMultiplier;
    }

    private void HandleJump()
    {
        if (jumpAction != null && jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * Mathf.Abs(gravity));
            OnJump?.Invoke();
        }
    }

    private void HandleCrouch()
    {
        bool requestedCrouch = (crouchAction != null && crouchAction.IsPressed());
        bool shouldCrouch = requestedCrouch || !canUncrouch();

        float transitionSpeed = crouchTransitionSpeed * Time.deltaTime;
        characterController.height = Mathf.Lerp(characterController.height, shouldCrouch ? crouchHeight : standingHeight, transitionSpeed);
        characterController.stepOffset = Mathf.Lerp(characterController.stepOffset, shouldCrouch ? crouchStepHeight : uncrouchStepHeight, transitionSpeed);

        if (shouldCrouch != wasCrouching)
        {
            OnCrouch?.Invoke(shouldCrouch);
            wasCrouching = shouldCrouch;
        }
    }

    private bool canUncrouch()
    {
        if (characterController.height >= standingHeight - 0.01f)
            return true;

        Vector3 worldCenter = transform.TransformPoint(characterController.center);

        Vector3 start = worldCenter + Vector3.up * (characterController.height / 2f);
        Vector3 end = worldCenter + Vector3.up * (standingHeight / 2f);

        float radius = characterController.radius;
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

    private void HandleSprint()
    {
        bool isSprinting = (sprintAction != null && sprintAction.IsPressed());
        if (isSprinting != wasSprinting)
        {
            OnSprint?.Invoke(isSprinting);
            wasSprinting = isSprinting;
        }
    }

    private void HandleRotation()
    {
        if (cameraTransform == null)
        {
            return;
        }

        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        if (forward.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), rotationSpeed);
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    private void Move(Vector3 horizontalVelocity)
    {
        Vector3 total = horizontalVelocity + velocity;
        characterController.Move(total * Time.deltaTime);
    }
}
