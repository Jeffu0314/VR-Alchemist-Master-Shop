using UnityEngine;
using System.Collections.Generic;

public class AlchemyManager : MonoBehaviour
{
    public static AlchemyManager Instance;

    [Header("Recipes Data")]
    public List<RecipeData> allRecipes = new List<RecipeData>();
    public List<RecipeData> unlockedRecipes = new List<RecipeData>();

    [Header("Inventory Settings")]
    public float startingAmount = 20f;  // 初始库存数量
    public int restockCost = 50;        // 补货一次的价格
    public float restockAddAmount = 20f; // 补货一次增加的数量

    // 库存字典：Key 是材料名字，Value 是剩余数量
    public Dictionary<string, float> ingredientInventory = new Dictionary<string, float>();

    [Header("Spawn Settings")]
    [Tooltip("在 Inspector 里配置：每个材料名字对应的桌子生成点和预制体")]
    public List<IngredientSpawnInfo> spawnInfos = new List<IngredientSpawnInfo>();

    [System.Serializable]
    public class IngredientSpawnInfo
    {
        public string ingredientName;    // 必须和 RecipeData 里的名字一致
        public Transform spawnPoint;    // 桌子上对应的空物体位置
        public GameObject prefab;       // 素材的物理预制体
    }

    [Header("Current Status")]
    public RecipeData currentCustomerOrder;
    public int currentCoins = 0;
    public int winTarget = 5000;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        InitializeStartingRecipes();
        InitializeInventory();
    }

    // 1. 初始化库存：将所有涉及到的材料初始设为 20
    void InitializeInventory()
    {
        ingredientInventory.Clear();
        foreach (var info in spawnInfos)
        {
            if (!ingredientInventory.ContainsKey(info.ingredientName))
            {
                ingredientInventory.Add(info.ingredientName, startingAmount);
                // 游戏开始时在桌子上先生成一波物理素材
                SpawnPhysicalIngredient(info.ingredientName);
            }
        }
        Debug.Log("<color=white>库存初始化完成，每种素材初始量: </color>" + startingAmount);
    }

    // 2. 核心补货逻辑：书本上的购买按钮调用此函数
    public void BuyIngredientRestock(string name)
    {
        if (currentCoins >= restockCost)
        {
            currentCoins -= restockCost;

            // 增加库存数值
            if (ingredientInventory.ContainsKey(name))
            {
                ingredientInventory[name] += restockAddAmount;
                Debug.Log($"<color=lime>购买成功:</color> {name} +{restockAddAmount}。剩余金币: {currentCoins}");

                // 在桌子上的固定位置生成物理物体
                SpawnPhysicalIngredient(name);
            }
        }
        else
        {
            Debug.LogWarning("金币不足！无法补货 " + name);
        }
    }

    // 3. 在桌子上生成物理素材的方法
    public void SpawnPhysicalIngredient(string name)
    {
        IngredientSpawnInfo info = spawnInfos.Find(x => x.ingredientName == name);
        if (info != null && info.spawnPoint != null && info.prefab != null)
        {
            // 在对应位置生成，并随机给一点点旋转偏移，看起来自然点
            Instantiate(info.prefab, info.spawnPoint.position, info.spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning($"无法生成 {name}，请检查 SpawnInfos 配置是否完整！");
        }
    }

    // --- 药水相关逻辑保持不变 ---

    void InitializeStartingRecipes()
    {
        unlockedRecipes.Clear();
        for (int i = 0; i < 3; i++)
        {
            if (i < allRecipes.Count) unlockedRecipes.Add(allRecipes[i]);
        }
    }

    public void UnlockNextAvailable()
    {
        RecipeData nextToUnlock = null;
        foreach (var recipe in allRecipes)
        {
            if (!unlockedRecipes.Contains(recipe))
            {
                nextToUnlock = recipe;
                break;
            }
        }
        if (nextToUnlock != null) UnlockNewPotion(nextToUnlock);
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        if (currentCoins >= winTarget) TriggerGameWin();
    }

    public void AssignRandomOrder()
    {
        if (unlockedRecipes.Count > 0)
        {
            int randomIndex = Random.Range(0, unlockedRecipes.Count);
            currentCustomerOrder = unlockedRecipes[randomIndex];
            Debug.Log("<color=yellow>新订单: </color>" + currentCustomerOrder.potionName);
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
                Debug.Log("<color=lime>解锁成功: </color>" + newRecipe.potionName);
            }
        }
    }

    void TriggerGameWin()
    {
        Debug.Log("<color=cyan><b>[胜利]</b></color> 目标达成！");
    }
}