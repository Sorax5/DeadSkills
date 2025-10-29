using System;
using UnityEngine;
using UnityEngine.Events;

public class TimedDeathZone : MonoBehaviour
{
    public UnityEvent LocalPlayerDeath;
    public UnityEvent PlayEffect;

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private DeathData Deaths;
    [SerializeField] private GameEvent deathEvent;
    [SerializeField] private SkillData deathImmunity;
    [SerializeField] private float deathDelay = 3f;

    private GameObject playerInZone;

    private TimeSpan delay;
    private DateTime playerEnterTime;

    private void Start()
    {
        delay = TimeSpan.FromSeconds(deathDelay);
        playerEnterTime = DateTime.Now;
    }

    private void Update()
    {
        TimeSpan timeInZone = DateTime.Now - playerEnterTime;
        if (timeInZone >= delay)
        {
            if (playerInZone != null && (deathImmunity == null || !deathImmunity.isUnlocked))
            {
                Debug.Log("Player has died in TimedDeathZone: " + Deaths.ToString());
                LocalPlayerDeath?.Invoke();
                Debug.Log("Raising global death event.");
                deathEvent?.Raise(this, Deaths);
                playerInZone = null;
            }

            PlayEffect?.Invoke();
            playerEnterTime = DateTime.Now;
        }
        
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInZone = other.gameObject;
        }
    }
}
