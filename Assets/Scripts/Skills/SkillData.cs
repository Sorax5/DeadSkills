using UnityEngine;

[CreateAssetMenu(menuName = "SkillData")]
public class SkillData : ScriptableObject
{
    public int skillID;
    public string skillName;
    public string skillDescription;

    public SkillData[] parentSkills;
    public GameEvent onSkillunlock;

}
