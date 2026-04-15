using UnityEngine;

// 这个标签让你在不运行游戏的情况下，直接在场景编辑界面调色就能看到变化
[ExecuteInEditMode]
public class CauldronManualColor : MonoBehaviour
{
    [Header("绑定水面的材质渲染器")]
    public Renderer liquidRenderer;

    [Header("手动调色盘")]
    public Color waterColor = Color.cyan;

    [Header("发光强度 (配合 Bloom 后处理)")]
    [Range(1f, 10f)] public float glowIntensity = 2.5f;

    [Header("Shader 内部 ID (身份证号)")]
    public string shallowReference = "Color_F01C36BF"; // 对应你 Shader 里的 Shallow Color
    public string deepReference = "Color_7D9A58EC";    // 对应你 Shader 里的 Deep Color

    void Update()
    {
        if (liquidRenderer == null) return;

        // 获取材质。在编辑器模式下建议用 sharedMaterial 以防产生大量材质实例
        Material mat = liquidRenderer.sharedMaterial;

        if (mat == null) return;

        // 计算带亮度的颜色向量 (Vector4)
        // 这里的乘法是为了让颜色突破 1.0，从而产生 HDR 发光效果
        Vector4 finalColorVector = new Vector4(
            waterColor.r * glowIntensity,
            waterColor.g * glowIntensity,
            waterColor.b * glowIntensity,
            waterColor.a
        );

        // 使用 SetVector 避开之前的 Color 转换报错
        if (mat.HasProperty(shallowReference))
        {
            mat.SetVector(shallowReference, finalColorVector);
        }

        if (mat.HasProperty(deepReference))
        {
            // 深色层通常是浅色的 20% 亮度，看起来更有层次感
            mat.SetVector(deepReference, finalColorVector * 0.2f);
        }
    }
}