using UnityEngine.InputSystem;

public class LightningSkill : SkillAction
{
    private bool isActive = false;
    private InputAction lightningAction;
    private ElectricState electricState;

    public LightningSkill(SkillsActionController controller, InputAction lightningAction) : base(controller)
    {
        this.lightningAction = lightningAction;
    }

    public override Skills Identifier => Skills.LetThereBeLight;

    public override bool IsActive { get { return isActive; } set { isActive = value; } }

    public override void Start()
    {
        StateMachine stateMachine = Controller.PlayerController.stateMachine;
        this.electricState = new ElectricState(Controller.CharacterController, Controller.PlayerInput, System.TimeSpan.FromSeconds(2), Controller.PlayerController.Gravity);
        stateMachine.AddState(electricState);

        stateMachine.AddTransition(electricState.Name, "IDLE", () => electricState.IsDurationOver());
    }

    public override void Update()
    {
        if (lightningAction != null && lightningAction.WasPressedThisFrame() && electricState != null)
        {
            StateMachine stateMachine = Controller.PlayerController.stateMachine;
            stateMachine.SetState(electricState);
            Controller.PlayerEffect.PlayerElectricityEffect();
        }
    }
}
