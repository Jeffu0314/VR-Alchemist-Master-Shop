using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUIController : MonoBehaviour
{
    public static VictoryUIController Instance;

    public GameObject victoryMenuObject; 

    void Awake()
    {
        Instance = this;
        
        if (victoryMenuObject != null) victoryMenuObject.SetActive(false);
    }

    public void ShowMenu()
    {
        if (victoryMenuObject != null)
        {
            
            victoryMenuObject.SetActive(true);
        }
    }

    public void OnClickContinue()
    {
        victoryMenuObject.SetActive(false);
        
    }

    public void OnClickEndGame()
    {
        PlayerPrefs.SetInt("LastRunScore", AlchemyManager.Instance.currentCoins);
        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
    }
}