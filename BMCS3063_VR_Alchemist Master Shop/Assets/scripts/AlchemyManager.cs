using UnityEngine;
using System.Collections.Generic;

public class AlchemyManager : MonoBehaviour
{
    public static AlchemyManager Instance;

    [Header("Recipes Data")]
    public List<RecipeData> allRecipes = new List<RecipeData>();    // 所有配方总表
    public List<RecipeData> unlockedRecipes = new List<RecipeData>(); // 玩家已学会的配方

    [Header("Game Status")]
    public RecipeData currentCustomerOrder; // 当前 NPC 想要的药水
    public int currentCoins = 0;
    public int winTarget = 5000;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeStartingRecipes();
    }

    // 游戏开始，默认解锁前 3 种药水
    void InitializeStartingRecipes()
    {
        unlockedRecipes.Clear();
        for (int i = 0; i < 3; i++)
        {
            if (i < allRecipes.Count)
            {
                unlockedRecipes.Add(allRecipes[i]);
            }
        }
        Debug.Log("初始化完成，当前可用药水种类: " + unlockedRecipes.Count);
    }

    // 增加金币（卖出药水时调用）
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        Debug.Log($"<color=green>收入: {amount}</color> | 总金币: {currentCoins}");

        if (currentCoins >= winTarget)
        {
            TriggerGameWin();
        }
    }

    // 为进入商店的 NPC 随机分配一个“已解锁”的订单
    public void AssignRandomOrder()
    {
        if (unlockedRecipes.Count > 0)
        {
            int randomIndex = Random.Range(0, unlockedRecipes.Count);
            currentCustomerOrder = unlockedRecipes[randomIndex];
            Debug.Log("<color=yellow>新订单生成: </color>" + currentCustomerOrder.potionName);
        }
        else
        {
            Debug.LogWarning("没有已解锁的配方，无法分配订单！");
        }
    }

    // 解锁新药水的逻辑（如果你的游戏之后有升级系统可以用到）
    public void UnlockNewPotion(RecipeData newRecipe)
    {
        if (!unlockedRecipes.Contains(newRecipe))
        {
            unlockedRecipes.Add(newRecipe);
            Debug.Log("<color=lime>解锁了新配方: </color>" + newRecipe.potionName);
        }
    }

    void TriggerGameWin()
    {
        Debug.Log("<color=cyan><b>[胜利]</b></color> 赚够了 5000 金币！");
    }
}