using UnityEngine;

public class LockMouse : MonoBehaviour
{
    public bool uiOpen = false; // toggle this when your UI menu is open

    void Update()
    {
        if (uiOpen)
        {
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Optional: toggle with Escape key for testing
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            uiOpen = !uiOpen;
    }
}
