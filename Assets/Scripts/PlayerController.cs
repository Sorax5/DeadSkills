using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private const string MOVE_ACTION = "Move";
    private const string JUMP_ACTION = "Jump";
    private const string CROUCH_ACTION = "Crouch";

    public event Action OnJump;
    public event Action OnCrouch;

    [Header("Movement")]
    [SerializeField] private float speed = 6.0f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float rotationSpeed = 0.1f;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchTransitionSpeed = 5.0f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;

    [Header("References")]
    [SerializeField] private GameObject rendererObject;
    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private PlayerInput playerInput;

    #region Input Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    #endregion

    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions.FindAction(MOVE_ACTION);
        jumpAction = playerInput.actions.FindAction(JUMP_ACTION);
        crouchAction = playerInput.actions.FindAction(CROUCH_ACTION);

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        ProcessGround();

        Vector2 input = ReadMoveInput();
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
        float speedMultiplier = (crouchAction != null && crouchAction.IsPressed()) ? crouchSpeedMultiplier : 1f;
        return horizontal * speed * speedMultiplier;
    }

    private void HandleJump()
    {
        if (jumpAction != null && jumpAction.triggered && isGrounded)
        {
            // Use kinematic formula to compute required initial vertical velocity
            velocity.y = Mathf.Sqrt(2f * jumpHeight * Mathf.Abs(gravity));
            OnJump?.Invoke();
        }
    }

    private void HandleCrouch()
    {
        bool shouldCrouch = (crouchAction != null && crouchAction.IsPressed());
        float transitionSpeed = crouchTransitionSpeed * Time.deltaTime;
        characterController.height = Mathf.Lerp(characterController.height, shouldCrouch ? crouchHeight : standingHeight, transitionSpeed);

        if (rendererObject != null)
        {
            rendererObject.transform.localScale = new Vector3(1, characterController.height / standingHeight, 1);
        }

        if (shouldCrouch)
        {
            OnCrouch?.Invoke();
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
