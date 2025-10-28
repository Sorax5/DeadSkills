using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }

public class EventListener : MonoBehaviour
{
    // L'event � �couter
    public GameEvent gameEvent;
    // La m�thode � appeler quand l'event a lieu
    public CustomGameEvent response;

    // Quand l'objet est activ�, s'enregistre aupr�s de l'�v�nement
    private void OnEnable()
    {
        if (gameEvent != null)
        {
            gameEvent.RegisterListener(this);
        }
        else
        {
            Debug.LogWarning($"{nameof(EventListener)} sur {name} n'a pas de GameEvent assign�.", this);
        }
    }

    // Quand l'objet est d�sactiv�, se d�senregistre aupr�s de l'�v�nement
    private void OnDisable()
    {
        if (gameEvent != null)
        {
            gameEvent.UnregisterListener(this);
        }
    }

    // Quand l'event est lev�, appelle la fonction choisie en attribut
    public void OnEventRaised(Component sender, object data)
    {
        response?.Invoke(sender, data);
    }
}
