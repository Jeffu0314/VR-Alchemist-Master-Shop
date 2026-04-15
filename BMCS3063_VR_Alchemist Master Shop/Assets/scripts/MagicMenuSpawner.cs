using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MagicMenuSpawner : MonoBehaviour
{
    public CanvasGroup menuCanvasGroup; // 拖入你的 World Space Canvas
    public ParticleSystem spawnParticles;  // 拖入 MenuSpawnEffect
    public float startDelay = 1.0f;    // 场景开始后多久启动魔法
    public float fadeDuration = 1.5f;  // 文字淡入的持续时间

    void Start()
    {
        // 初始确保菜单不可见，且粒子未播放
        menuCanvasGroup.alpha = 0;
        menuCanvasGroup.interactable = false; // 防止透明时被误点
        menuCanvasGroup.blocksRaycasts = false;

        // 启动魔法仪式协程
        StartCoroutine(SpawnMenuRoutine());
    }

    IEnumerator SpawnMenuRoutine()
    {
        // 1. 静谧期
        yield return new WaitForSeconds(startDelay);

        // 2. 触发期：爆发魔法粒子
        Debug.Log("魔法启动：粒子爆发");
        spawnParticles.Play();

        // 可选：在这里播放一个短促的“Bling”魔法音效

        // 等待粒子爆发的最亮时刻（约 0.5 秒）
        yield return new WaitForSeconds(0.5f);

        // 3. 呈现期：文字淡入
        Debug.Log("菜单呈现：文字淡入");
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            // 线性插值计算透明度 (从 0 到 1)
            menuCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        // 最终确保全亮并开启交互
        menuCanvasGroup.alpha = 1;
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;
    }
}