using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class NPCReceiver : MonoBehaviour
{
    private XRSocketInteractor socket;
    private bool isProcessing = false; // 状态锁：防止重复触发

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    // 在 Unity Inspector 的 XR Socket Interactor -> Select Entered 事件中绑定此函数
    public void OnPotionDelivered(SelectEnterEventArgs args)
    {
        // --- 核心修复：如果正在处理中，直接跳过 ---
        if (isProcessing) return;

        // 1. 获取药瓶对象
        PotionObject potion = args.interactableObject.transform.GetComponent<PotionObject>();

        // 2. 验证 NPC 状态和药水
        NPCController currentNPC = Object.FindFirstObjectByType<NPCController>();

        // 只有当 NPC 存在，且处于等待订单状态时才处理
        if (currentNPC != null && currentNPC.currentState == NPCController.NPCState.Waiting)
        {
            if (potion != null && potion.potionType == AlchemyManager.Instance.currentCustomerOrder)
            {
                // --- 锁定逻辑：开始处理交付 ---
                isProcessing = true;
                Debug.Log("药水正确！开始单次结算流程...");

                if (GameDataTracker.Instance != null)
                {
                    GameDataTracker.Instance.customersServed++;
                }

                // A: 获取价格
                int rewardAmount = AlchemyManager.Instance.currentCustomerOrder.potionPrice;

                // B: 解除 Socket 选择
                if (socket.interactionManager != null)
                {
                    socket.interactionManager.SelectExit(socket, args.interactableObject);
                }

                // C: 财务增加
                AlchemyManager.Instance.AddCoins(rewardAmount);

                // D: 命令 NPC 离开
                currentNPC.OnReceivePotion();

                // E: 销毁药瓶
                Destroy(potion.gameObject, 0.1f);

                Debug.Log($"交付成功！金币 +{rewardAmount}。当前总额: {AlchemyManager.Instance.currentCoins}");

                // 重置锁：为了下一个 NPC 准备。
                // 我们在 2 秒后解锁（或者等下一个 NPC 生成时由 NPCManager 重置）
                Invoke(nameof(ResetProcessor), 2.0f);
            }
            else
            {
                Debug.LogWarning("交付失败：药水不匹配或物体错误。");
            }
        }
    }

    // 重置锁，允许下一次交付
    public void ResetProcessor()
    {
        isProcessing = false;
        Debug.Log("交付器已重置，可以接收下一个订单。");
    }
}