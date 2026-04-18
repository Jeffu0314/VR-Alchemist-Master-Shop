using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro; 

public class VolumeSettings : MonoBehaviour
{
    public AudioMixer mainMixer;    
    public Slider volumeSlider;     
    public TextMeshProUGUI volumeLabel; 

    void Start()
    {
        volumeSlider.minValue = 0.0001f;
        volumeSlider.maxValue = 1f;

        float savedVol = PlayerPrefs.GetFloat("UserVolume", 0.8f);
        volumeSlider.value = savedVol;

        SetVolume(savedVol);

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float sliderValue)
    {
        float dB = Mathf.Log10(sliderValue) * 20;

        mainMixer.SetFloat("MyExposedVolume", dB);

        if (volumeLabel != null)
        {
            int displayPercentage = Mathf.RoundToInt(sliderValue * 100);
            volumeLabel.text = displayPercentage.ToString();
        }

        PlayerPrefs.SetFloat("UserVolume", sliderValue);
    }
}