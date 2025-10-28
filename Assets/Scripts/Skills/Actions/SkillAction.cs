using UnityEngine;

public abstract class SkillAction
{
    public SkillsActionController Controller;

    public SkillAction(SkillsActionController controller)
    {
        this.Controller = controller;
    }

    public abstract int Identifier { get; }
    public abstract bool IsActive { get; set; }

    public abstract void Start();
    public abstract void Update();
}
