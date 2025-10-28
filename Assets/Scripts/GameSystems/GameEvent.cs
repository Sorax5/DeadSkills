using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvent")]
public class GameEvent : ScriptableObject
{
    // Liste des objets écoutant l'événement (non sérialisée pour éviter les refs persistantes entre Play/Stop)
    [System.NonSerialized]
    private readonly List<EventListener> listeners = new List<EventListener>();

    // Fonction appelée par les objets souhaitant lever l'événement
    public void Raise(Component sender, object data)
    {
        // Itérer à l'envers pour supporter (dés)inscriptions pendant la notification
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            var listener = listeners[i];
            if (listener != null)
            {
                listener.OnEventRaised(sender, data);
            }
        }
    }

    // Fonction permettant à un objet de s'enregistrer auprès de l'événement
    public void RegisterListener(EventListener listener)
    {
        if (listener == null) return;
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    // Fonction permettant à un objet de se désenregistrer de l'événement
    public void UnregisterListener(EventListener listener)
    {
        if (listener == null) return;
        listeners.Remove(listener);
    }
}

