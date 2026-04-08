using UnityEngine;

public class UIBillboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // 自动获取主摄像机（VR头显）的 Transform
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    // 使用 LateUpdate 确保在相机移动后才更新 UI 角度，防止抖动
    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            // 让 UI 始终注视相机
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                             mainCameraTransform.rotation * Vector3.up);

            // 如果你发现 UI 是反的，可以尝试直接用这行最简单的：
            // transform.LookAt(mainCameraTransform);
        }
    }
}