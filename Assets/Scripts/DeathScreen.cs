using TMPro;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{

    public DeathData deathData;
    public TextMeshPro deathName;
    public TextMeshPro deathDescription;
    void OnEnable()
    {
        deathName.text = deathData.deathName;
        deathDescription.text = deathData.deathDescription;        
    }
}
