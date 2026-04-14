using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUIController : MonoBehaviour
{
    public static VictoryUIController Instance;

    [Header("UI 面板物体")]
    public GameObject victoryMenuObject; // 将你的 Canvas 下的 Panel 拖进来

    void Awake()
    {
        Instance = this;
        // 确保游戏开始时它是隐藏的
        if (victoryMenuObject != null) victoryMenuObject.SetActive(false);
    }

    public void ShowMenu()
    {
        if (victoryMenuObject != null)
        {
            // 仅仅激活显示，不移动位置
            victoryMenuObject.SetActive(true);
        }
    }

    // --- 按钮点击事件 ---

    // 玩家选择：继续游玩
    public void OnClickContinue()
    {
        victoryMenuObject.SetActive(false);
        Debug.Log("玩家选择继续游玩");
    }

    // 玩家选择：结束并去结算
    public void OnClickEndGame()
    {
        // 既然不去 EndScene 了，我们就直接存一下最高分（可选），然后回起点
        PlayerPrefs.SetInt("LastRunScore", AlchemyManager.Instance.currentCoins);
        PlayerPrefs.Save();

        // 直接加载 StartScene
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }
}