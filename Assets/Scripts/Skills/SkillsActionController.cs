using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class SkillsActionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public ElectrifyController ElectrifyController;
    [SerializeField] public PlayerEffect PlayerEffect;
    [SerializeField] public PlayerController PlayerController;

    private List<SkillAction> actions = new List<SkillAction>();

    public CharacterController CharacterController { get; private set; }
    public PlayerInput PlayerInput { get; private set; }

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
        PlayerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        RegisterAction(new LightningSkill(this, PlayerInput.actions["Attack"]));
        RegisterAction(new CrouchSkill(this));
        RegisterAction(new LongJumpSkill(this));
        RegisterAction(new SprintSkill(this));
    }

    public void OnSkillUnlocked(Component arg0, object arg1)
    {
        if (arg1 is Skills skill)
        {
            SetActionActive(skill, true);
        }
    }

    public void RegisterAction(SkillAction action)
    {
        if (!actions.Contains(action))
        {
            actions.Add(action);
            if (action.IsActive)
            {
                action.Start();
            }
        }
    }

    public void UnregisterAction(SkillAction action)
    {
        if (actions.Contains(action))
        {
            actions.Remove(action);
        }
    }

    public void SetActionActive(Skills identifier, bool active)
    {
        // Use for loop instead of foreach to avoid garbage allocation
        for (int i = 0; i < actions.Count; i++)
        {
            var action = actions[i];
            if (action.Identifier.Equals(identifier))
            {
                action.IsActive = active;
                if (active)
                {
                    action.Start();
                }
            }
        }
    }

    private void Update()
    {
        // Use for loop instead of foreach to avoid garbage allocation
        for (int i = 0; i < actions.Count; i++)
        {
            var action = actions[i];
            if (action.IsActive)
            {
                action.Update();
            }
        }
    }
}
