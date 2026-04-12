using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public enum NPCState { Entering, Waiting, Leaving }
    public NPCState currentState;

    private NavMeshAgent agent;
    private Animator anim; // 新增：动画组件
    private Transform orderPoint;
    private Transform exitPoint;

    // 距离判定阈值
    [SerializeField] private float stopDistance = 0.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // --- 新增：初始化动画组件 ---
        anim = GetComponent<Animator>();
        if (anim != null)
        {
            // 确保由 Agent 控制位移，防止模型“瞬移”或“乱飘”
            anim.applyRootMotion = false;
        }

        // 1. 自动寻找场景中的关键点位置
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
        // --- 新增：每帧同步物理速度到动画参数 ---
        if (anim != null && agent != null)
        {
            // 获取 Agent 的实时移动速率
            float currentSpeed = agent.velocity.magnitude;
            // 把这个值传给 Animator 里的 "Speed" 参数
            anim.SetFloat("Speed", currentSpeed);
        }

        // 原有的状态机逻辑完全保留
        switch (currentState)
        {
            case NPCState.Entering:
                if (!agent.pathPending && agent.remainingDistance <= stopDistance)
                {
                    OnReachedCounter();
                }
                break;

            case NPCState.Leaving:
                if (!agent.pathPending && agent.remainingDistance <= stopDistance)
                {
                    OnReachedExit();
                }
                break;
        }
    }

    // --- 原有状态切换函数（完全保留） ---

    void GoToCounter()
    {
        if (orderPoint == null) return;

        currentState = NPCState.Entering;
        agent.SetDestination(orderPoint.position);
        Debug.Log("<color=orange>NPC:</color> 正在进入商店，前往柜台...");
    }

    void OnReachedCounter()
    {
        if (currentState == NPCState.Waiting) return;

        currentState = NPCState.Waiting;
        agent.ResetPath();

        // --- 核心修正：不再调用 AssignRandomOrder ---
        // 而是通知自己身上的 NPCOrderUI 脚本开始工作
        NPCOrderUI myUI = GetComponent<NPCOrderUI>();
        if (myUI != null)
        {
            myUI.StartOrderProcess();
            Debug.Log("<color=yellow>NPC:</color> 已到达柜台，通知 UI 脚本领单并计时。");
        }
        else
        {
            Debug.LogError("NPC: 找不到 NPCOrderUI 脚本，请检查 Prefab 是否挂载了该组件！");
        }
    }

    public void OnReceivePotion()
    {
        if (exitPoint == null || currentState == NPCState.Leaving) return;

        currentState = NPCState.Leaving;
        agent.ResetPath();
        agent.SetDestination(exitPoint.position);

        Debug.Log("<color=blue>NPC:</color> 拿到药水了，这就离开！");
    }

    void OnReachedExit()
    {
        Debug.Log("<color=red>NPC:</color> 已到达出口，销毁并请求下一个。");

        if (NPCManager.Instance != null)
        {
            NPCManager.Instance.SpawnNextNPC();
        }

        Destroy(gameObject);
    }
}