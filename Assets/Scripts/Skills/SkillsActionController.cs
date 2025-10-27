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

    [Header("References")]
    [SerializeField] public Transform CameraTransform;
    [SerializeField] public ElectrifyController ElectrifyController;
    [SerializeField] public PlayerEffect PlayerEffect;

    private List<SkillAction> actions = new List<SkillAction>();

    public CharacterController CharacterController { get; private set; }
    public PlayerInput PlayerInput { get; private set; }

    public SpeedModifier SpeedModifiers = new SpeedModifier();
    public Vector3 velocity;

    public bool IsSliding { get; set; } = false;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        PlayerInput = GetComponent<PlayerInput>();
        actions.Add(new MoveSkill(this, PlayerInput.actions["Move"]));
        actions.Add(new CrouchSkill(this, PlayerInput.actions["Crouch"]));
        actions.Add(new JumpSkill(this, PlayerInput.actions["Jump"]));
        actions.Add(new SprintSkill(this, PlayerInput.actions["Sprint"]));
        actions.Add(new LightningSkill(this, PlayerInput.actions["Attack"]));

        if (CameraTransform == null && Camera.main != null)
        {
            CameraTransform = Camera.main.transform;
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
        if (CameraTransform == null)
        {
            return;
        }

        ProcessGround();
        Vector3 forward = CameraTransform.forward;
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
