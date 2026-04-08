using UnityEngine;
using TMPro;
using System.Collections; // 必须引用，才能使用协程

public class NPCOrderUI : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject orderCanvas;
    public TextMeshProUGUI nameText;

    [Header("时间设置")]
    public float waitBeforeShow = 5.0f; // 显示前的等待时间
    public float displayDuration = 5.0f; // UI 显示的时长

    private void Awake()
    {
        // 确保一开始是隐藏的
        if (orderCanvas != null) orderCanvas.SetActive(false);
    }

    private void Start()
    {
        if (AlchemyManager.Instance != null)
        {
            // 1. 先把自己设为经理关注的 NPC
            AlchemyManager.Instance.currentNPCUI = this;

            // 2. 开启协程流程：等待 -> 分配订单并显示 -> 再等待 -> 消失
            StartCoroutine(OrderDisplayRoutine());
        }
    }

    // 核心逻辑：控制显示和消失的闹钟
    private IEnumerator OrderDisplayRoutine()
    {
        // --- 第一步：静默等待 ---
        // NPC 已经报到了，但此时 UI 还没开
        yield return new WaitForSeconds(waitBeforeShow);

        // --- 第二步：触发订单并显示 ---
        if (AlchemyManager.Instance != null)
        {
            // 此时才让经理分配订单，经理会反过来调用下方的 ShowOrder
            AlchemyManager.Instance.AssignRandomOrder();
        }

        // --- 第三步：保持显示状态 ---
        // UI 已经在 ShowOrder 里打开了，现在数 5 秒
        yield return new WaitForSeconds(displayDuration);

        // --- 第四步：自动隐藏 ---
        HideOrder();
        Debug.Log($"{gameObject.name} 的订单显示时间结束，已隐藏。");
    }

    // 由 AlchemyManager 的 AssignRandomOrder 自动调用
    public void ShowOrder(RecipeData recipe)
    {
        if (recipe == null || orderCanvas == null || nameText == null) return;

        nameText.text = "I need: " + recipe.potionName;
        orderCanvas.SetActive(true);
        Debug.Log($"{gameObject.name} 的订单 UI 开启。");
    }

    public void HideOrder()
    {
        if (orderCanvas != null) orderCanvas.SetActive(false);
    }
}