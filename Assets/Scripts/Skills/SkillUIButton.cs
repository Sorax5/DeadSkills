using TMPro;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public SkillData skillData;

    // UI Ref
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    // Skill Data
    private GameEvent unlockEvent;
    private int skillID;

    private void Start()
    {
        title.text = skillData.skillName;
        description.text = skillData.skillDescription;
    }

    public void onSkillUnlock()
    {
        unlockEvent.Raise(this, skillID);
    }

}
