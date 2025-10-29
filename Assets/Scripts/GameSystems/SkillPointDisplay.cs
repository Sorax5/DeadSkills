using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillPointDisplay : MonoBehaviour
{
    public TextMeshProUGUI skillPointDisplay;


    private void OnEnable()
    {
        if (skillPointDisplay == null)
            skillPointDisplay = GetComponentInChildren<TextMeshProUGUI>();

        if (skillPointDisplay != null && GameManager.Instance != null)
            UpdateSkillPointDisplay();
    }

    public void UpdateSkillPointDisplay(){ skillPointDisplay.text = GameManager.Instance.GetSkillPoints().ToString(); }
}
