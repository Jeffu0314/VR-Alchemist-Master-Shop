using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    public AudioSource coinAudioSource; 
    public AudioClip coinGainSound;    

    private int lastCoinAmount = 0;    

    void Start()
    {
        if (AlchemyManager.Instance != null)
        {
            lastCoinAmount = AlchemyManager.Instance.currentCoins;
            UpdateText(lastCoinAmount);
        }
    }

    void Update()
    {
        if (AlchemyManager.Instance == null || coinText == null) return;

        int currentCoins = AlchemyManager.Instance.currentCoins;

        if (currentCoins > lastCoinAmount)
        {
            PlayCoinSound();
            UpdateText(currentCoins);
            lastCoinAmount = currentCoins; 
        }
        else if (currentCoins < lastCoinAmount)
        {
            UpdateText(currentCoins);
            lastCoinAmount = currentCoins;
        }
    }

    void UpdateText(int amount)
    {
        coinText.text = amount.ToString() + " G";
    }

    void PlayCoinSound()
    {
        if (coinAudioSource != null && coinGainSound != null)
        {
            coinAudioSource.PlayOneShot(coinGainSound);
        }
    }
}