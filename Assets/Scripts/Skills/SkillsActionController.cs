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
    }

    public void OnSkillUnlocked(int id)
    {
        Debug.Log("Skill unlocked: " + id);
        SetActionActive(id, true);
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

    public void SetActionActive(int identifier, bool active)
    {
        foreach (var action in actions)
        {
            if (action.Identifier == identifier)
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
        foreach (var action in actions)
        {
            if (action.IsActive)
            {
                action.Update();
            }
        }
    }
}
