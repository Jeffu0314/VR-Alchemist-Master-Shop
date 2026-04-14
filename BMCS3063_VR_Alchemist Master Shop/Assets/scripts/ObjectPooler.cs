using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit; // 必须添加这个命名空间

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int size = 20;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            if (pool.prefab == null) continue;

            string poolKey = pool.prefab.name;
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                // 建议：将生成的物体设为 ObjectPooler 的子物体，防止场景列表太乱
                obj.transform.SetParent(this.transform);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(poolKey, objectPool);
        }
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;

        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"池子里没有名为 {key} 的资源！");
            return null;
        }

        // 1. 获取物体
        GameObject objectToSpawn = poolDictionary[key].Dequeue();

        // 2. 核心修复：在激活前设置位置，防止触发旧位置的物理碰撞
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        // 3. 核心修复：重置物理状态
        Rigidbody rb = objectToSpawn.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // Unity 2023+ 写法
            rb.angularVelocity = Vector3.zero;
            rb.WakeUp(); // 强制唤醒
        }

        // 4. 核心修复：针对 XR 抓取物体的刷新逻辑
        var grabInteractable = objectToSpawn.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // --- 关键添加：强制刷新交互组件 ---
            // 很多时候 XR 组件在物体禁用后会卡在“已抓取”或“非法交互”状态
            // 重新开关一次组件能强制它重新扫描 Interactors
            grabInteractable.enabled = false;
            grabInteractable.enabled = true;

            // --- 关键添加：解除父子关系（防止物体粘在货架上） ---
            // 如果你发现物体拿不动，可能是它还把自己当成货架的子物体
            objectToSpawn.transform.SetParent(null);
        }

        // 5. 循环入队，为下一次使用做准备
        poolDictionary[key].Enqueue(objectToSpawn);
        return objectToSpawn;
    }
}