
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUiManager : MonoBehaviour
{
    public string gameScene;
    public GameObject settings;
    public GameObject main;
    public void LoadGame()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void ShowSettings()
    {
        settings.SetActive(true);
    }

    public void HideSettings()
    {
        settings.SetActive(false);
    }

    public void ShowMain()
    {
        main.SetActive(true);
    }

    public void HideMain()
    {
        main.SetActive(false);
    }

    public void QuitFunc(){ Application.Quit(); }
}

