using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // 载入游戏主场景
    public void LoadMainGame()
    {
        // 确保引号里的名字和你主场景的文件名完全一致
        SceneManager.LoadScene("MagicScene");
    }

    // 退出游戏
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}