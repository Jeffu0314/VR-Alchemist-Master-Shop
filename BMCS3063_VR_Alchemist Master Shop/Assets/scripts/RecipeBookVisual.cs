using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RecipeBookVisual : MonoBehaviour
{
    [Header("UI 设置")]
    [Tooltip("这里拖入你书本上的那个 Canvas 或者 UI 父物体")]
    public GameObject recipesUI;

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        // 自动获取书本上的抓取组件
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            // 订阅抓取事件：拿起书显示 UI
            grabInteractable.selectEntered.AddListener(OnBookPickedUp);
            // 订阅放下事件：放下书隐藏 UI
            grabInteractable.selectExited.AddListener(OnBookDropped);
        }

        // 游戏开始时默认隐藏 UI
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
            // 拿起时可以播个翻书音效（可选）
            Debug.Log("书被拿起了，显示配方");
        }
    }

    private void OnBookDropped(SelectExitEventArgs args)
    {
        if (recipesUI != null)
        {
            recipesUI.SetActive(false);
            Debug.Log("书放下了，隐藏配方");
        }
    }

    private void OnDestroy()
    {
        // 脚本销毁时移除监听，防止内存泄漏
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnBookPickedUp);
            grabInteractable.selectExited.RemoveListener(OnBookDropped);
        }
    }
}