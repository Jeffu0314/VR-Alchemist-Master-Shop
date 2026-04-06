using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public enum NPCState { Entering, Waiting, Leaving }
    public NPCState currentState;

    private NavMeshAgent agent;
    private Transform orderPoint;
    private Transform exitPoint;

    // 距离判定阈值 (根据你的模型大小，如果 NPC 走不到位，可以稍微调大到 0.8f)
    [SerializeField] private float stopDistance = 0.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // 1. 自动寻找场景中的关键点位置
        // 注意：确保你的 Hierarchy 场景里确实有这两个名字一模一样的物体
        GameObject op = GameObject.Find("OrderPoint");
        GameObject ep = GameObject.Find("ExitPoint");

        if (op != null) orderPoint = op.transform;
        else Debug.LogError("NPC: 找不到场景中的 'OrderPoint'，请检查命名！");

        if (ep != null) exitPoint = ep.transform;
        else Debug.LogError("NPC: 找不到场景中的 'ExitPoint'，请检查命名！");

        // 2. 游戏开始，立刻走向柜台
        GoToCounter();
    }

    void Update()
    {
        // 状态机逻辑
        switch (currentState)
        {
            case NPCState.Entering:
                // 检查是否到达柜台
                if (!agent.pathPending && agent.remainingDistance <= stopDistance)
                {
                    OnReachedCounter();
                }
                break;

            case NPCState.Leaving:
                // 检查是否到达出口
                if (!agent.pathPending && agent.remainingDistance <= stopDistance)
                {
                    OnReachedExit();
                }
                break;
        }
    }

    // --- 状态切换函数 ---

    void GoToCounter()
    {
        if (orderPoint == null) return;

        currentState = NPCState.Entering;
        agent.SetDestination(orderPoint.position);
        Debug.Log("<color=orange>NPC:</color> 正在进入商店，前往柜台...");
    }

    void OnReachedCounter()
    {
        if (currentState == NPCState.Waiting) return; // 防止重复触发

        currentState = NPCState.Waiting;

        // 停止移动，保持站立
        agent.ResetPath();

        // --- 关键：在这里生成随机订单 ---
        if (AlchemyManager.Instance != null)
        {
            AlchemyManager.Instance.AssignRandomOrder();
            Debug.Log("<color=yellow>NPC:</color> 已到达柜台，发出订单：" + AlchemyManager.Instance.currentCustomerOrder.potionName);
        }
    }

    // 供 NPCReceiver 调用的函数
    public void OnReceivePotion()
    {
        if (exitPoint == null || currentState == NPCState.Leaving) return;

        currentState = NPCState.Leaving;

        // 先清除之前的路径，再设置新目的地
        agent.ResetPath();
        agent.SetDestination(exitPoint.position);

        Debug.Log("<color=blue>NPC:</color> 拿到药水了，这就离开！");
    }

    void OnReachedExit()
    {
        Debug.Log("<color=red>NPC:</color> 已到达出口，销毁并请求下一个。");

        // 1. 走到门口了，告诉管理器可以准备下一个 NPC 了
        if (NPCManager.Instance != null)
        {
            NPCManager.Instance.SpawnNextNPC();
        }

        // 2. 销毁自己
        Destroy(gameObject);
    }
}