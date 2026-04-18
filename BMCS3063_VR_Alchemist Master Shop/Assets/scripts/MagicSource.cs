using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MagicSource : MonoBehaviour
{
    public GameObject ingredientPrefab;

    private XRBaseInteractable sourceInteractable;

    void Start()
    {
        sourceInteractable = GetComponent<XRBaseInteractable>();
        sourceInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject;

        interactionManager.SelectExit(interactor, (IXRSelectInteractable)sourceInteractable);

        GameObject newObj = ObjectPooler.Instance.SpawnFromPool(ingredientPrefab, transform.position, transform.rotation);

        if (newObj != null)
        {
            XRGrabInteractable newGrab = newObj.GetComponent<XRGrabInteractable>();
            if (newGrab != null)
            {
                interactionManager.SelectEnter(interactor, (IXRSelectInteractable)newGrab);
            }
        }
    }

    private XRInteractionManager interactionManager => sourceInteractable.interactionManager;
}