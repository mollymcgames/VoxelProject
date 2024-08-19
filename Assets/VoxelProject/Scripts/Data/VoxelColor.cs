using UnityEngine;

[System.Serializable]
public class VoxelColor
{
    public Color color;
    public float metallic;
    public float smoothness;

    public VoxelColor(float r, float g, float b)
    {
        this.color = new Color(r, g, b);
        this.metallic = 0.0f;
        this.smoothness = 0.0f;
    }


    public VoxelColor(float r, float g, float b, float metallic, float smoothness)
    {
        this.color = new Color(r, g, b);
        this.metallic = metallic;
        this.smoothness = smoothness;
    }

    public VoxelColor()
    {
        color = Color.white;
        metallic = 0.0f;
        smoothness = 0.0f;
    }
}