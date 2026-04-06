using UnityEngine;
using UnityEngine.AI;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

    [Header("NPC Settings")]
    public GameObject npcPrefab;
    public Transform spawnPoint;
    public float delayBetweenNPCs = 3.0f;

    [Header("Debug")]
    public bool spawnOnStart = true;

    void Awake()
    {
        // 单例初始化
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (spawnOnStart)
        {
            SpawnNextNPC();
        }
    }

    // 供其他脚本（如 NPCReceiver 或 NPCController）调用的生成函数
    public void SpawnNextNPC()
    {
        // 关键修复：删除之前的 Object.FindFirstObjectByType 检查！
        // 因为调用这个函数时，旧 NPC 可能还没彻底从内存中移除。

        Debug.Log("<color=cyan>NPCManager:</color> 收到生成请求，" + delayBetweenNPCs + "秒后生成新顾客。");

        // 使用 CancelInvoke 确保不会因为意外重复调用产生多个 NPC
        CancelInvoke("CreateNPC");
        Invoke("CreateNPC", delayBetweenNPCs);
    }

    void CreateNPC()
    {
        if (npcPrefab == null || spawnPoint == null)
        {
            Debug.LogError("NPCManager: 请检查 Inspector 中的赋值！");
            return;
        }

        // 1. 生成 NPC 实例
        GameObject newNPC = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);

        // 建议：给生成的 NPC 改个名字，方便在层级面板看清楚
        newNPC.name = "Customer_NPC";

        // 2. 导航贴地处理
        NavMeshAgent agent = newNPC.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPoint.position, out hit, 1.5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }

        Debug.Log("<color=green>NPCManager:</color> 新顾客已进入商店。");
    }
}