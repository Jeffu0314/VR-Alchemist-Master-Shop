using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WorldBoundary : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ingredient") || other.CompareTag("Potion"))
        {
            ForceReleaseAndRecycle(other.gameObject);
        }
    }

    private void ForceReleaseAndRecycle(GameObject obj)
    {
        XRGrabInteractable interactable = obj.GetComponent<XRGrabInteractable>();

        if (interactable != null && interactable.isSelected)
        {
            XRInteractionManager manager = interactable.interactionManager;
            if (manager != null)
            {
                var interactors = interactable.interactorsSelecting;
                for (int i = interactors.Count - 1; i >= 0; i--)
                {
                    manager.SelectExit(interactors[i], (IXRSelectInteractable)interactable);
                }
            }
        }

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.Sleep();
        }

        obj.SetActive(false);

    }
}