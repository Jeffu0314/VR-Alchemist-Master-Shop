using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

    [Header("NPC Settings")]
    // 1. 修改这里：把单一的 Prefab 变成一个列表
    [Tooltip("把你的 5 个不同样子的 NPC 预制体拖进这个列表")]
    public List<GameObject> npcPrefabs = new List<GameObject>();

    public Transform spawnPoint;
    public float spawnDelay = 3f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 游戏开始，生成第一个随机 NPC
        SpawnNextNPC();
    }

    public void SpawnNextNPC()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        Debug.Log("<color=lightblue>系统:</color> 正在准备下一位顾客...");
        yield return new WaitForSeconds(spawnDelay);

        // 2. 核心逻辑：从 5 个预制体里随机选一个
        if (npcPrefabs.Count > 0 && spawnPoint != null)
        {
            int randomIndex = Random.Range(0, npcPrefabs.Count);
            GameObject selectedPrefab = npcPrefabs[randomIndex];

            // 3. 生成选中的那个随机 NPC
            Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);

            Debug.Log($"<color=lightblue>系统:</color> 一位新的顾客 ({selectedPrefab.name}) 进入了商店。");
        }
        else
        {
            Debug.LogError("NPCManager: npcPrefabs 列表为空或没有设置 SpawnPoint！");
        }
    }
}