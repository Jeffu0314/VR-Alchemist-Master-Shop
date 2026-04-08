using UnityEngine;
using System.Collections.Generic;

public class NPCAudioFeedback : MonoBehaviour
{
    [Header("组件引用")]
    public AudioSource audioSource;

    [Header("音效库 (点 + 号随时增加新音效)")]
    public List<AudioClip> reactionClips = new List<AudioClip>();

    [Header("随机音调设置")]
    public float minPitch = 0.85f;
    public float maxPitch = 1.15f;

    [Header("冷却设置")]
    public float minInterval = 0.4f;
    private float lastSoundTime;

    private void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource != null) audioSource.spatialBlend = 1.0f;
    }

    // 处理物理碰撞 (Non-Trigger)
    private void OnCollisionEnter(Collision collision)
    {
        CheckAndPlay(collision.gameObject);
    }

    // 处理触发器碰撞 (Is-Trigger)
    private void OnTriggerEnter(Collider other)
    {
        CheckAndPlay(other.gameObject);
    }

    private void CheckAndPlay(GameObject hitObject)
    {
        // --- 核心判断逻辑 ---

        // 1. 检查是不是材料或药水
        bool isItem = hitObject.GetComponent<Ingredient>() != null ||
                      hitObject.GetComponent<PotionObject>() != null;

       
        bool isStirrer = hitObject.GetComponent<Stirrer>() != null;

        // 只要满足其中一个条件，且音效库不为空，且过了冷却时间
        if ((isItem || isStirrer) && reactionClips.Count > 0 && Time.time > lastSoundTime + minInterval)
        {
            PlayRandomReaction();
        }
    }

    void PlayRandomReaction()
    {
        if (audioSource == null) return;

        int randomIndex = Random.Range(0, reactionClips.Count);
        AudioClip clipToPlay = reactionClips[randomIndex];

        if (clipToPlay != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(clipToPlay);
            lastSoundTime = Time.time;

            Debug.Log($"<color=orange>NPC被砸中:</color> 来源: {clipToPlay.name}");
        }
    }
}