using UnityEngine;
using System.Collections.Generic;

public class AlchemyManager : MonoBehaviour
{
    public static AlchemyManager Instance;

    [Header("Recipes Data")]
    public List<RecipeData> allRecipes = new List<RecipeData>(); // 这里的 10 个是总库
    public List<RecipeData> unlockedRecipes = new List<RecipeData>(); // 这里的代码自动生成的

    [Header("Current Status")]
    public RecipeData currentCustomerOrder;
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
        Debug.Log("初始化完成，初始解锁药水: " + unlockedRecipes.Count);
    }

    // --- 核心修改：供 Testing Cube 调用的自动解锁函数 ---
    public void UnlockNextAvailable()
    {
        RecipeData nextToUnlock = null;

        // 遍历总库，找到第一个还没解锁的
        foreach (var recipe in allRecipes)
        {
            if (!unlockedRecipes.Contains(recipe))
            {
                nextToUnlock = recipe;
                break; // 找到一个就退出循环
            }
        }

        if (nextToUnlock != null)
        {
            UnlockNewPotion(nextToUnlock);
        }
        else
        {
            Debug.Log("<color=white>提示：</color>所有配方已全部解锁完毕！");
        }
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        Debug.Log("<color=green>收入: </color>" + amount + " | 总额: " + currentCoins);

        if (currentCoins >= winTarget)
        {
            TriggerGameWin();
        }
    }

    public void AssignRandomOrder()
    {
        if (unlockedRecipes.Count > 0)
        {
            int randomIndex = Random.Range(0, unlockedRecipes.Count);
            currentCustomerOrder = unlockedRecipes[randomIndex];
            Debug.Log("<color=yellow>新订单: </color>" + currentCustomerOrder.potionName + " | 预期收入: " + currentCustomerOrder.potionPrice);
        }
    }

    public void UnlockNewPotion(RecipeData newRecipe)
    {
        int cost = newRecipe.unlockCost;

        if (currentCoins >= cost)
        {
            if (!unlockedRecipes.Contains(newRecipe))
            {
                currentCoins -= cost;
                unlockedRecipes.Add(newRecipe);
                Debug.Log("<color=lime>解锁成功: </color>" + newRecipe.potionName + " | 剩余金币: " + currentCoins);
            }
        }
        else
        {
            Debug.LogWarning("金币不足！解锁 " + newRecipe.potionName + " 需要: " + cost + " 当前: " + currentCoins);
        }
    }

    void TriggerGameWin()
    {
        Debug.Log("<color=cyan><b>[胜利]</b></color> 赚够了 5000 金币！");
    }
}