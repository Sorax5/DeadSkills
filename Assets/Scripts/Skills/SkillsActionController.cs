using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class SkillsActionController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float Speed = 6.0f;
    [SerializeField] public float Gravity = -9.81f;
    [SerializeField] public float JumpHeight = 1.5f;
    [SerializeField] public float RotationSpeed = 0.1f;

    [Header("Crouch")]
    [SerializeField] public float CrouchHeight = 1.0f;
    [SerializeField] public float StandingHeight = 2.0f;

    [SerializeField] public float CrouchStepHeight = 0.3f;
    [SerializeField] public float UncrouchStepHeight = 0.5f;

    [SerializeField] public float CrouchTransitionSpeed = 5.0f;
    [SerializeField] public float CrouchSpeedMultiplier = 0.5f;

    [Header("Sprint")]
    [SerializeField] public float SprintSpeedMultiplier = 1.5f;

    [Header("Slide")]
    [SerializeField] public float SlideDuration = 0.8f;
    [SerializeField] public float SlideSpeedMultiplier = 1.8f;
    [SerializeField] public float SlideCooldown = 0.5f;

    [Header("References")]
    [SerializeField] public Transform cameraTransform;

    private List<SkillAction> actions = new List<SkillAction>();

    public CharacterController CharacterController { get; private set; }
    public PlayerInput PlayerInput { get; private set; }

    public SpeedModifier SpeedModifiers = new SpeedModifier();
    public Vector3 velocity;

    // Indicates whether a slide is currently active
    public bool IsSliding { get; set; } = false;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        PlayerInput = GetComponent<PlayerInput>();
        actions.Add(new MoveSkill(this, PlayerInput.actions["Move"]));
        actions.Add(new CrouchSkill(this, PlayerInput.actions["Crouch"]));
        actions.Add(new JumpSkill(this, PlayerInput.actions["Jump"]));
        actions.Add(new SprintSkill(this, PlayerInput.actions["Sprint"]));
        actions.Add(new SlideSkill(this, PlayerInput.actions["Crouch"], PlayerInput.actions["Sprint"]));

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    public void RegisterAction(SkillAction action)
    {
        if (!actions.Contains(action))
        {
            actions.Add(action);
        }
    }

    public void UnregisterAction(SkillAction action)
    {
        if (actions.Contains(action))
        {
            actions.Remove(action);
        }
    }

    // New helper to enable/disable actions by their identifier
    public void SetActionActive(string identifier, bool active)
    {
        foreach (var action in actions)
        {
            if (action.Identifier == identifier)
            {
                action.IsActive = active;
            }
        }
    }

    private void Update()
    {
        HandleRotation();
        foreach (var action in actions)
        {
            if (action.IsActive)
            {
                action.Update();
            }
        }
    }

    private void HandleRotation()
    {
        if (cameraTransform == null)
        {
            return;
        }

        ProcessGround();
        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        if (forward.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), RotationSpeed);
        }

        velocity.y += Gravity * Time.deltaTime;
        CharacterController.Move(velocity * SpeedModifiers.ApplyAllModifiers() * Time.deltaTime);
    }

    private void ProcessGround()
    {
        if (CharacterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
}
