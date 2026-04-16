using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MovieCreditsController : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI statsText;    // 统计和致谢的文本框
    public CanvasGroup canvasGroup;      // 整个 UI 的淡入淡出控制
    public GameObject returnButton;      // 返回按钮（初始设为隐藏）

    [Header("Movie Settings")]
    public float fadeSpeed = 0.5f;       // 淡入速度
    public float scrollSpeed = 40f;      // 文字向上飘的速度
    public float pauseBeforeScroll = 2f; // 战报出来后停顿多久再开始飘

    void Start()
    {
        // 1. 初始化状态
        canvasGroup.alpha = 0;
        if (returnButton != null) returnButton.SetActive(false);

        // 2. 检查数据并设置文本
        if (GameDataTracker.Instance == null)
        {
            Debug.LogError("未找到 GameDataTracker！");
            statsText.text = "Thanks for playing!";
        }
        else
        {
            SetupFinalText();
        }

        // 3. 启动电影序列
        StartCoroutine(PlayMovieSequence());
    }

    void SetupFinalText()
    {
        var data = GameDataTracker.Instance;
        string time = data.GetPlayTime();

        // 重新整理字符串，删除多余的重复块
        statsText.text =
            $"<line-height=150%>" +
            $"<size=180%><b>ALCHEMICAL SUMMARY</b></size>\n\n" +

            $"\n\n<size=120%>— THE JOURNEY ENDS HERE —</size>\n\n\n\n" +

            $"<size=160%><b>CREDITS</b></size>\n\n" +

            $"<size=130%><b>DEVELOPMENT</b></size>\n" +
            $"<b>Lead Producer & Programmer</b>\nJunYang\n\n" +
            $"<b>Lead Producer & Programmer</b>\nJeffLim\n\n" +
            $"<b>Game Mechanic Design</b>\nJunYang\n\n" +
            $"<b>VR Interaction & HCI</b>\nJeffLim\n\n" +

            $"\n<size=130%><b>ART & ATMOSPHERE</b></size>\n" +
            $"<b>Environment Design</b>\nJunYang\n\n" +
            $"<b>Visual Effects (VFX)</b>\nUnity Particle Systems\n\n" +
            $"<b>3D Asset Curation</b>\nUnity Asset Store Artists\n\n" +

            $"\n<size=130%><b>SPECIAL THANKS</b></size>\n" +
            $"<b>University Mentors</b>\nTan Bee Sian\n\n" +
            $"<b>The VR Lab Community</b>\nFor testing and feedback\n\n" +
    

            $"\n\n\n\n\n" +
            $"<size=220%><b>THANKS FOR PLAYING!</b></size>" +
            $"</line-height>";
    }

    IEnumerator PlayMovieSequence()
    {
        // A. 整体淡入
        yield return new WaitForSeconds(1f);
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // B. 停顿让玩家看清楚统计数据
        yield return new WaitForSeconds(pauseBeforeScroll);

        // --- 核心改动：在滚动开始的同时，启动一个倒计时显示按钮的任务 ---
        StartCoroutine(ShowButtonAfterDelay(3f));

        // C. 开始电影滚动
        float startY = statsText.transform.localPosition.y;
        float contentHeight = statsText.preferredHeight;
        float targetY = startY + contentHeight + 1000f;

        while (statsText.transform.localPosition.y < targetY)
        {
            statsText.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // 新增：专门负责倒计时显示按钮的协程
    IEnumerator ShowButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (returnButton != null)
        {
            returnButton.SetActive(true);
            // 顺便给按钮加个淡入效果（可选）
            CanvasGroup btnGroup = returnButton.GetComponent<CanvasGroup>();
            if (btnGroup != null)
            {
                btnGroup.alpha = 0;
                while (btnGroup.alpha < 1)
                {
                    btnGroup.alpha += Time.deltaTime * 2f;
                    yield return null;
                }
            }
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
}