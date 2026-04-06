using UnityEngine;
using System.Collections.Generic;

public class Cauldron : MonoBehaviour
{
    // 存储当前坩埚内已经加入的材料及其总量
    public Dictionary<string, float> currentMix = new Dictionary<string, float>();

    [Header("Stirring Settings")]
    public float stirRequired = 10.0f;
    private float currentStirProgress = 0f;
    private bool isFinished = false;

    [Header("Spawn Settings")]
    public Transform spawnPoint; // 药水生成的坐标点

    // --- 材料进入检测 ---
    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            if (currentMix.ContainsKey(ingredient.ingredientName))
            {
                currentMix[ingredient.ingredientName] += ingredient.amount;
            }
            else
            {
                currentMix.Add(ingredient.ingredientName, ingredient.amount);
            }

            Debug.Log($"Added {ingredient.ingredientName}. Total ingredients: {currentMix.Count}");
            Destroy(other.gameObject); // 销毁掉入的材料
        }
    }

    // 由搅拌棒调用的增加进度函数
    public void AddStirProgress(float amount)
    {
        if (isFinished) return;

        currentStirProgress += amount;
        // 打印搅拌百分比
        Debug.Log($"Stirring Progress: {Mathf.Clamp01(currentStirProgress / stirRequired) * 100:F0}%");

        if (currentStirProgress >= stirRequired)
        {
            FinishPotion();
        }
    }

    void FinishPotion()
    {
        isFinished = true;

        // 1. 获取当前 NPC 要求的订单数据
        RecipeData targetRecipe = AlchemyManager.Instance.currentCustomerOrder;

        // 2. 判定是否成功（包含解锁检查和比例检查）
        if (CheckSuccess(targetRecipe))
        {
            Debug.Log("<color=green>制作成功！</color>");

            // --- 核心：动态生成该配方指定的模型 ---
            if (targetRecipe.potionPrefab != null)
            {
                GameObject finalPotion = Instantiate(targetRecipe.potionPrefab, spawnPoint.position, spawnPoint.rotation);

                // 注入数据：让生成的药水知道自己属于哪个配方，方便 NPCReceiver 识别
                PotionObject pObj = finalPotion.GetComponent<PotionObject>();
                if (pObj != null)
                {
                    pObj.potionType = targetRecipe;
                }
            }
            else
            {
                Debug.LogError($"{targetRecipe.potionName} 没有分配 Potion Prefab!");
            }

            ResetCauldron();
        }
        else
        {
            Debug.Log("<color=red>制作失败！</color> 可能原因：材料不对、误差太大或未解锁该药水。");
            // 这里可以触发冒烟特效提示玩家
            ResetCauldron();
        }
    }

    bool CheckSuccess(RecipeData recipe)
    {
        if (recipe == null) return false;

        // --- 校验 1：解锁状态检查 ---
        // 只有已解锁的药水才能制作成功
        if (!AlchemyManager.Instance.unlockedRecipes.Contains(recipe))
        {
            Debug.LogWarning("该药水尚未解锁，无法完成制作！");
            return false;
        }

        // --- 校验 2：材料比例检查 ---
        float totalError = 0f;
        foreach (var req in recipe.ingredients)
        {
            if (currentMix.ContainsKey(req.ingredientName))
            {
                // 计算该材料的误差比例
                float error = Mathf.Abs(currentMix[req.ingredientName] - req.requiredAmount) / req.requiredAmount;
                totalError += error;
            }
            else
            {
                // 缺少订单要求的必要材料
                return false;
            }
        }

        // 允许的总误差阈值为 20%
        return totalError < 0.2f;
    }

    public void ResetCauldron()
    {
        currentMix.Clear();
        currentStirProgress = 0f;
        isFinished = false;
        Debug.Log("坩埚已重置，等待下一位顾客。");
    }
}