using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ui_manager : MonoBehaviour
{
    InputSystem_Actions inputSystem;
    public GameObject pauseMenu;
    public GameObject skillTreeMenu;
    public GameObject settingsMenu;
    public GameObject buttonCloseAll;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputSystem = new InputSystem_Actions();
        inputSystem.UI.Enable();
        inputSystem.UI.Pause.performed += showOrHidePauseMenu;
        inputSystem.UI.SkillTree.performed += showOrHideSkillMenu;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Time.timeScale);
    }

    void showOrHidePauseMenu(InputAction.CallbackContext context)
    {
        activeOrDesactive(pauseMenu);
    }

    void showOrHideSkillMenu(InputAction.CallbackContext context)
    {
        if (!pauseMenu.activeInHierarchy)
        {
            activeOrDesactive(skillTreeMenu);
        }
    }

    public void showOrHidePauseMenu()
    {
        activeOrDesactive(pauseMenu);
    }

    public void showOrHideSettings()
    {
        Debug.Log("jhdi  vdivd");
        activeOrDesactive(settingsMenu);
    }

    void activeOrDesactive(GameObject lala)
    {
        if (lala.activeInHierarchy)
        {
            hideAll();
        }
        else
        {
            hideAll();
            lala.SetActive(true);
            buttonCloseAll.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    
    public void hideAll()
    {
        if (settingsMenu.activeInHierarchy)
        {
            settingsMenu.SetActive(false);
            showOrHidePauseMenu();
        }
        else
        {
            skillTreeMenu.SetActive(false);
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(false);
            buttonCloseAll.SetActive(false);
            Debug.Log("lalalalala");
            Time.timeScale = 1f;
        }
    }
}
