using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RecipeBookVisual : MonoBehaviour
{
    public GameObject recipesUI;

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnBookPickedUp);
            
            grabInteractable.selectExited.AddListener(OnBookDropped);
        }

        if (recipesUI != null)
        {
            recipesUI.SetActive(false);
        }
    }

    private void OnBookPickedUp(SelectEnterEventArgs args)
    {
        if (recipesUI != null)
        {
            recipesUI.SetActive(true);
        }
    }

    private void OnBookDropped(SelectExitEventArgs args)
    {
        if (recipesUI != null)
        {
            recipesUI.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnBookPickedUp);
            grabInteractable.selectExited.RemoveListener(OnBookDropped);
        }
    }
}