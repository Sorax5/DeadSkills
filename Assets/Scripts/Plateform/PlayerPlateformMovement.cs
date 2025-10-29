using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerPlateformMovement : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    private CharacterController playerController;

    private void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag(playerTag))
       {
           playerController = other.GetComponent<CharacterController>();
           if (playerController != null)
           {
               playerController.transform.SetParent(this.transform);
           }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (playerController != null)
            {
                playerController.transform.SetParent(null);
                playerController = null;
            }
        }
    }
}
