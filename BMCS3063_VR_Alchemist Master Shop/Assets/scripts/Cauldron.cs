using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class Cauldron : MonoBehaviour
{
    [Header("Ingredients Logic")]
    public Dictionary<string, float> currentMix = new Dictionary<string, float>();
    private Color internalMixedColor;
    private float totalIngredientAmount = 0f;

    [Header("Stirring Settings")]
    public float stirRequired = 10.0f;
    public float currentStirProgress = 0f;
    private bool isFinished = false;

    [Header("UI Settings")]
    public Slider progressSlider;
    public GameObject uiCanvasGroup;

    [Header("Visual Effects & Feedback (直接拖入场景里的物体)")]
    // 修改为 ParticleSystem 类型，这样直接拖入层级里的物体
    public ParticleSystem splashParticle;   // 拖入层级里的 Splash Effect
    public ParticleSystem successParticle;  // 拖入层级里的 Success Flare
    public ParticleSystem failParticle;     // 拖入层级里的 Fail Smoke
    public Renderer liquidRenderer;
    public Transform liquidVisual;

    [Header("Shader Settings")]
    public string shallowColorName = "_ShallowColor";
    public string deepColorName = "_DeepColor";
    public float colorTransitionTime = 1.2f;

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public GameObject junkPotionPrefab;

    private Color originalLiquidColor;
    private Coroutine colorChangeCoroutine;

    void Start()
    {
        if (liquidRenderer != null && liquidRenderer.material.HasProperty(shallowColorName))
        {
            originalLiquidColor = liquidRenderer.material.GetColor(shallowColorName);
            internalMixedColor = originalLiquidColor;
        }

        if (uiCanvasGroup != null) uiCanvasGroup.SetActive(false);
        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = stirRequired;
            progressSlider.value = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            if (currentMix.ContainsKey(ingredient.ingredientName))
                currentMix[ingredient.ingredientName] += ingredient.amount;
            else
                currentMix.Add(ingredient.ingredientName, ingredient.amount);

            float newTotalAmount = totalIngredientAmount + ingredient.amount;
            float blendWeight = (totalIngredientAmount == 0) ? 0.8f : ingredient.amount / newTotalAmount;
            internalMixedColor = Color.Lerp(internalMixedColor, ingredient.ingredientColor, blendWeight);
            totalIngredientAmount = newTotalAmount;

            // --- 粒子修复逻辑 ---
            if (splashParticle != null)
            {
                // 将粒子移动到碰撞位置（稍微往上一点防止被淹没）
                splashParticle.transform.position = other.transform.position + Vector3.up * 0.1f;

                var main = splashParticle.main;
                main.startColor = ingredient.ingredientColor; // 同步颜色

                splashParticle.Stop(); // 先停止之前的
                splashParticle.Play(); // 播放
            }

            if (liquidRenderer != null)
            {
                if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
                colorChangeCoroutine = StartCoroutine(SmoothChangeColor(internalMixedColor));
            }

            Destroy(other.gameObject);
        }
    }

    IEnumerator SmoothChangeColor(Color targetShallow)
    {
        Material mat = liquidRenderer.material;
        if (!mat.HasProperty(shallowColorName)) yield break;

        Color startShallow = mat.GetColor(shallowColorName);
        Color startDeep = mat.HasProperty(deepColorName) ? mat.GetColor(deepColorName) : originalLiquidColor * 0.4f;
        Color targetDeep = targetShallow * 0.4f;

        float elapsed = 0;
        while (elapsed < colorTransitionTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / colorTransitionTime;
            float curve = Mathf.SmoothStep(0, 1, t);

            mat.SetColor(shallowColorName, Color.Lerp(startShallow, targetShallow, curve));
            if (mat.HasProperty(deepColorName))
                mat.SetColor(deepColorName, Color.Lerp(startDeep, targetDeep, curve));

            yield return null;
        }
    }

    public void AddStirProgress(float amount)
    {
        if (isFinished || currentMix.Count == 0) return;

        if (uiCanvasGroup != null && !uiCanvasGroup.activeSelf)
            uiCanvasGroup.SetActive(true);

        currentStirProgress += amount;

        if (progressSlider != null) progressSlider.value = currentStirProgress;

        if (liquidVisual != null)
            liquidVisual.Rotate(Vector3.up, amount * 150f);

        if (currentStirProgress >= stirRequired)
            FinishPotion();
    }

    void FinishPotion()
    {
        isFinished = true;
        if (uiCanvasGroup != null) uiCanvasGroup.SetActive(false);

        RecipeData targetRecipe = AlchemyManager.Instance.currentCustomerOrder;
        if (CheckSuccess(targetRecipe)) HandleSuccess(targetRecipe);
        else HandleFailure();
    }

    bool CheckSuccess(RecipeData recipe)
    {
        if (recipe == null || !AlchemyManager.Instance.unlockedRecipes.Contains(recipe)) return false;
        foreach (var req in recipe.ingredients)
        {
            if (!currentMix.ContainsKey(req.ingredientName)) return false;
            float error = Mathf.Abs(currentMix[req.ingredientName] - req.requiredAmount) / req.requiredAmount;
            if (error > 0.2f) return false;
        }
        return currentMix.Count == recipe.ingredients.Count;
    }

    void HandleSuccess(RecipeData targetRecipe)
    {
        // 播放层级里的成功粒子
        if (successParticle != null) successParticle.Play();

        if (targetRecipe.potionPrefab != null)
        {
            GameObject finalPotion = Instantiate(targetRecipe.potionPrefab, spawnPoint.position, spawnPoint.rotation);
            PotionObject pObj = finalPotion.GetComponent<PotionObject>();
            if (pObj != null) pObj.potionType = targetRecipe;
        }
        ResetCauldron();
    }

    void HandleFailure()
    {
        // 播放层级里的失败粒子
        if (failParticle != null) failParticle.Play();

        if (junkPotionPrefab != null) Instantiate(junkPotionPrefab, spawnPoint.position, spawnPoint.rotation);

        if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
        colorChangeCoroutine = StartCoroutine(SmoothChangeColor(Color.black));

        ResetCauldron();
    }

    public void ResetCauldron()
    {
        currentMix.Clear();
        currentStirProgress = 0f;
        totalIngredientAmount = 0f;
        isFinished = false;
        if (progressSlider != null) progressSlider.value = 0;
        Invoke(nameof(StartResetColor), 3.0f);
    }

    void StartResetColor()
    {
        internalMixedColor = originalLiquidColor;
        if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
        colorChangeCoroutine = StartCoroutine(SmoothChangeColor(originalLiquidColor));
    }
}