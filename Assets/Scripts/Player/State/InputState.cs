using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InputState : IState
{
    public abstract string Name { get; }

    private readonly CharacterController controller;
    private readonly PlayerInput input;

    public Animator animator;


    public InputState(CharacterController controller, PlayerInput input)
    {
        this.controller = controller;
        this.input = input;
    }

    public CharacterController Controller => controller;
    public PlayerInput Input => input;

    public abstract void Enter();
    public abstract void Exit();
    public abstract void FixedUpdate();
    public abstract void Update();
}
