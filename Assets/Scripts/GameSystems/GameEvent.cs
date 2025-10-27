using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvent")]
public class GameEvent : ScriptableObject
{
    //Liste des objets �coutant l'�v�nement
    public List<EventListener> listeners = new List<EventListener>();

    //Fonction a appel�e par les objets souhaitant lever l'�v�nement
    public void Raise(Component sender, object data)
    {
        //Boucle � travers tous les objets �coutant l'�v�nement
        //pour leur signaler que l'event a �t� lev�
        for (int i = 0; i < listeners.Count; i++)
        {
            listeners[i].OnEventRaised(sender, data);
        }
    }

    //Fonction permettant a un objet de s'enregistrer aupr�s de l'�v�nement
    public void RegisterListener(EventListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }
    //Fonction permettant a un objet de se d�senregistrer de l'�v�nement
    public void UnregisterListener(EventListener listener)
    {
        if (listeners.Contains(listener))
            listeners.Add(listener);
    }
}

