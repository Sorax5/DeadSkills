using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Static instance accessible globally
    public static GameManager Instance;


    // Skills
    public static string[] skillNames;
    public static string[] skillDescription;
    public int SkillPoints;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
