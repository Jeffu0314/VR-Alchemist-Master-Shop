using UnityEngine;
using TMPro;
using System.Collections;

public class CatOrderDisplay : MonoBehaviour
{
    [Header("UI 绑定")]
    public TextMeshProUGUI orderText;

    [Header("颜色设置")]
    public Color activeOrderColor = Color.black;
    public Color waitingColor = Color.gray;
    public Color successColor = Color.green;

    private bool isShowingSuccess = false; // 成功反馈锁
    private int completedOrderCount = 0;   // 已完成订单总数
    private object lastServedOrder = null; // 记录上一个刚交货的订单对象

    void Start()
    {
        if (orderText == null)
        {
            Debug.LogError("CatOrderDisplay: 请在 Inspector 中绑定 OrderText!");
            enabled = false;
        }
    }

    void Update()
    {
        // 如果正在显示 3 秒的“成功”特效，不执行订单逻辑更新
        if (isShowingSuccess) return;

        UpdateCatDisplay();
    }

    void UpdateCatDisplay()
    {
        if (AlchemyManager.Instance == null) return;

        // 获取当前经理手里的订单
        var currentOrder = AlchemyManager.Instance.currentCustomerOrder;

        // --- 核心逻辑判断 ---
        // 只有当：1. 有新订单 2. 且这个订单不是刚才已经付过钱的那个旧订单
        if (currentOrder != null && currentOrder != lastServedOrder)
        {
            // 显示新订单详情
            orderText.text = $"<size=120%><b>ORDER {completedOrderCount + 1}</b></size>\n\n" +
                             $"Need: <color=purple>{currentOrder.potionName}</color>\n" +
                             $"Reward: <color=yellow>{currentOrder.potionPrice}</color> Gold";
            orderText.color = activeOrderColor;
        }
        else
        {
            // 如果订单还没刷新（NPC还没来，或者旧订单还没被清除）
            // 显示等待状态，并预告下一个号码
            orderText.text = $"<i>Meow~\nWaiting for Customer {completedOrderCount + 1}...</i>";
            orderText.color = waitingColor;
        }
    }

    // 当玩家把药水丢进 NPC 篮子时，由 NPCReceiver 调用此函数
    public void NotifyOrderSuccess()
    {
        // 记录当前这一单已经结账了，防止 Update 回跳
        lastServedOrder = AlchemyManager.Instance.currentCustomerOrder;

        StopAllCoroutines();
        StartCoroutine(ShowSuccessFeedback());
    }

    IEnumerator ShowSuccessFeedback()
    {
        isShowingSuccess = true;
        completedOrderCount++; // 增加完成计数

        // 视觉反馈：显示成功文字和当前单号
        orderText.text = $"<size=150%><b>✨ SUCCESS! ✨</b></size>\n\n" +
                         $"<color=green>Order #{completedOrderCount} Paid!</color>\n" +
                         "Great job, Master!";
        orderText.color = successColor;

        // 停顿 3 秒，让玩家看清楚结算
        yield return new WaitForSeconds(3.0f);

        isShowingSuccess = false;

        // 协程结束后，Update 会重新运行。
        // 此时由于 currentOrder 仍然等于 lastServedOrder，
        // 文字会自动切换到 "Meow~ Waiting for Customer..."
    }
}