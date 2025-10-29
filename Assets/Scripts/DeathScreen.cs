using TMPro;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{

    public DeathData deathData;
    public TextMeshProUGUI deathName;
    public TextMeshProUGUI deathDescription;
    void OnEnable()
    {
        Debug.Log("DEATH DATA : " + deathData);
        deathName.text = deathData.deathName;
        deathDescription.text = deathData.deathDescription;        
    }
}
