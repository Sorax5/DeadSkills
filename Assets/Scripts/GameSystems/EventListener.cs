using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }
public class EventListener : MonoBehaviour
{
    //L'event à écouter
    public GameEvent gameEvent;
    //La méthode a appele quand l'event a lieu
    public CustomGameEvent response;

    //Quand l'objet est créé, s'enregistre auprs de l'évnement
    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    //Quand l'objet est détruit, se désenregistre auprs de l'évnement
    private void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }

    //Quand l'event est levé, appelle la fonction choisie en attribut
    public void OnEventRaised(Component sender, object data)
    {
        response.Invoke(sender, data);
    }
}
