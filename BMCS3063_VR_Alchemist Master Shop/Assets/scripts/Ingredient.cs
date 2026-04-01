using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public string ingredientName; // 必须与 RecipeData 里的名字完全一致
    public float amount = 1.0f;   // 这一份材料代表的量
}