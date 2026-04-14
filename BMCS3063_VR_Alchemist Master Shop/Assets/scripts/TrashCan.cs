using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 1. 检查进入的东西是不是药水（通过 Tag 或脚本）
        if (other.CompareTag("Potion"))
        {
            Debug.Log("<color=red>垃圾桶：正在回收废弃药水...</color>");

            // 2. 这里的逻辑非常简单：直接把它关掉
            // 因为你的 ObjectPooler 逻辑是只要 SetActive(false) 就算是回池子了
            other.gameObject.SetActive(false);

            // 3. 可选：播放一个“嗖”的音效或者垃圾桶震动动画
            PlayTrashEffect();
        }
    }

    void PlayTrashEffect()
    {
        // 以后可以在这里加特效
    }
}