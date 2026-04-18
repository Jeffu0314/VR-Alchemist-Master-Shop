using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Cauldron : MonoBehaviour
{
    [Header("Ingredients Logic")]
    public Dictionary<string, float> currentMix = new Dictionary<string, float>();
    private Color internalMixedColor;
    private float totalIngredientAmount = 0f;
    private Vector3 colorRGBAccumulator = Vector3.zero; // 颜色分量累加器

    [Header("Stirring Settings")]
    public float stirRequired = 10.0f;
    public float currentStirProgress = 0f;
    private float lastFrameStirProgress = 0f;
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

    [Header("Audio Settings")]
    public AudioSource cauldronAudioSource;
    public AudioSource stirringAudioSource;
    public AudioClip splashSound;
    public AudioClip successSound;
    public AudioClip failSound;
    public AudioClip stirLoopSound;

    [Header("Color Settings")]
    public string shallowColorName = "_ShallowColor";
    public string deepColorName = "_DeepColor";
    public float colorTransitionTime = 1.2f;
    [Range(0, 1)] public float baseColorInfluence = 0.4f; // 初始锅水对整体颜色的稀释权重

    [Header("Spawn Settings")]
    public Transform spawnPoint;
    public GameObject junkPotionPrefab;

    private Color originalLiquidColor;
    private Coroutine colorChangeCoroutine;
    private float stirFadeSpeed = 8f;

    void Start()
    {
        // 初始化基础颜色
        if (liquidRenderer != null && liquidRenderer.material.HasProperty(shallowColorName))
        {
            originalLiquidColor = liquidRenderer.material.GetColor(shallowColorName);
            internalMixedColor = originalLiquidColor;
        }

        // 初始化 UI
        if (uiCanvasGroup != null) uiCanvasGroup.SetActive(false);
        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = stirRequired;
            progressSlider.value = 0;
        }

        // 音频组件初始化
        if (cauldronAudioSource == null) cauldronAudioSource = GetComponent<AudioSource>();

        if (stirringAudioSource != null && stirLoopSound != null)
        {
            stirringAudioSource.clip = stirLoopSound;
            stirringAudioSource.loop = true;
            stirringAudioSource.volume = 0;
            stirringAudioSource.Play();
        }
    }

    void Update()
    {
        // 搅拌声音逻辑：仅当进度条在移动时播放
        if (stirringAudioSource != null)
        {
            bool isStirringNow = currentStirProgress > lastFrameStirProgress && !isFinished;
            float targetVolume = isStirringNow ? 1f : 0f;
            stirringAudioSource.volume = Mathf.Lerp(stirringAudioSource.volume, targetVolume, Time.deltaTime * stirFadeSpeed);
            lastFrameStirProgress = currentStirProgress;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient != null)
        {
            PlaySound(splashSound);
            float addedAmount = ingredient.amount;

            // --- 物理级自然颜色混合算法 ---
            Vector3 newColorVector = new Vector3(ingredient.ingredientColor.r, ingredient.ingredientColor.g, ingredient.ingredientColor.b);

            if (totalIngredientAmount == 0)
            {
                // 第一份材料：将锅水原色作为底色加入累加池
                Vector3 baseColorVector = new Vector3(originalLiquidColor.r, originalLiquidColor.g, originalLiquidColor.b);

                // 初始权重 = 基础权重 + 第一份材料分量
                colorRGBAccumulator = (baseColorVector * baseColorInfluence) + (newColorVector * addedAmount);
                totalIngredientAmount = baseColorInfluence + addedAmount;
            }
            else
            {
                colorRGBAccumulator += newColorVector * addedAmount;
                totalIngredientAmount += addedAmount;
            }

            // 计算平均值并转换为颜色
            Vector3 finalRGB = colorRGBAccumulator / totalIngredientAmount;
            internalMixedColor = new Color(finalRGB.x, finalRGB.y, finalRGB.z, 1.0f);

            // 记录配方
            if (currentMix.ContainsKey(ingredient.ingredientName))
                currentMix[ingredient.ingredientName] += addedAmount;
            else
                currentMix.Add(ingredient.ingredientName, addedAmount);

            // 粒子与变色
            if (splashParticle != null)
            {
                splashParticle.transform.position = other.transform.position + Vector3.up * 0.1f;
                var main = splashParticle.main;
                main.startColor = ingredient.ingredientColor;
                splashParticle.Stop();
                splashParticle.Play();
            }

            if (liquidRenderer != null)
            {
                if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
                colorChangeCoroutine = StartCoroutine(SmoothChangeColor(internalMixedColor));
            }

            other.gameObject.SetActive(false);
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

        NPCOrderUI targetNPC = Object.FindFirstObjectByType<NPCOrderUI>();
        RecipeData targetRecipe = null;
        if (targetNPC != null) targetRecipe = targetNPC.GetMyOrder();

        if (CheckSuccess(targetRecipe))
            HandleSuccess(targetRecipe);
        else
            HandleFailure();
    }

    void HandleSuccess(RecipeData targetRecipe)
    {
        PlaySound(successSound);
        if (successParticle != null) successParticle.Play();
        if (GameDataTracker.Instance != null) GameDataTracker.Instance.potionsMade++;
        if (targetRecipe != null && targetRecipe.potionPrefab != null)
            Instantiate(targetRecipe.potionPrefab, spawnPoint.position, spawnPoint.rotation);
        ResetCauldron();
    }

    void HandleFailure()
    {
        PlaySound(failSound);
        if (failParticle != null) failParticle.Play();
        if (junkPotionPrefab != null) Instantiate(junkPotionPrefab, spawnPoint.position, spawnPoint.rotation);

        if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
        colorChangeCoroutine = StartCoroutine(SmoothChangeColor(Color.black));
        ResetCauldron();
    }

    private void PlaySound(AudioClip clip)
    {
        if (cauldronAudioSource != null && clip != null)
            cauldronAudioSource.PlayOneShot(clip);
    }

    IEnumerator SmoothChangeColor(Color targetShallow)
    {
        Material mat = liquidRenderer.material;
        Color startShallow = mat.GetColor(shallowColorName);
        Color targetDeep = targetShallow * 0.35f;
        targetDeep.a = 1.0f;

        float elapsed = 0;
        while (elapsed < colorTransitionTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / colorTransitionTime);
            mat.SetColor(shallowColorName, Color.Lerp(startShallow, targetShallow, t));
            if (mat.HasProperty(deepColorName))
            {
                Color startDeep = mat.GetColor(deepColorName);
                mat.SetColor(deepColorName, Color.Lerp(startDeep, targetDeep, t));
            }
            yield return null;
        }
        mat.SetColor(shallowColorName, targetShallow);
    }

    public void ResetCauldron()
    {
        currentMix.Clear();
        currentStirProgress = 0f;
        lastFrameStirProgress = 0f;
        totalIngredientAmount = 0f;
        colorRGBAccumulator = Vector3.zero; // 重置颜色池
        isFinished = false;
        if (progressSlider != null) progressSlider.value = 0;
        Invoke(nameof(StartResetColor), 3.0f);
    }

    void StartResetColor()
    {
        if (colorChangeCoroutine != null) StopCoroutine(colorChangeCoroutine);
        colorChangeCoroutine = StartCoroutine(SmoothChangeColor(originalLiquidColor));
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
}