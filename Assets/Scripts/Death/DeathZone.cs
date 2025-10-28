using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class DeathZone : MonoBehaviour
{
    public UnityEvent LocalPlayerDeath;

    [SerializeField] private string playerTag = "Player";
    public DeathData Deaths;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player has died in DeathZone: " + Deaths.ToString());
            LocalPlayerDeath?.Invoke();
            GameManager.Instance.OnPlayerDeath(this, Deaths);
        }
    }
}
