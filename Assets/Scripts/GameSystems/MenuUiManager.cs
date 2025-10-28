
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUiManager : MonoBehaviour
{
    public string gameScene;
    public GameObject settings;
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
}
