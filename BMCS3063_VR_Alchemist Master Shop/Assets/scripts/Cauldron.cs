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
    [Range(0, 1)] public float baseColorInfluence = 0.3f;

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
            // --- 物理混色算法 ---
            float addedAmount = ingredient.amount;
            if (totalIngredientAmount == 0)
            {
                internalMixedColor = Color.Lerp(originalLiquidColor, ingredient.ingredientColor, 1f - baseColorInfluence);
            }
            else
            {
                float total = totalIngredientAmount + addedAmount;
                float ratio = addedAmount / total;
                internalMixedColor = Color.Lerp(internalMixedColor, ingredient.ingredientColor, ratio);
            }

            totalIngredientAmount += addedAmount;

            // 记录配方
            if (currentMix.ContainsKey(ingredient.ingredientName))
                currentMix[ingredient.ingredientName] += addedAmount;
            else
                currentMix.Add(ingredient.ingredientName, addedAmount);

            // --- 粒子效果 ---
            if (splashParticle != null)
            {
                splashParticle.transform.position = other.transform.position + Vector3.up * 0.1f;
                var main = splashParticle.main;
                main.startColor = ingredient.ingredientColor;
                splashParticle.Stop();
                splashParticle.Play();
            }

            // --- 平滑变色 ---
            if (liquidRenderer != null)
            {
                if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
                colorChangeCoroutine = StartCoroutine(SmoothChangeColor(internalMixedColor));
            }

            // --- 核心优化：回收逻辑 ---
            // 不要 Destroy，而是直接隐藏，让它回到 ObjectPool 的队列中
            other.gameObject.SetActive(false);
        }
    }

    IEnumerator SmoothChangeColor(Color targetShallow)
    {
        Material mat = liquidRenderer.material;
        Color startShallow = mat.GetColor(shallowColorName);
        Color targetDeep = targetShallow * 0.4f;
        targetDeep.a = 1.0f;

        float elapsed = 0;
        while (elapsed < colorTransitionTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / colorTransitionTime;
            float curve = t * t * (3f - 2f * t);

            mat.SetColor(shallowColorName, Color.Lerp(startShallow, targetShallow, curve));
            if (mat.HasProperty(deepColorName))
            {
                Color startDeep = mat.GetColor(deepColorName);
                mat.SetColor(deepColorName, Color.Lerp(startDeep, targetDeep, curve));
            }
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

        // --- 核心修正：寻找场景中当前正在等待的 NPC 并对比其订单 ---
        // 这样可以避免多 NPC 时判定错误
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

        // 这里的成品药水如果数量也很多，也可以考虑用 ObjectPool 生成
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