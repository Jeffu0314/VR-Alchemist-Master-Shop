using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WorldBoundary : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 统一识别所有材料（Ingredient）或药瓶（Potion）
        // 确保你那 20 多种材料的 Prefab 都设置了 "Ingredient" 标签
        if (other.CompareTag("Ingredient") || other.CompareTag("Potion"))
        {
            ForceReleaseAndRecycle(other.gameObject);
        }
    }

    private void ForceReleaseAndRecycle(GameObject obj)
    {
        // 1. 获取交互组件
        XRGrabInteractable interactable = obj.GetComponent<XRGrabInteractable>();

        // 2. 检查是否正在被抓取
        if (interactable != null && interactable.isSelected)
        {
            XRInteractionManager manager = interactable.interactionManager;
            if (manager != null)
            {
                // 获取当前正在抓取它的所有 Interactor
                // 使用列表副本防止在循环中解除交互导致的集合修改错误
                var interactors = interactable.interactorsSelecting;
                for (int i = interactors.Count - 1; i >= 0; i--)
                {
                    manager.SelectExit(interactors[i], (IXRSelectInteractable)interactable);
                }
            }
        }

        // 3. 彻底重置物理状态（防止下次从池子出来时带着之前的惯性）
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Unity 2023+ 使用 linearVelocity，旧版本请改回 velocity
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // 额外保险：如果是回收，强制重置受力状态
            rb.Sleep();
        }

        // 4. 执行对象池回收（隐藏即视为回收）
        obj.SetActive(false);

        Debug.Log($"<color=cyan>[Boundary]</color> 已安全释放并回收物体: {obj.name}");
    }
}