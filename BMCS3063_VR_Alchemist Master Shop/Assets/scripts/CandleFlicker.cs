using UnityEngine;

public class CandleFlicker : MonoBehaviour
{
    private Light candleLight;
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 0.1f;

    void Start()
    {
        candleLight = GetComponent<Light>();
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * 5f, 0f);
        candleLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}