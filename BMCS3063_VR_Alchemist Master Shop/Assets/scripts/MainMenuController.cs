using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    void Start()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);  
        if (settingsPanel != null) settingsPanel.SetActive(false); 
    }

    public void LoadMainGame()
    {
        if (GetComponent<SceneFader>() != null)
        {
            GetComponent<SceneFader>().FadeToScene("MagicScene");
        }
        else
        {
            SceneManager.LoadScene("MagicScene");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void OpenSettings()
    {
        UISoundManager.Instance.PlayClick();

        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        UISoundManager.Instance.PlayClick();

        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}