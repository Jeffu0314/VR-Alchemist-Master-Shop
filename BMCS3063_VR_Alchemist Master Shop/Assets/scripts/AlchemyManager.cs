using UnityEngine;
using System.Collections.Generic;

public class AlchemyManager : MonoBehaviour
{
    // 单例模式，方便其他脚本直接访问
    public static AlchemyManager Instance { get; private set; }

    [Header("Game State")]
    public int currentCoins = 0;              // 当前金币 
    public int targetGoal = 5000;             // 目标金币（根据你最新的流程图，建议设为 5000）

    [Header("Recipe Collections")]
    public List<RecipeData> allRecipes;       // 游戏中所有的配方
    public List<RecipeData> unlockedRecipes;  // 玩家当前已解锁的配方

    [Header("Order Management")]
    public RecipeData currentCustomerOrder;   // 当前顾客要求的药水订单

    private void Awake()
    {
        // 确保场景中只有一个 AlchemyManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 更新金币的方法
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        Debug.Log("Current Gold: " + currentCoins);

        // 检查是否达到金币目标
        if (currentCoins >= targetGoal)
        {
            Debug.Log("Goal Reached! Choose to continue or end game.");
            // 这里以后可以触发显示 "Complete Goal Message" 的 UI
        }
    }

    // 解锁新配方的逻辑
    public void UnlockRecipe(RecipeData recipe)
    {
        if (currentCoins >= recipe.unlockCost && !unlockedRecipes.Contains(recipe))
        {
            currentCoins -= recipe.unlockCost;
            unlockedRecipes.Add(recipe);
            Debug.Log("Unlocked: " + recipe.potionName);
        }
    }
}