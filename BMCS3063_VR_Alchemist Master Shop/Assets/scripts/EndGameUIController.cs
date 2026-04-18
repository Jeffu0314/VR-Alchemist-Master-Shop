using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class MovieCreditsController : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI statsText;   
    public CanvasGroup canvasGroup;     
    public GameObject returnButton;     

    [Header("Movie Settings")]
    public float fadeSpeed = 0.5f;       
    public float scrollSpeed = 40f;    
    public float pauseBeforeScroll = 2f; 

    void Start()
    {
        canvasGroup.alpha = 0;
        if (returnButton != null) returnButton.SetActive(false);

        if (GameDataTracker.Instance == null)
        {
            statsText.text = "Thanks for playing!";
        }
        else
        {
            SetupFinalText();
        }

        StartCoroutine(PlayMovieSequence());
    }

    void SetupFinalText()
    {
        var data = GameDataTracker.Instance;
        string time = data.GetPlayTime();

        statsText.text =
            $"<line-height=150%>" +
            $"<size=180%><b>ALCHEMICAL SUMMARY</b></size>\n\n" +

            $"\n\n<size=120%>¡ª THE JOURNEY ENDS HERE ¡ª</size>\n\n\n\n" +

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
        yield return new WaitForSeconds(1f);
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        yield return new WaitForSeconds(pauseBeforeScroll);

        StartCoroutine(ShowButtonAfterDelay(3f));

        float startY = statsText.transform.localPosition.y;
        float contentHeight = statsText.preferredHeight;
        float targetY = startY + contentHeight + 1000f;

        while (statsText.transform.localPosition.y < targetY)
        {
            statsText.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator ShowButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (returnButton != null)
        {
            returnButton.SetActive(true);
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