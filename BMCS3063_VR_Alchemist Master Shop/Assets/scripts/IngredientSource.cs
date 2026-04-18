using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class IngredientSource : MonoBehaviour
{
    public GameObject ingredientPrefab; 

    private XRBaseInteractable shelfInteractable;

    void Awake()
    {
        shelfInteractable = GetComponent<XRBaseInteractable>();

        if (shelfInteractable != null)
        {
            shelfInteractable.selectEntered.AddListener(OnGrabShelfItem);
        }
    }

    private void OnGrabShelfItem(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject;

        shelfInteractable.interactionManager.SelectExit(interactor, shelfInteractable);

        GameObject newIngredient = ObjectPooler.Instance.SpawnFromPool(
            ingredientPrefab,
            transform.position,
            transform.rotation
        );

        if (newIngredient != null)
        {
            var newGrab = newIngredient.GetComponent<XRGrabInteractable>();

            if (newGrab != null)
            {
                newGrab.enabled = false;
                newGrab.enabled = true;

                shelfInteractable.interactionManager.SelectEnter(interactor, newGrab);
            }
        }
    }

    private void OnDestroy()
    {
        if (shelfInteractable != null)
        {
            shelfInteractable.selectEntered.RemoveListener(OnGrabShelfItem);
        }
    }
}