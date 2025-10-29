using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";
    public UnityEvent OnEnterZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            OnEnterZone.Invoke();
        }
    }
}
