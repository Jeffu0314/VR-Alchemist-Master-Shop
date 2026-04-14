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

    [Header("Visual Effects & Feedback")]
    public ParticleSystem splashParticle;
    public ParticleSystem successParticle;
    public ParticleSystem failParticle;
    public Renderer liquidRenderer;
    public Transform liquidVisual;

    [Header("Color Settings")]
    public string shallowColorName = "_ShallowColor";
    public string deepColorName = "_DeepColor";
    public float colorTransitionTime = 1.8f;
    [Range(0, 1)] public float baseColorInfluence = 0.3f; // 初始锅水对第一份材料的影响力

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
            float addedAmount = ingredient.amount;

            // --- 自然颜色混合逻辑开始 ---
            if (totalIngredientAmount == 0)
            {
                // 第一份材料：从锅水原色渐变到材料颜色
                // 此时 internalMixedColor 直接设为目标色，由协程去完成视觉过渡
                internalMixedColor = Color.Lerp(originalLiquidColor, ingredient.ingredientColor, 1f - baseColorInfluence);
            }
            else
            {
                // 物理混合：基于当前总量计算新颜色的权重
                float total = totalIngredientAmount + addedAmount;
                float ratio = addedAmount / total;

                // 重点：使用线性插值混合。加红色就变红，加黄色就变橘
                internalMixedColor = Color.Lerp(internalMixedColor, ingredient.ingredientColor, ratio);
            }

            totalIngredientAmount += addedAmount;
            // --- 自然颜色混合逻辑结束 ---

            // 记录配方 (保持原样)
            if (currentMix.ContainsKey(ingredient.ingredientName))
                currentMix[ingredient.ingredientName] += addedAmount;
            else
                currentMix.Add(ingredient.ingredientName, addedAmount);

            // 粒子反馈 (保持原样)
            if (splashParticle != null)
            {
                splashParticle.transform.position = other.transform.position + Vector3.up * 0.1f;
                var main = splashParticle.main;
                main.startColor = ingredient.ingredientColor;
                splashParticle.Stop();
                splashParticle.Play();
            }

            // --- 触发平滑变色协程 ---
            if (liquidRenderer != null)
            {
                if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
                colorChangeCoroutine = StartCoroutine(SmoothChangeColor(internalMixedColor));
            }

            other.gameObject.SetActive(false);
        }
    }

    IEnumerator SmoothChangeColor(Color targetShallow)
    {
        Material mat = liquidRenderer.material;
        Color startShallow = mat.GetColor(shallowColorName);

        // 计算对应的深色层，保持视觉深度
        Color targetDeep = targetShallow * 0.35f; // 自然深色通常是浅色的 35%
        targetDeep.a = 1.0f;

        float elapsed = 0;
        while (elapsed < colorTransitionTime)
        {
            elapsed += Time.deltaTime;
            // 使用 SmoothStep 提供最自然的淡入淡出（S型）插值曲线
            float t = Mathf.SmoothStep(0, 1, elapsed / colorTransitionTime);

            // 应用浅色过渡
            mat.SetColor(shallowColorName, Color.Lerp(startShallow, targetShallow, t));

            // 应用深色过渡（如果 Shader 支持）
            if (mat.HasProperty(deepColorName))
            {
                Color startDeep = mat.GetColor(deepColorName);
                mat.SetColor(deepColorName, Color.Lerp(startDeep, targetDeep, t));
            }
            yield return null;
        }

        // 最终锁定颜色，防止浮点数误差
        mat.SetColor(shallowColorName, targetShallow);
    }

    // --- 以下逻辑均为你的原始代码，未做任何改动 ---

    public void AddStirProgress(float amount)
    {
        if (isFinished || currentMix.Count == 0) return;

        if (uiCanvasGroup != null && !uiCanvasGroup.activeSelf)
            uiCanvasGroup.SetActive(true);

        currentStirProgress += amount;
        if (progressSlider != null) progressSlider.value = currentStirProgress;

        if (liquidRenderer != null)
        {
            float wave = Mathf.Sin(Time.time * 8f) * 0.03f;
            Color current = liquidRenderer.material.GetColor(shallowColorName);
            liquidRenderer.material.SetColor(shallowColorName, current + Color.white * wave);
        }

        if (liquidVisual != null)
            liquidVisual.Rotate(Vector3.up, amount * 150f);

        if (currentStirProgress >= stirRequired)
            FinishPotion();
    }

    void FinishPotion()
    {
        isFinished = true;
        if (uiCanvasGroup != null) uiCanvasGroup.SetActive(false);

        NPCOrderUI targetNPC = Object.FindFirstObjectByType<NPCOrderUI>();
        RecipeData targetRecipe = null;

        if (targetNPC != null)
        {
            targetRecipe = targetNPC.GetMyOrder();
        }

        if (CheckSuccess(targetRecipe))
            HandleSuccess(targetRecipe);
        else
            HandleFailure();
    }

    bool CheckSuccess(RecipeData recipe)
    {
        if (recipe == null) return false;

        foreach (var req in recipe.ingredients)
        {
            if (!currentMix.ContainsKey(req.ingredientName)) return false;
            float error = Mathf.Abs(currentMix[req.ingredientName] - req.requiredAmount) / req.requiredAmount;
            if (error > 0.25f) return false;
        }
        return currentMix.Count == recipe.ingredients.Count;
    }

    void HandleSuccess(RecipeData targetRecipe)
    {
        if (successParticle != null) successParticle.Play();

        if (targetRecipe != null && targetRecipe.potionPrefab != null)
        {
            Instantiate(targetRecipe.potionPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        ResetCauldron();
    }

    void HandleFailure()
    {
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
        if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
        colorChangeCoroutine = StartCoroutine(SmoothChangeColor(originalLiquidColor));
    }
}