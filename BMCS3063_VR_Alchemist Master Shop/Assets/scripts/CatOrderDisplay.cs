using UnityEngine;
using TMPro;
using System.Collections;

public class CatOrderDisplay : MonoBehaviour
{
    public TextMeshProUGUI orderText;

    public AudioSource catAudioSource; 
    public AudioClip newOrderSound;   
    public AudioClip successSound;     

    public Color activeOrderColor = Color.black;
    public Color waitingColor = Color.gray;
    public Color successColor = Color.green;

    private bool isShowingSuccess = false;
    private int completedOrderCount = 0;
    private object lastServedOrder = null;

    private bool hadOrderLastFrame = false;

    void Start()
    {
        if (orderText == null)
        {
            Debug.LogError("CatOrderDisplay: 请在 Inspector 中绑定 OrderText!");
            enabled = false;
        }

        if (catAudioSource == null) catAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isShowingSuccess) return;

        UpdateCatDisplay();
    }

    void UpdateCatDisplay()
    {
        if (AlchemyManager.Instance == null) return;

        var currentOrder = AlchemyManager.Instance.currentCustomerOrder;

        bool hasValidNewOrder = currentOrder != null && currentOrder != lastServedOrder;

        if (hasValidNewOrder)
        {
            if (!hadOrderLastFrame)
            {
                PlaySound(newOrderSound);
                hadOrderLastFrame = true; 
            }

            orderText.text = $"<size=120%><b>ORDER {completedOrderCount + 1}</b></size>\n\n" +
                             $"Need: <color=purple>{currentOrder.potionName}</color>\n" +
                             $"Reward: <color=yellow>{currentOrder.potionPrice}</color> Gold";
            orderText.color = activeOrderColor;
        }
        else
        {
            orderText.text = $"<i>Meow~\nWaiting for Customer {completedOrderCount + 1}...</i>";
            orderText.color = waitingColor;

            hadOrderLastFrame = false;
        }
    }

    public void NotifyOrderSuccess()
    {
        lastServedOrder = AlchemyManager.Instance.currentCustomerOrder;
        hadOrderLastFrame = false; 

        StopAllCoroutines();
        StartCoroutine(ShowSuccessFeedback());
    }

    IEnumerator ShowSuccessFeedback()
    {
        isShowingSuccess = true;

        if (successSound != null) PlaySound(successSound);

        completedOrderCount++;

        orderText.text = $"<size=150%><b> SUCCESS! </b></size>\n\n" +
                         $"<color=green>Order #{completedOrderCount} Paid!</color>\n" +
                         "Great job, Master!";
        orderText.color = successColor;

        yield return new WaitForSeconds(3.0f);

        isShowingSuccess = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (catAudioSource != null && clip != null)
        {
            catAudioSource.PlayOneShot(clip);
        }
    }
}