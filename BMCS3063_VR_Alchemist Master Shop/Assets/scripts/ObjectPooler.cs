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
        // 如果物体带有 XRGrabInteractable，我们需要确保它在重新激活后状态是“干净”的
        var grabInteractable = objectToSpawn.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // 如果物体正处于某种奇怪的选中状态，强制重置
            // 这能解决“明明生成了却抓不起来”或者“手感僵死”的问题
        }

        poolDictionary[key].Enqueue(objectToSpawn);
        return objectToSpawn;
    }
}