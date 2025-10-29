using UnityEngine;

[CreateAssetMenu(menuName = "SkillData")]
public class SkillData : ScriptableObject
{
    // Enums
    public Skills skill;
    public DeathData linkedDeath;

    // TODO Add an image ? Image that will be used in the UIButton
    public string skillName;
    public string skillDescription;
    public SkillData[] parentSkills;
    public bool isUnlocked;
    public bool gameJamLocked;

    public bool canBeUnlocked()
    {
        // If player has not achieved the corresponding death, it can't be unlocked
        if (!linkedDeath.hasBeenAchieved)
            return false;

        // If the player has not unlocked all parent skills, it can't be unlocked
        foreach (SkillData skill in parentSkills)
            if (!skill.isUnlocked)
                return false;

        // Else the skill can be unlocked
        return true;
    }
}
