using UnityEngine;
using System.Collections.Generic;

public class AlchemyManager : MonoBehaviour
{
    public static AlchemyManager Instance;

    [Header("Recipes Data")]
    public List<RecipeData> allRecipes = new List<RecipeData>();    // 所有配方总表
    public List<RecipeData> unlockedRecipes = new List<RecipeData>(); // 玩家已学会的配方

    [Header("Game Status")]
    // 注意：在多 NPC 模式下，这个变量仅作为“最后生成的订单”记录，主要判定应参考 NPC 自身
    public RecipeData currentCustomerOrder;
    public int currentCoins = 0;
    public int winTarget = 5000;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 如果这个经理跨场景，可以取消注释下面这行
            // DontDestroyOnLoad(gameObject);
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

    // --- 核心修复：添加这个方法供 NPCOrderUI 调用 ---
    public RecipeData GetRandomUnlockedRecipe()
    {
        if (unlockedRecipes.Count > 0)
        {
            int randomIndex = Random.Range(0, unlockedRecipes.Count);
            RecipeData selectedRecipe = unlockedRecipes[randomIndex];

            // 同步更新一下全局变量，方便调试查看
            currentCustomerOrder = selectedRecipe;
            return selectedRecipe;
        }

        Debug.LogError("经理：没有已解锁的配方！");
        return null;
    }

    // 增加金币
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        Debug.Log($"<color=green>收入: {amount}</color> | 总金币: {currentCoins}");

        if (currentCoins >= winTarget)
        {
            TriggerGameWin();
        }
    }

    // 在 AlchemyManager.cs 中添加
    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            Debug.Log("<color=yellow>消费成功：</color>" + amount);
            return true;
        }
        return false;
    }

    // 解锁新药水
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
        Debug.Log("<color=cyan><b>[胜利]</b></color> 赚够了目标金币！");
    }
}