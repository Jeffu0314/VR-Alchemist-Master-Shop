using UnityEngine;
using TMPro;

public class SmoothRainbowText : MonoBehaviour
{
    private TextMeshProUGUI textComponent;

    public float speed = 0.5f;

    public float saturation = 0.8f;

    public float brightness = 1.0f;

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (textComponent == null) return;

        float hue = (Time.time * speed) % 1.0f;

        Color finalColor = Color.HSVToRGB(hue, saturation, brightness);

        textComponent.color = finalColor;
    }
}