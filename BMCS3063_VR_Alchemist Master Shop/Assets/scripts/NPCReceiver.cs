using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class NPCReceiver : MonoBehaviour
{
    private XRSocketInteractor socket;

    void Awake() => socket = GetComponent<XRSocketInteractor>();

    // 在 Socket 的 Hover Entered 或 Select Entered 事件中调用此函数
    public void OnPotionDelivered(SelectEnterEventArgs args)
    {
        // 获取玩家放上来的物体
        PotionObject deliveredPotion = args.interactableObject.transform.GetComponent<PotionObject>();

        if (deliveredPotion != null)
        {
            // 检查药水是否匹配当前订单 [cite: 120]
            if (deliveredPotion.potionType == AlchemyManager.Instance.currentCustomerOrder)
            {
                Debug.Log("NPC: Thank you! Here is your gold.");

                // 给玩家金币（例如 500 金币） [cite: 122]
                AlchemyManager.Instance.AddCoins(500);

                // 销毁药水并刷新下一个订单 [cite: 130]
                Destroy(deliveredPotion.gameObject);

                // 这里可以调用一个函数来随机生成下一个 Customer Order
            }
            else
            {
                Debug.Log("NPC: This is not what I asked for!");
            }
        }
    }
}