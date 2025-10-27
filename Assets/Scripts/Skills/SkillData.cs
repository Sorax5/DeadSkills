using UnityEngine;

[CreateAssetMenu(menuName = "SkillData")]
public class SkillData : ScriptableObject
{
    // Enums
    public Skills skill;
    public DeathData unlock;

    public string skillName;
    public string skillDescription;
    public SkillData[] parentSkills;
}
