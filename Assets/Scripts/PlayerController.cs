using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private const string MOVE_ACTION = "Move";
    private const string JUMP_ACTION = "Jump";

    [SerializeField] private float speed = 6.0f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;

    private CharacterController characterController;
    private PlayerInput playerInput;

    #region Input Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    #endregion

    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions.FindAction(MOVE_ACTION);
        jumpAction = playerInput.actions.FindAction(JUMP_ACTION);
    }

    private void Update()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        characterController.Move(move * speed * Time.deltaTime);
        if (jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
