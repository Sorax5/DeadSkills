using System.Linq;
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

    // Skill color
    public Color lockedColor;
    public Color unlockedColor;
    public Color unlockableColor;

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
    private void OnEnable(){ UpdateUI(); }

    // Called when the Skill Tree is displayed or when an other skill is unlocked
    public void UpdateUI()
    {
        // Blue if skill unlocked, red if not unlockable, white if unlockable
        if (skillData.isUnlocked) 
            ChangeColor(unlockedColor);
        else if (!skillData.canBeUnlocked()) 
            ChangeColor(lockedColor);
        else 
            ChangeColor(unlockableColor);
        
    }

    private void ChangeColor(Color color){ image.color = color; }

    public void onSkillClicked()
    {
        // TODO Add the check that there's an available skill point (stored in game event, game event listen to death event when one that hasNotBeenAchieved yet is sent get a point
        // If the skill is unlockable and not yet unlocked
        if (skillData.canBeUnlocked() && !skillData.isUnlocked)
        {
            if (GameManager.Instance.GetSkillPoints() > 0){
                Debug.Log("Skill débloqué : " + skillData.skillName);
                GameManager.Instance.SkillPointDecrease();
                skillData.isUnlocked = true;
                unlockEvent.Raise(this, skill);

                // Update the UI
                ChangeColor(unlockedColor);
                Debug.Log("Ougah Bougah");
            }
 
        }

    }

}
