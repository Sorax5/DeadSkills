using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class DeathZone : MonoBehaviour
{
    public UnityEvent LocalPlayerDeath;

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private DeathData Deaths;  
    [SerializeField] private GameEvent deathEvent;
    [SerializeField] private SkillData deathImmunity;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // Early return if immune to the death
            if (deathImmunity != null && deathImmunity.isUnlocked)
                return;

            Debug.Log("Player has died in DeathZone: " + Deaths.ToString());
            LocalPlayerDeath?.Invoke();
            Debug.Log("Raising global death event.");
            deathEvent?.Raise(this, Deaths);
        }
    }
}
