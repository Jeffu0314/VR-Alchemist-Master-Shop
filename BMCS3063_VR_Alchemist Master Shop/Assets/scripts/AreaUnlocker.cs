using UnityEngine;
using TMPro;
using System.Collections; // 必须加上，因为要用协程

public class AreaUnlocker : MonoBehaviour
{
    public int unlockPrice = 1000;      
    public GameObject blockerObject;  

    public GameObject darknessVisualRoot;
    public float dissolveTime = 2.5f;   

    public TextMeshPro priceText;      

    private bool isUnlocked = false;

    void Start()
    {
        if (priceText != null)
        {
            priceText.text = $"Cost: {unlockPrice} Gold";
            priceText.color = new Color(0.5f, 0f, 0f, 1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isUnlocked)
        {
            TryUnlock();
        }
    }

    public void TryUnlock()
    {
        if (AlchemyManager.Instance != null && AlchemyManager.Instance.SpendCoins(unlockPrice))
        {
            PerformUnlock();
        }
        else
        {
            if (priceText != null) StartCoroutine(FlashPriceRed());
        }
    }

    void PerformUnlock()
    {
        isUnlocked = true;

        

        if (blockerObject != null)
        {
            blockerObject.SetActive(false);
        }

        if (darknessVisualRoot != null)
        {
            StartCoroutine(DissolveDarknessRoutine());
        }

        if (priceText != null) priceText.gameObject.SetActive(false);

        this.enabled = false;
    }

    private IEnumerator DissolveDarknessRoutine()
    {
        ParticleSystem[] particleSystems = darknessVisualRoot.GetComponentsInChildren<ParticleSystem>();
      
        Light[] lights = darknessVisualRoot.GetComponentsInChildren<Light>();

        foreach (var ps in particleSystems)
        {
            var emission = ps.emission;
            emission.enabled = false;
        }

        float elapsed = 0;
        while (elapsed < dissolveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dissolveTime;

            foreach (var l in lights)
            {
                l.intensity = Mathf.Lerp(l.intensity, 0f, t);
            }
            yield return null;
        }

        Destroy(darknessVisualRoot);
    }

    private IEnumerator FlashPriceRed()
    {
        if (priceText == null) yield break;
        priceText.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        priceText.color = new Color(0.5f, 0f, 0f, 1f);
    }
}