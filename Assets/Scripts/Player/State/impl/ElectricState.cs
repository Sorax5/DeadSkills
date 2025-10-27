using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElectricState : InputState
{
    private DateTime enterTime;
    private TimeSpan duration;

    private float gravity;

    public ElectricState(CharacterController controller, PlayerInput input, TimeSpan duration, float gravity) : base(controller, input)
    {
        this.duration = duration;
        this.gravity = gravity;
    }

    public override string Name => "ELECTRIC";

    public override void Enter()
    {
        this.enterTime = DateTime.Now;
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
        Vector3 gravityVector = new Vector3(0, gravity * Time.deltaTime, 0);
        Controller.Move(gravityVector);
    }

    public bool IsDurationOver()
    {
        return (DateTime.Now - enterTime) >= duration;
    }
}
