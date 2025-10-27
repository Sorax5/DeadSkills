using UnityEngine;

public class SkillTree : MonoBehaviour
{
    // Static instance accessible globally
    public static SkillTree Instance;

    public Skill[] skillsList;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
