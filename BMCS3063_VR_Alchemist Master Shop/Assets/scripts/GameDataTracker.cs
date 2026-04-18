using UnityEngine;

public class GameDataTracker : MonoBehaviour
{
    public static GameDataTracker Instance;

    public int potionsMade = 0;
    public int customersServed = 0;

    private float startTime;
    private float finalTimeInSeconds = 0;
    private bool isTimerRunning = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            startTime = Time.time;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StopTrackingTime()
    {
        if (isTimerRunning)
        {
            finalTimeInSeconds = Time.time - startTime;
            isTimerRunning = false;
        }
    }

    public string GetPlayTime()
    {
        float totalDisplayTime;

        if (isTimerRunning)
        {
            totalDisplayTime = Time.time - startTime;
        }
        else
        {
            totalDisplayTime = finalTimeInSeconds;
        }

        int minutes = Mathf.FloorToInt(totalDisplayTime / 60F);
        int seconds = Mathf.FloorToInt(totalDisplayTime - (minutes * 60));
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}