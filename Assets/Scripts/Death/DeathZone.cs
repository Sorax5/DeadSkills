using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class DeathZone : MonoBehaviour
{
    public UnityEvent LocalPlayerDeath;

    [SerializeField] private GameEvent onPlayerDeath;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private DeathData Deaths;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !Deaths.hasBeenAchieved)
        {
            Deaths.hasBeenAchieved = true;
            Debug.Log("Player has died in DeathZone: " + Deaths.ToString());
            onPlayerDeath.Raise(this, Deaths);
            LocalPlayerDeath?.Invoke();
        }
    }
}
