using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockPanel : MonoBehaviour
{
    [Header("配方数据")]
    public RecipeData recipeData;      // 拖入该面板对应的配方
    public int price = 500;

    [Header("UI 绑定")]
    public TextMeshProUGUI titleText;
    public Image potionIcon;
    public Button unlockButton;
    public TextMeshProUGUI priceText;

    [Header("货架关联")]
    public GameObject shelfItems;      // 对应货架上要显示的材料/图片组

    void Start()
    {
        // 初始化 UI 显示
        if (recipeData != null)
        {
            titleText.text = recipeData.potionName;
            // potionIcon.sprite = recipeData.potionIcon; // 如果你的 RecipeData 有图标的话
        }

        priceText.text = price + " G";
        unlockButton.onClick.AddListener(AttemptUnlock);

        // 如果已经解锁了，隐藏面板（针对存档读取，暂不展开）
    }

    public void AttemptUnlock()
    {
        Debug.Log("射线确实点到我了！");

        if (AlchemyManager.Instance != null && AlchemyManager.Instance.currentCoins >= price)
        {
            // 1. 扣钱
            AlchemyManager.Instance.currentCoins -= price;

            // 2. 告诉经理：这个药水现在可以卖了
            AlchemyManager.Instance.UnlockNewPotion(recipeData);

            // 3. 激活货架上的物体
            if (shelfItems != null) shelfItems.SetActive(true);

            // 4. 视觉反馈：销毁或隐藏解锁 UI
            Debug.Log($"解锁成功！{recipeData.potionName} 已上架。");
            gameObject.SetActive(false);

            // 可以加个撒花粒子效果
        }
        else
        {
            Debug.Log("<color=red>金币不足！</color>");
            // 这里可以给按钮加个抖动动画
        }
    }
}