using System.Collections.Generic;
using UnityEditor;
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

#if UNITY_EDITOR
[InitializeOnLoad]
public static class SkillDataResetter
{
    static SkillDataResetter()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static Dictionary<SkillData, string> backups = new Dictionary<SkillData, string>();

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.EnteredPlayMode:
                backups.Clear();
                foreach (var skill in Resources.FindObjectsOfTypeAll<SkillData>())
                    backups[skill] = JsonUtility.ToJson(skill);
                break;

            case PlayModeStateChange.ExitingPlayMode:
                foreach (var kv in backups)
                {
                    var skill = kv.Key;
                    JsonUtility.FromJsonOverwrite(kv.Value, skill);
                    skill.isUnlocked = false;

                    EditorUtility.SetDirty(skill);
                }

                backups.Clear();
                AssetDatabase.SaveAssets();
                Debug.Log("🔁 Tous les SkillData ont été réinitialisés après le Play Mode.");
                break;
        }
    }
}
#endif
