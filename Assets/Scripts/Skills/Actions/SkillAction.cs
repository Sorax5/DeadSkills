using UnityEngine;

public abstract class SkillAction
{
    public SkillsActionController Controller;

    public SkillAction(SkillsActionController controller)
    {
        this.Controller = controller;
    }

    public abstract string Identifier { get; }
    public abstract bool IsActive { get; set; }

    public abstract void Update();
}
