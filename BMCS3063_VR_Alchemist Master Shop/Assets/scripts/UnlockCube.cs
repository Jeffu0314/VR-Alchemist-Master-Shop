using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class UnlockCube : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private float cooldown = 1.0f; // 冷却时间，防止抓取瞬间触发多次
    private float lastUnlockTime;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        // 监听抓取事件
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // 增加时间间隔判断，防止抖动触发
        if (Time.time < lastUnlockTime + cooldown) return;

        if (AlchemyManager.Instance != null)
        {
            Debug.Log("<color=cyan>[Testing Cube]</color> 拿起魔方：请求解锁下一个配方...");

            // 调用之前在 AlchemyManager 里写的自动寻找下一个的函数
            AlchemyManager.Instance.UnlockNextAvailable();

            lastUnlockTime = Time.time;
        }
    }
}