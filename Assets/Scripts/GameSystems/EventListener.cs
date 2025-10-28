using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }

public class EventListener : MonoBehaviour
{
    // L'event à écouter
    public GameEvent gameEvent;
    // La méthode à appeler quand l'event a lieu
    public CustomGameEvent response;

    // Quand l'objet est activé, s'enregistre auprès de l'événement
    private void OnEnable()
    {
        if (gameEvent != null)
        {
            gameEvent.RegisterListener(this);
        }
        else
        {
            Debug.LogWarning($"{nameof(EventListener)} sur {name} n'a pas de GameEvent assigné.", this);
        }
    }

    // Quand l'objet est désactivé, se désenregistre auprès de l'événement
    private void OnDisable()
    {
        if (gameEvent != null)
        {
            gameEvent.UnregisterListener(this);
        }
    }

    // Quand l'event est levé, appelle la fonction choisie en attribut
    public void OnEventRaised(Component sender, object data)
    {
        response?.Invoke(sender, data);
    }
}
