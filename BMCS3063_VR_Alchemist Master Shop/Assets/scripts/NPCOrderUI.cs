using UnityEngine;
using TMPro;
using System.Collections;

public class NPCOrderUI : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject orderCanvas;    // NPC 自己的 Canvas
    public TextMeshProUGUI nameText;  // NPC 自己的 Text

    [Header("计时设置")]
    public float waitSeconds = 5.0f;     // 出现前的等待
    public float displaySeconds = 5.0f;  // 显示的时长

    // 存储这个 NPC 独有的订单，防止被全局变量覆盖
    private RecipeData myPrivateOrder;

    private void Awake()
    {
        if (orderCanvas != null) orderCanvas.SetActive(false);
    }

    // 由 NPCController 在到达柜台时调用
    public void StartOrderProcess()
    {
        StartCoroutine(IndependentOrderRoutine());
    }

    private IEnumerator IndependentOrderRoutine()
    {
        yield return new WaitForSeconds(waitSeconds);

        if (AlchemyManager.Instance != null)
        {
            // 向经理领一个随机订单并存入私有变量
            myPrivateOrder = AlchemyManager.Instance.GetRandomUnlockedRecipe();

            if (myPrivateOrder != null)
            {
                ShowOrder(myPrivateOrder);
            }
        }

        yield return new WaitForSeconds(displaySeconds);
        HideOrder();
    }

    public void ShowOrder(RecipeData recipe)
    {
        if (orderCanvas == null || nameText == null || recipe == null) return;

        nameText.text = "I need: " + recipe.potionName;
        orderCanvas.SetActive(true);
    }

    public void HideOrder()
    {
        if (orderCanvas != null) orderCanvas.SetActive(false);
    }

    // --- 核心修复：添加这个方法，让 Cauldron 能读到订单 ---
    public RecipeData GetMyOrder()
    {
        return myPrivateOrder;
    }
}