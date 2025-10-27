using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIButton : MonoBehaviour
{
    public SkillData skillData;

    // UI Ref
    // TODO La version clean voudrait qu'on les récupère depuis le code
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public TextMeshProUGUI unlockCondition;
    public Image image;


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
    }

    // On displaying the Skill Tree check which skill can be unlocked
    private void OnEnable()
    {
        canBeUnlocked();
    }

    // Called when the Skill Tree is displayed or when an other skill is unlocked
    public void canBeUnlocked()
    {
        if (skillData.isUnlocked)
            return;

        // Really not optimised but I don't have the time + I'm tired
        // TODO Could put the background to red instead 
        if (!skillData.canBeUnlocked()){
            Debug.Log("Je susi pas déblocable");
            // Decrease the alpha of the skill because it's not unlockable (no skill are unlockable at the start)
            Color color = image.color;
            color.a = .5f;
            image.color = color;
        }
        else {
            Debug.Log("Je susi déblocable");

            // Increase the alpha of the skill because it's not unlockable (no skill are unlockable at the start)
            Color color = image.color;
            color.a = 1f;
            image.color = color;
        }

    }

    public void onSkillClicked()
    {
        // TODO Add the check that there's an available skill point (stored in game event, game event listen to death event when one that hasNotBeenAchieved yet is sent get a point
        // If the skill is unlockable and not yet unlocked
        if (skillData.canBeUnlocked() && !skillData.isUnlocked)
        {
            Debug.Log("Skill débloqué : " + skillData.skillName);
            skillData.isUnlocked = true;
            unlockEvent.Raise(this, skill);

            //TODO Change the color of the unlocked button
        }

    }

}
