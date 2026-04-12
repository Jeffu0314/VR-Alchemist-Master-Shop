using UnityEngine;
using TMPro;

public class RecipeUnlocker : MonoBehaviour
{
    [Header("解锁设置")]
    public RecipeData recipeToUnlock;    // 关联你要解锁的那个配方资源
    public int unlockPrice = 500;       // 解锁价格
    public GameObject shelfVisualGroup; // 货架上对应的材料和图片父物体

    [Header("UI 显示")]
    public TextMeshPro priceText;

    private bool isUnlocked = false;

    void Start()
    {
        if (priceText != null) priceText.text = $"{unlockPrice} Gold";

        // 确保一开始货架物品是隐藏的
        if (shelfVisualGroup != null) shelfVisualGroup.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 玩家走近触发购买
        if (other.CompareTag("Player") && !isUnlocked)
        {
            TryPurchase();
        }
    }

    void TryPurchase()
    {
        if (AlchemyManager.Instance != null && AlchemyManager.Instance.currentCoins >= unlockPrice)
        {
            UnlockContent();
        }
        else
        {
            Debug.Log("钱不够，无法解锁新配方！");
        }
    }

    void UnlockContent()
    {
        isUnlocked = true;

        // 1. 扣除金币
        AlchemyManager.Instance.currentCoins -= unlockPrice;

        // 2. 将配方加入经理的“已解锁列表”
        // 这样 NPC 就会开始点这个新药水了
        AlchemyManager.Instance.UnlockNewPotion(recipeToUnlock);

        // 3. 让货架上的材料“变”出来
        if (shelfVisualGroup != null)
        {
            shelfVisualGroup.SetActive(true);
            // 这里可以加一个简单的音效或粒子特效
            Debug.Log($"<color=lime>解锁成功！{recipeToUnlock.potionName} 已上架。</color>");
        }

        // 4. 移除购买触发器和文字
        if (priceText != null) priceText.gameObject.SetActive(false);
        // 如果你希望这个触发器消失，可以 Destory 掉脚本挂载的物体
        // Destroy(gameObject); 
    }
}