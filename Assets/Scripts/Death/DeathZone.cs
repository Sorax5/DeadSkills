using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class DeathZone : MonoBehaviour
{
    public UnityEvent LocalPlayerDeath;

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private DeathData Deaths;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !Deaths.hasBeenAchieved)
        {
            Debug.Log("Player has died in DeathZone: " + Deaths.ToString());
            LocalPlayerDeath?.Invoke();
            GameManager.Instance.OnPlayerDeath(this, Deaths);
        }
    }
}
