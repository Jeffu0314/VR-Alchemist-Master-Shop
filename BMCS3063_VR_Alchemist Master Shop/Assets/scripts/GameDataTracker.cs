using UnityEngine;

public class GameDataTracker : MonoBehaviour
{
    public static GameDataTracker Instance;

    [Header("统计数据")]
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

    // --- 新增：停止计时功能 ---
    // 在切换到 EndScene 之前调用这个方法
    public void StopTrackingTime()
    {
        if (isTimerRunning)
        {
            finalTimeInSeconds = Time.time - startTime;
            isTimerRunning = false;
        }
    }

    // 获取游玩总时长（格式化为 分:秒）
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