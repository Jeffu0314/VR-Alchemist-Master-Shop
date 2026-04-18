using UnityEngine;
using System.Collections.Generic;

public class AlchemyManager : MonoBehaviour
{
    public static AlchemyManager Instance;

    [Header("Recipes Data")]
    public List<RecipeData> allRecipes = new List<RecipeData>();    
    public List<RecipeData> unlockedRecipes = new List<RecipeData>(); 

    [Header("Game Status")]
    public RecipeData currentCustomerOrder;
    public int currentCoins = 0;
    public int winTarget = 5000;

    private bool hasTriggeredWinMenu = false;

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
    }

    public RecipeData GetRandomUnlockedRecipe()
    {
        if (unlockedRecipes.Count > 0)
        {
            int randomIndex = Random.Range(0, unlockedRecipes.Count);
            RecipeData selectedRecipe = unlockedRecipes[randomIndex];

            currentCustomerOrder = selectedRecipe;
            return selectedRecipe;
        }

        return null;
    }

    // 增加金币
    public void AddCoins(int amount)
    {
        currentCoins += amount;

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
        }
    }

    void TriggerGameWin()
    {
        if (hasTriggeredWinMenu) return;


        if (VictoryUIController.Instance != null)
        {
            hasTriggeredWinMenu = true; 
            VictoryUIController.Instance.ShowMenu();
        }
    }
}