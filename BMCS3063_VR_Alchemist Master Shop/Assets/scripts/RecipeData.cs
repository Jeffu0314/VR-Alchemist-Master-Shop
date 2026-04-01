using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemy/Recipe")]
public class RecipeData : ScriptableObject
{
    public string potionName;          // 药水名称
    public int unlockCost;             // 解锁所需金币
    public Sprite potionIcon;          // 药水图标（用于UI显示）

    [Header("Recipe Ratios")]
    public List<IngredientRequirement> ingredients; // 配方要求的材料比例

    [System.Serializable]
    public struct IngredientRequirement
    {
        public string ingredientName;  // 材料名称（如：龙血、麒麟草）
        public float requiredAmount;   // 标准配方量
    }
}