using UnityEngine;
using TMPro;
using System.Collections; // 必须加上，因为要用协程

public class AreaUnlocker : MonoBehaviour
{
    [Header("解锁设置")]
    public int unlockPrice = 1000;      // 价格
    public GameObject blockerObject;   // 阻挡玩家的物理墙（比如门）

    [Header("黑暗视觉特效 (必须填入)")]
    // 将刚才创建的 Dark_Visual (包含粒子系统和光的物体) 拖到这里
    public GameObject darknessVisualRoot;
    public float dissolveTime = 2.5f;   // 黑暗消散的时间（秒）

    [Header("UI 显示")]
    public TextMeshPro priceText;      // 价格文字

    private bool isUnlocked = false;

    void Start()
    {
        if (priceText != null)
        {
            priceText.text = $"Cost: {unlockPrice} Gold";
            // 钱不够时，文字也显示诡异的暗红色
            priceText.color = new Color(0.5f, 0f, 0f, 1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isUnlocked)
        {
            TryUnlock();
        }
    }

    public void TryUnlock()
    {
        if (AlchemyManager.Instance != null && AlchemyManager.Instance.SpendCoins(unlockPrice))
        {
            PerformUnlock();
        }
        else
        {
            // 触发一个“钱不够”的声音或 UI 提示
            if (priceText != null) StartCoroutine(FlashPriceRed());
        }
    }

    void PerformUnlock()
    {
        isUnlocked = true;

        Debug.Log($"<color=green>区域已解锁！</color> 花费了 {unlockPrice} 金币");

        // 1. 移除阻挡物
        if (blockerObject != null)
        {
            // 你可以选直接销毁，或者执行一个开门的动画
            blockerObject.SetActive(false);
        }

        // 2. 核心修正：执行黑暗消散协程
        if (darknessVisualRoot != null)
        {
            StartCoroutine(DissolveDarknessRoutine());
        }

        // 3. 隐藏价格文字
        if (priceText != null) priceText.gameObject.SetActive(false);

        // 禁用掉这个触发器
        this.enabled = false;
    }

    // --- 优化：黑暗消散协程 ---
    private IEnumerator DissolveDarknessRoutine()
    {
        // 获取根物体下所有的粒子系统
        ParticleSystem[] particleSystems = darknessVisualRoot.GetComponentsInChildren<ParticleSystem>();
        // 获取根物体下所有的光
        Light[] lights = darknessVisualRoot.GetComponentsInChildren<Light>();

        // 1. 停止粒子发射（让现有的粒子慢慢飘散死亡）
        foreach (var ps in particleSystems)
        {
            var emission = ps.emission;
            emission.enabled = false;
        }

        // 2. 慢慢淡出光照
        float elapsed = 0;
        while (elapsed < dissolveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dissolveTime;

            foreach (var l in lights)
            {
                // 光的强度随时间线性下降到 0
                l.intensity = Mathf.Lerp(l.intensity, 0f, t);
            }
            yield return null;
        }

        // 3. 彻底彻底删除特效物体
        Destroy(darknessVisualRoot);
    }

    private IEnumerator FlashPriceRed()
    {
        if (priceText == null) yield break;
        priceText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        priceText.color = new Color(0.5f, 0f, 0f, 1f);
    }
}