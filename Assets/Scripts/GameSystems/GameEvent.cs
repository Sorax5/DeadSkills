using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvent")]
public class GameEvent : ScriptableObject
{
    // Liste des objets �coutant l'�v�nement (non s�rialis�e pour �viter les refs persistantes entre Play/Stop)
    [System.NonSerialized]
    private readonly List<EventListener> listeners = new List<EventListener>();

    // Fonction appel�e par les objets souhaitant lever l'�v�nement
    public void Raise(Component sender, object data)
    {
        // It�rer � l'envers pour supporter (d�s)inscriptions pendant la notification
        // Using for loop instead of foreach to avoid garbage allocation
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            var listener = listeners[i];
            if (listener != null)
            {
                listener.OnEventRaised(sender, data);
            }
        }
    }

    // Fonction permettant � un objet de s'enregistrer aupr�s de l'�v�nement
    public void RegisterListener(EventListener listener)
    {
        if (listener == null) return;
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    // Fonction permettant � un objet de se d�senregistrer de l'�v�nement
    public void UnregisterListener(EventListener listener)
    {
        if (listener == null) return;
        listeners.Remove(listener);
    }
}

