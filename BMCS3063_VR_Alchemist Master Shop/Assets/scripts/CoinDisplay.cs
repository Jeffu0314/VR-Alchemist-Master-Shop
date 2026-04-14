using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    // 每帧更新虽然消耗小，但我们可以优化为每秒更新几次，或者通过事件更新
    void Update()
    {
        if (AlchemyManager.Instance != null && coinText != null)
        {
            // 格式化显示：例如 "Coins: 1250 G"
            coinText.text = AlchemyManager.Instance.currentCoins.ToString() + " G";
        }
    }
}