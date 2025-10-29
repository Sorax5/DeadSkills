using System.Collections;
using UnityEngine;

public class InteractZone : MonoBehaviour
{
    public GameObject deathZone;
    private InputSystem_Actions inputActions;

    void Start()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
    }
    void OnTriggerStay(Collider collision)
    {
        Debug.Log("lelelele"); 
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("kikiikikiki");
            if (inputActions.Player.Interact.IsPressed())
            {
                Debug.Log("yohohoho");
                deathZone.SetActive(true);
                StartCoroutine(Desactive());
            }
        }
    }

    IEnumerator Desactive()
    {
        yield return new WaitForSeconds(1);
        deathZone.SetActive(false);
    }
}
