using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathTouch : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private GameEvent deathEvent;
    [SerializeField] private DeathData deathData;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision " + collision.gameObject.name);
        if (collision.gameObject.CompareTag(targetTag))
        {
            Debug.Log($"DeathTouch triggered for {deathData.deathName}");
            deathEvent.Raise(this, deathData);
        }
    }
}
