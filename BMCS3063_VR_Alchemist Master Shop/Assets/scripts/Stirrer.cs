using UnityEngine;

public class Stirrer : MonoBehaviour
{
    public float stirSpeedThreshold = 0.1f; // 移动多快才算在有效搅拌
    private Vector3 lastPosition;
    public Cauldron currentCauldron;

    void Start() => lastPosition = transform.position;

    void Update()
    {
        if (currentCauldron == null) return;

        // 计算这一帧移动的距离
        float distance = Vector3.Distance(transform.position, lastPosition);
        if (distance > stirSpeedThreshold)
        {
            // 通知坩埚增加搅拌进度
            currentCauldron.AddStirProgress(distance);
        }
        lastPosition = transform.position;
    }

    // 进入坩埚触发区时激活
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CauldronLiquid"))
            currentCauldron = other.GetComponentInParent<Cauldron>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CauldronLiquid")) currentCauldron = null;
    }
}