using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Static instance accessible globally
    public static GameManager Instance;


    // Skills
    public static string[] skillNames;
    public static string[] skillDescription;
    // TODO Will be private just accessible in editor for the moment to facilitate debug
    public int skillPoints;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // TODO Does the game manager listen to the even or those the player send the info with the ref to game manager ?
    public void OnPlayerDeath(DeathData death)
    {
        if (!death.hasBeenAchieved){
            skillPoints++;
            death.hasBeenAchieved = true;
        }
    }

    // Takes no arguments as all no skills cost 1
    public void SkillPointDecrease() { skillPoints--; }
    public int GetSkillPoints() { return skillPoints; }

}
