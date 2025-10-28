using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvent")]
public class GameEvent : ScriptableObject
{
    //Liste des objets écoutant l'événement
    public List<EventListener> listeners = new List<EventListener>();

    //Fonction a appelée par les objets souhaitant lever l'événement
    public void Raise(Component sender, object data)
    {
        //Boucle à travers tous les objets écoutant l'événement
        //pour leur signaler que l'event a été levé
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].OnEventRaised(sender, data);
        }
    }

    //Fonction permettant a un objet de s'enregistrer auprès de l'événement
    public void RegisterListener(EventListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }
    //Fonction permettant a un objet de se désenregistrer de l'événement
    public void UnregisterListener(EventListener listener)
    {
        if (listeners.Contains(listener))
            listeners.Add(listener);
    }
}

