using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MagicSource : MonoBehaviour
{
    [Header("直接把这个材料的 Prefab 拖到这里")]
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

        // 1. 松开母体
        interactionManager.SelectExit(interactor, (IXRSelectInteractable)sourceInteractable);

        // 2. 从池子里变出来（直接传 Prefab 引用）
        GameObject newObj = ObjectPooler.Instance.SpawnFromPool(ingredientPrefab, transform.position, transform.rotation);

        if (newObj != null)
        {
            // 3. 自动抓取
            XRGrabInteractable newGrab = newObj.GetComponent<XRGrabInteractable>();
            if (newGrab != null)
            {
                interactionManager.SelectEnter(interactor, (IXRSelectInteractable)newGrab);
            }
        }
    }

    private XRInteractionManager interactionManager => sourceInteractable.interactionManager;
}