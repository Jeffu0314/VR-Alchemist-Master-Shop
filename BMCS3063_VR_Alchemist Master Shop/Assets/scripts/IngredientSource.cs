using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class IngredientSource : MonoBehaviour
{
    [Header("设置生成的材料")]
    public GameObject ingredientPrefab; // 拖入你想要生成的那个材料 Prefab

    private XRBaseInteractable shelfInteractable;

    void Awake()
    {
        // 获取货架物体自带的抓取脚本
        shelfInteractable = GetComponent<XRBaseInteractable>();

        // 绑定：当这个东西被“选中”（按下抓取键）时触发
        if (shelfInteractable != null)
        {
            shelfInteractable.selectEntered.AddListener(OnGrabShelfItem);
        }
    }

    private void OnGrabShelfItem(SelectEnterEventArgs args)
    {
        // 1. 确定是哪只手在抓
        var interactor = args.interactorObject;

        // 2. 核心逻辑：立刻让这只手“松开”货架上的母体
        shelfInteractable.interactionManager.SelectExit(interactor, shelfInteractable);

        // 3. 从对象池里拿一个新材料
        GameObject newIngredient = ObjectPooler.Instance.SpawnFromPool(
            ingredientPrefab,
            transform.position,
            transform.rotation
        );

        if (newIngredient != null)
        {
            // 4. 获取新材料身上的抓取组件
            var newGrab = newIngredient.GetComponent<XRGrabInteractable>();

            if (newGrab != null)
            {
                // 解决你说的“拿一次就拿不起来”的问题：强制刷新组件状态
                newGrab.enabled = false;
                newGrab.enabled = true;

                // 5. 强行让手柄“握住”这个新生成的材料
                shelfInteractable.interactionManager.SelectEnter(interactor, newGrab);
            }
        }
    }

    private void OnDestroy()
    {
        // 记得解绑，防止内存泄漏
        if (shelfInteractable != null)
        {
            shelfInteractable.selectEntered.RemoveListener(OnGrabShelfItem);
        }
    }
}