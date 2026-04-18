using UnityEngine;
using TMPro;

public class RecipeUnlocker : MonoBehaviour
{
    public RecipeData recipeToUnlock;    
    public int unlockPrice = 500;       
    public GameObject shelfVisualGroup; 

    public TextMeshPro priceText;

    private bool isUnlocked = false;

    void Start()
    {
        if (priceText != null) priceText.text = $"{unlockPrice} Gold";

        if (shelfVisualGroup != null) shelfVisualGroup.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isUnlocked)
        {
            TryPurchase();
        }
    }

    void TryPurchase()
    {
        if (AlchemyManager.Instance != null && AlchemyManager.Instance.currentCoins >= unlockPrice)
        {
            UnlockContent();
        }
    }

    void UnlockContent()
    {
        isUnlocked = true;

        AlchemyManager.Instance.currentCoins -= unlockPrice;

        AlchemyManager.Instance.UnlockNewPotion(recipeToUnlock);

        if (shelfVisualGroup != null)
        {
            shelfVisualGroup.SetActive(true);
        }

        if (priceText != null) priceText.gameObject.SetActive(false);
    }
}