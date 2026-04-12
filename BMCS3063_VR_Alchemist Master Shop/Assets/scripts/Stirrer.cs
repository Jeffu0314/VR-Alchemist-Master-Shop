using UnityEngine;

public class Stirrer : MonoBehaviour
{
    public float stirSpeedThreshold = 0.05f; // 降低一点阈值，让搅拌更灵敏
    private Vector3 lastPosition;

    [Header("手动拖入你的 Cauldron 物体")]
    public Cauldron currentCauldron;

    private bool isInsideLiquid = false;

    void Start()
    {
        lastPosition = transform.position;

        // 如果你忘了拖引用，脚本会自动在场景里找那唯一的锅
        if (currentCauldron == null)
        {
            currentCauldron = Object.FindFirstObjectByType<Cauldron>();
        }
    }

    void Update()
    {
        if (currentCauldron == null || !isInsideLiquid) return;

        // 计算移动距离
        float distance = Vector3.Distance(transform.position, lastPosition);

        if (distance > stirSpeedThreshold)
        {
            // 只有在液面以下移动才增加进度
            currentCauldron.AddStirProgress(distance);
        }

        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 只要碰到液面标签，就开始搅拌逻辑
        if (other.CompareTag("CauldronLiquid"))
        {
            isInsideLiquid = true;
            Debug.Log("<color=cyan>搅拌棒已入水</color>");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CauldronLiquid"))
        {
            isInsideLiquid = false;
            Debug.Log("<color=white>搅拌棒已离开液面</color>");
        }
    }
}