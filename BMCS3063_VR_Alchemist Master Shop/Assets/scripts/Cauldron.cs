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
    public GameObject potionPrefab;
    public Transform spawnPoint;

    // --- 关键修复：添加这个函数来销毁并记录材料 ---
    private void OnTriggerEnter(Collider other)
    {
        // 尝试获取进入物体的 Ingredient 脚本
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            // 1. 记录材料数据
            if (currentMix.ContainsKey(ingredient.ingredientName))
            {
                currentMix[ingredient.ingredientName] += ingredient.amount;
            }
            else
            {
                currentMix.Add(ingredient.ingredientName, ingredient.amount);
            }

            Debug.Log($"Added {ingredient.ingredientName}. Current mix count: {currentMix.Count}");

            // 2. 销毁掉进来的材料物体
            Destroy(other.gameObject);
        }
    }

    public void AddStirProgress(float amount)
    {
        if (isFinished) return;

        currentStirProgress += amount;
        Debug.Log($"Stirring... {Mathf.Clamp01(currentStirProgress / stirRequired) * 100}%");

        if (currentStirProgress >= stirRequired)
        {
            FinishPotion();
        }
    }

    void FinishPotion()
    {
        isFinished = true;
        RecipeData target = AlchemyManager.Instance.currentCustomerOrder;

        if (CheckSuccess(target))
        {
            Debug.Log("Potion Success!");
            // 自动生成药水道具
            GameObject finalPotion = Instantiate(potionPrefab, spawnPoint.position, spawnPoint.rotation);

            // 将当前订单的数据传给这瓶药水，以便 NPC 识别
            finalPotion.GetComponent<PotionObject>().potionType = target;

            // 重置坩埚等待下一次制作
            ResetCauldron();
        }
        else
        {
            Debug.Log("Potion Failed!");
            // 可以在这里触发冒烟特效
        }
    }

    bool CheckSuccess(RecipeData recipe)
    {
        if (recipe == null) return false;
        float totalError = 0f;

        foreach (var req in recipe.ingredients)
        {
            if (currentMix.ContainsKey(req.ingredientName))
            {
                float error = Mathf.Abs(currentMix[req.ingredientName] - req.requiredAmount) / req.requiredAmount;
                totalError += error;
            }
            else
            {
                return false;
            }
        }
        return totalError < 0.2f;
    }

    // 用于重置坩埚（如果你在流程图中执行了“Recycle”操作）
    public void ResetCauldron()
    {
        currentMix.Clear();
        currentStirProgress = 0f;
        isFinished = false;
        Debug.Log("Cauldron has been reset.");
    }
}