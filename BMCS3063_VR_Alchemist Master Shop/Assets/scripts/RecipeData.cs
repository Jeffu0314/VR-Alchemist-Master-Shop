using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemy/Recipe")]
public class RecipeData : ScriptableObject
{
    public string potionName;          // 药水名称
    public int unlockCost;             // 解锁所需金币
    public Sprite potionIcon;          // 药水图标

    [Header("Visual Settings")]
    public GameObject potionPrefab;    // <--- 关键：拖入该药水对应的 3D 模型预制体
    public Color liquidColor;          // 坩埚液体的颜色

    [Header("Price Settings")]
    public int potionPrice;            // 卖给 NPC 的价格

    [Header("Recipe Ratios")]
    public List<IngredientRequirement> ingredients;

    [System.Serializable]
    public struct IngredientRequirement
    {
        public string ingredientName;
        public float requiredAmount;
    }
}