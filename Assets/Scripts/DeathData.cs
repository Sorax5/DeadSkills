using UnityEngine;

[CreateAssetMenu(menuName = "DeathData")]
public class DeathData : ScriptableObject
{

    public Deaths death;
    public string deathName;
    public string deathDescription;
    private bool done = false;
}
