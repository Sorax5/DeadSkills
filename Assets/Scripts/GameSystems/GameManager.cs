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
    public int SkillPoints;

    public GameObject playerInstance;
    public EventListener onPlayerDeathListener;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        FetchPlayerInScene();
        onPlayerDeathListener.response.AddListener(OnPlayerDeath);
    }

    private void OnPlayerDeath(Component arg0, object arg1)
    {
        var deathData = arg1 as DeathData;
        Debug.Log("GameManager detected player death: " + deathData.deathName);
        playerInstance.transform.position = playerSpawnPosition;
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
    }
}
