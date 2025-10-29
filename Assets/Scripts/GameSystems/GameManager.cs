using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject respawnAnchor;
    [SerializeField] private string playerTag = "Player";
    public Ui_manager ui_Manager;

    // Skills
    public static string[] skillNames;
    public static string[] skillDescription;
    // TODO Will be private just accessible in editor for the moment to facilitate debug
    public int skillPoints;

    public GameObject playerInstance;
    public float respawnTime;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        FetchPlayerInScene();
        TeleportPlayer(respawnAnchor.transform.position);
    }

    public void OnPlayerDeath(Component arg0, object arg1)
    {
        var deathData = arg1 as DeathData;
        Debug.Log("GameManager detected player death: " + deathData.deathName);

        if (!deathData.hasBeenAchieved)
        {
            skillPoints++;
            deathData.hasBeenAchieved = true;
        }

        if (playerInstance != null)
        {
            playerInstance.GetComponent<PlayerController>().SetRagdoll(true);
            ui_Manager.ShowDeathScreen(deathData);
            StartCoroutine(Respawn());
        }
        else
        {
            Debug.LogWarning("Player instance is null during respawn.");
        }

    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        playerInstance.GetComponent<PlayerController>().SetRagdoll(false);
        ui_Manager.HideDeathScreen();
        TeleportPlayer(respawnAnchor.transform.position);
        ui_Manager.showOrHideSkillMenu();
    }

    private void FetchPlayerInScene()
    {
        playerInstance = GameObject.FindGameObjectWithTag(playerTag);
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

    // Takes no arguments as all no skills cost 1
    public void SkillPointDecrease() { skillPoints--; }
    public int GetSkillPoints() { return skillPoints; }

}
