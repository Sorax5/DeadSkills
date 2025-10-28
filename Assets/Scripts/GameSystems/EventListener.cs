using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }
public class EventListener : MonoBehaviour
{
    //L'event � �couter
    public GameEvent gameEvent;
    //La m�thode a appele quand l'event a lieu
    public CustomGameEvent response;

    //Quand l'objet est cr��, s'enregistre auprs de l'�vnement
    private void OnEnable()
    {
        gameEvent.RegisterListener(this);
    }

    //Quand l'objet est d�truit, se d�senregistre auprs de l'�vnement
    private void OnDisable()
    {
        gameEvent.UnregisterListener(this);
    }

    //Quand l'event est lev�, appelle la fonction choisie en attribut
    public void OnEventRaised(Component sender, object data)
    {
        response.Invoke(sender, data);
    }
}
