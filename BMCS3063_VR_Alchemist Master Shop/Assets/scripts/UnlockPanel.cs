using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockPanel : MonoBehaviour
{
    public RecipeData recipeData;
    public int price = 500;

    public TextMeshProUGUI titleText;
    public Image potionIcon;
    public Button unlockButton;
    public TextMeshProUGUI priceText;

    public GameObject shelfItems;

    public AudioSource uiAudioSource;  
    public AudioClip buySuccessSound;  
    public AudioClip buyFailSound;     

    void Start()
    {
        if (recipeData != null)
        {
            titleText.text = recipeData.potionName;
        }

        priceText.text = price + " G";

        // 自动获取音效源，如果面板没拖的话
        if (uiAudioSource == null && UISoundManager.Instance != null)
        {
            uiAudioSource = UISoundManager.Instance.uiAudioSource;
        }
    }

    public void AttemptUnlock()
    {
        if (AlchemyManager.Instance != null && AlchemyManager.Instance.currentCoins >= price)
        {
            PlaySound(buySuccessSound);

            AlchemyManager.Instance.currentCoins -= price;

            AlchemyManager.Instance.UnlockNewPotion(recipeData);

            if (shelfItems != null) shelfItems.SetActive(true);

            gameObject.SetActive(false);
        }
        else
        {
            PlaySound(buyFailSound);

        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (uiAudioSource != null && clip != null)
        {
            uiAudioSource.PlayOneShot(clip);
        }
    }
}