using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestockButton : MonoBehaviour
{
    public string ingredientName;       // 在 Inspector 填入：例如 "DragonBlood"
    public TextMeshProUGUI statusText;  // 拖入显示名字和数量的 Text 物体

    void Start()
    {
        // 绑定按钮点击事件
        GetComponent<Button>().onClick.AddListener(() => {
            AlchemyManager.Instance.BuyIngredientRestock(ingredientName);
        });
    }

    void Update()
    {
        // 实时刷新书本上显示的数值
        if (AlchemyManager.Instance != null && statusText != null)
        {
            float currentAmount = 0;
            AlchemyManager.Instance.ingredientInventory.TryGetValue(ingredientName, out currentAmount);
            statusText.text = $"{ingredientName}: {currentAmount:F0}";
        }
    }
}