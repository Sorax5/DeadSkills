using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    public int skillID;
    public string skillName;
    public string skillDescription;

    public SkillData[] parentSkills;

}
