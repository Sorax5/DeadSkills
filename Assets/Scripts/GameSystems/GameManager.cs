using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Vector3 playerSpawnPosition = Vector3.zero;
    [SerializeField] private string playerTag = "Player";

    // Skills
    public static string[] skillNames;
    public static string[] skillDescription;
    // TODO Will be private just accessible in editor for the moment to facilitate debug
    public int skillPoints;

    public GameObject playerInstance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        FetchPlayerInScene();
    }

    public void OnPlayerDeath(Component arg0, DeathData deathData)
    {
        Debug.Log("GameManager detected player death: " + deathData.deathName);

        if (playerInstance != null)
        {
            TeleportPlayer(playerSpawnPosition);
        }
        else
        {
            Debug.LogWarning("Player instance is null during respawn.");
        }
        SkillPoints += 1;
    }

    private void FetchPlayerInScene()
    {
        playerInstance = GameObject.FindGameObjectWithTag(playerTag);
        if (playerInstance != null)
        {
            playerInstance.transform.position = playerSpawnPosition;
        }
        else
        {
            Debug.LogWarning("Player with tag '" + playerTag + "' not found in the scene.");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(playerSpawnPosition, 0.5f);

        if(playerInstance != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(playerSpawnPosition, playerInstance.transform.position);
        }
    }

    public void TeleportPlayer(Vector3 newPosition)
    {
        if (playerInstance != null)
        {
            CharacterController characterController = playerInstance.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
                playerInstance.transform.position = newPosition;
                characterController.enabled = true;
            }
            else
            {
                Debug.LogWarning("CharacterController component not found on player instance.");
            }

        }
        else
        {
            Debug.LogWarning("Player instance is null during teleportation.");
        }
    }

    // TODO Does the game manager listen to the even or those the player send the info with the ref to game manager ?
    public void OnPlayerDeath(DeathData death)
    {
        if (!death.hasBeenAchieved){
            skillPoints++;
            death.hasBeenAchieved = true;
        }
    }

    // Takes no arguments as all no skills cost 1
    public void SkillPointDecrease() { skillPoints--; }
    public int GetSkillPoints() { return skillPoints; }

}
