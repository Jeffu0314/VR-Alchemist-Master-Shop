using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Text;

[RequireComponent(typeof(XRGrabInteractable))]
public class InventoryBook : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject uiCanvas;         // 书上的 Canvas
    public TextMeshProUGUI contentText;  // 显示文字的 TMP

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        // 初始确保是关闭的
        if (uiCanvas != null) uiCanvas.SetActive(false);
    }

    void OnEnable()
    {
        // 核心：绑定 XRI 的抓取和放开事件
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    // 当抓起书本时
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(true);
            UpdateInventoryDisplay(); // 开启时立即刷新一次
        }
    }

    // 当放下书本时
    private void OnReleased(SelectExitEventArgs args)
    {
        if (uiCanvas != null) uiCanvas.SetActive(false);
    }

    void Update()
    {
        // 只要 UI 是开着的，就持续更新数值
        if (uiCanvas != null && uiCanvas.activeSelf)
        {
            UpdateInventoryDisplay();
        }
    }

    void UpdateInventoryDisplay()
    {
        if (AlchemyManager.Instance == null || contentText == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<size=130%><b>📖 库存清单</b></size>");
        sb.AppendLine("__________________");

        // 遍历 Manager 里的字典
        foreach (var item in AlchemyManager.Instance.ingredientInventory)
        {
            // 如果小于 5 就显示红色，否则白色
            string color = item.Value <= 5 ? "#FF4444" : "#FFFFFF";
            sb.AppendLine($"<color={color}>{item.Key}: {item.Value:F0}</color>");
        }

        contentText.text = sb.ToString();
    }
}