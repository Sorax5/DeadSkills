using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathZone : MonoBehaviour
{
    public event Action LocalPlayerDeath;

    [SerializeField] private GameEvent onPlayerDeath;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string deathName = "";

    private bool isAlreadyTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isAlreadyTriggered)
        {
            isAlreadyTriggered = true;
            Debug.Log("Player has died in DeathZone: " + deathName);
            onPlayerDeath.Raise(this, deathName);
            LocalPlayerDeath?.Invoke();
        }
    }
}
