using TMPro;
using UnityEngine;

public class SkillUIButton : MonoBehaviour
{
    public SkillData skillData;

    // UI Ref
    // TODO La version clean voudrait qu'on les récupère depuis le code
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public TextMeshProUGUI unlockCondition;

    // Skill Data
    public GameEvent unlockEvent;
    private Skills skill;

    private void Start()
    {
        // Fill the UI texts
        title.text = skillData.skillName;
        description.text = skillData.skillDescription;
        unlockCondition.text = skillData.unlock.deathDescription;
        skill = skillData.skill;
    }

    public void onSkillUnlock()
    {
        Debug.Log("Skill débloqué");
        unlockEvent.Raise(this, skill);
    }

}
