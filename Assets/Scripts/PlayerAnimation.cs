using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimation : MonoBehaviour
{
    [Header("Animation Reference")]
    [SerializeField] private GameObject rendererObject;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerController.OnJump += onPlayerJump;
        playerController.OnCrouch += onPlayerCrouch;
        playerController.OnSprint += onPlayerSprint;
    }

    private void Update()
    {
        if (rendererObject != null)
        {
            rendererObject.transform.localScale = new Vector3(1, playerController.Height / playerController.standingHeight, 1);
        }
    }

    private void onPlayerCrouch(bool isCrouching)
    {
    }

    private void onPlayerSprint(bool isSprinting)
    {
    }

    private void onPlayerJump()
    {
    }
}
