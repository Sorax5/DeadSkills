using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem electricityEffect;

    public void PlayerElectricityEffect()
    {
        if (electricityEffect != null)
        {
            electricityEffect.Play();
        }
    }
}
