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
        unlockCondition.text = skillData.linkedDeath.deathDescription;
        skill = skillData.skill;

        // TODO Change background color for the button based if the skill can be unlocked (no to be done in start though)

    }

    public void onSkillUnlock()
    {

        if (skillData.canBeUnlocked())
        {
            Debug.Log("Skill débloqué : " + skillData.skillName);
            unlockEvent.Raise(this, skill);
        }

    }

}
