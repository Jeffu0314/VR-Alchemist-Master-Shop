using UnityEngine;

[ExecuteInEditMode]
public class CauldronManualColor : MonoBehaviour
{
    public Renderer liquidRenderer;

    public Color waterColor = Color.cyan;

    [Range(1f, 10f)] public float glowIntensity = 2.5f;

    public string shallowReference = "Color_F01C36BF"; 
    public string deepReference = "Color_7D9A58EC";    

    void Update()
    {
        if (liquidRenderer == null) return;

        Material mat = liquidRenderer.sharedMaterial;

        if (mat == null) return;

        Vector4 finalColorVector = new Vector4(
            waterColor.r * glowIntensity,
            waterColor.g * glowIntensity,
            waterColor.b * glowIntensity,
            waterColor.a
        );

        if (mat.HasProperty(shallowReference))
        {
            mat.SetVector(shallowReference, finalColorVector);
        }

        if (mat.HasProperty(deepReference))
        {
            mat.SetVector(deepReference, finalColorVector * 0.2f);
        }
    }
}