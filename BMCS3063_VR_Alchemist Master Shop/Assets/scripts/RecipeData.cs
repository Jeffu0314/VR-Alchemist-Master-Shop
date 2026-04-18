using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemy/Recipe")]
public class RecipeData : ScriptableObject
{
    public string potionName;          // 药水名称
    public int unlockCost;             // 解锁所需金币
    public Sprite potionIcon;          // 药水图标

    [Header("Visual Settings")]
    public GameObject potionPrefab;    
    public Color liquidColor;         

    [Header("Price Settings")]
    public int potionPrice;            

    [Header("Recipe Ratios")]
    public List<IngredientRequirement> ingredients;

    [System.Serializable]
    public struct IngredientRequirement
    {
        public string ingredientName;
        public float requiredAmount;
    }
}