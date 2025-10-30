using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "DeathData")]
public class DeathData : ScriptableObject
{

    public Deaths death;
    public string deathName;
    public string deathDescription;
    // TODO Would be best if it was private and modified only for 'any' but don't have the time for that
    public bool hasBeenAchieved = false;
}

#if UNITY_EDITOR
[InitializeOnLoad]
public static class DeathDataResetter
{
    static DeathDataResetter()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static Dictionary<DeathData, string> backups = new Dictionary<DeathData, string>();

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.EnteredPlayMode:
                backups.Clear();
                foreach (var skill in Resources.FindObjectsOfTypeAll<DeathData>())
                    backups[skill] = JsonUtility.ToJson(skill);
                break;

            case PlayModeStateChange.ExitingPlayMode:
                foreach (var kv in backups)
                {
                    var skill = kv.Key;
                    JsonUtility.FromJsonOverwrite(kv.Value, skill);
                    skill.hasBeenAchieved = false;

                    EditorUtility.SetDirty(skill);
                }

                backups.Clear();
                AssetDatabase.SaveAssets();
                Debug.Log("🔁 Tous les DeathData ont été réinitialisés après le Play Mode.");
                break;
        }
    }
}
#endif
