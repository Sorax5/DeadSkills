using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightningSkill : SkillAction
{
    private bool isActive = true;
    private InputAction lightningAction;

    public LightningSkill(SkillsActionController controller, InputAction lightningAction) : base(controller)
    {
        this.lightningAction = lightningAction;
    }

    public override string Identifier => "lightning";

    public override bool IsActive { get { return isActive; } set { isActive = value; } }

    public override void Update()
    {
        if (lightningAction != null && lightningAction.WasPressedThisFrame() && !Controller.ElectrifyController.IsCurrentlyElectrified)
        {
            Controller.ElectrifyController.EnableElectrify();
        }
    }
}
