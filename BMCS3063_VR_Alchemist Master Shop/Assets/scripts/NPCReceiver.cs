using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class NPCReceiver : MonoBehaviour
{
    private XRSocketInteractor socket;
    private bool isProcessing = false;

    void Awake() => socket = GetComponent<XRSocketInteractor>();

    public void OnPotionDelivered(SelectEnterEventArgs args)
    {
        if (isProcessing) return;

        PotionObject potion = args.interactableObject.transform.GetComponent<PotionObject>();
        NPCController currentNPC = Object.FindFirstObjectByType<NPCController>();

        if (currentNPC != null && currentNPC.currentState == NPCController.NPCState.Waiting)
        {
            if (potion != null && potion.potionType == AlchemyManager.Instance.currentCustomerOrder)
            {
                isProcessing = true;

                int rewardAmount = AlchemyManager.Instance.currentCustomerOrder.potionPrice;
                AlchemyManager.Instance.AddCoins(rewardAmount);

                if (GameDataTracker.Instance != null) GameDataTracker.Instance.customersServed++;

                CatOrderDisplay cat = Object.FindFirstObjectByType<CatOrderDisplay>();
                if (cat != null) cat.NotifyOrderSuccess();

                if (socket.interactionManager != null) socket.interactionManager.SelectExit(socket, args.interactableObject);
                currentNPC.OnReceivePotion();
                Destroy(potion.gameObject, 0.1f);

                Invoke(nameof(ResetProcessor), 2.0f);
            }
        }
    }

    public void ResetProcessor() => isProcessing = false;
}