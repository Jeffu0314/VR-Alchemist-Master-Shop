using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

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
        SpawnNextNPC();
    }

    public void SpawnNextNPC()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(spawnDelay);

        if (npcPrefabs.Count > 0 && spawnPoint != null)
        {
            int randomIndex = Random.Range(0, npcPrefabs.Count);
            GameObject selectedPrefab = npcPrefabs[randomIndex];

            Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);

        }
    }
}