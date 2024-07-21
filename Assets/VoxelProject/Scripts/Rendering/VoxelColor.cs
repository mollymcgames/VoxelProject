using UnityEngine;

[System.Serializable]
public class VoxelColor
{
    public Color32 color;
    public float metallic;
    public float smoothness;

    public VoxelColor(int r, int g, int b)
    {
        this.color = new Color32((byte)r, (byte)g, (byte)b, (byte)1);
        this.metallic = 0.0f;
        this.smoothness = 0.0f;
    }


    public VoxelColor(int r, int g, int b, float metallic, float smoothness)
    {
        this.color = new Color32((byte)r, (byte)g, (byte)b, (byte)1);
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