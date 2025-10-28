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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player has died in DeathZone: " + Deaths.ToString());
            LocalPlayerDeath?.Invoke();
            Debug.Log("Raising global death event.");
            deathEvent?.Raise(this, Deaths);
        }
    }
}
