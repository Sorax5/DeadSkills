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
