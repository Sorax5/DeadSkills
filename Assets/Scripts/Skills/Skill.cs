using TMPro;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public SkillData skillData;

    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    private void Start()
    {
        title.text = skillData.skillName;
        description.text = skillData.skillDescription;
    }

}
