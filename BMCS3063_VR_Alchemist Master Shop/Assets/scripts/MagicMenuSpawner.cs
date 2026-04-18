using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MagicMenuSpawner : MonoBehaviour
{
    public CanvasGroup menuCanvasGroup; 
    public ParticleSystem spawnParticles;  
    public float startDelay = 1.0f;   
    public float fadeDuration = 1.5f;  

    void Start()
    {
        menuCanvasGroup.alpha = 0;
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;

        StartCoroutine(SpawnMenuRoutine());
    }

    IEnumerator SpawnMenuRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        spawnParticles.Play();

        yield return new WaitForSeconds(0.5f);

        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            menuCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        menuCanvasGroup.alpha = 1;
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;
    }
}