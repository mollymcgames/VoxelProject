using System;
using System.Drawing;
using Unity.Collections;
using static UnityEngine.Rendering.DebugUI;
using Unity.VisualScripting;
using UnityEngine;

//Class to represent a single voxel
public class Voxel
{
    public readonly int colorGrayScale;

    public readonly bool isSegmentVoxel;

    public readonly Vector3Int position;

    public Voxel(int colorGreyScale)
    {
        this.colorGrayScale = colorGreyScale;
        isSegmentVoxel = false;
    }

    public Voxel(int colorGreyScale, Vector3Int position)
    {
        this.colorGrayScale = colorGreyScale;
        isSegmentVoxel = false;
        this.position = position;
    }

    public Voxel(int colorGreyScale, bool isSegmentVoxel)
    {
        this.colorGrayScale = colorGreyScale;
        this.isSegmentVoxel = isSegmentVoxel;
    }

    public Voxel(int colorGreyScale, bool isSegmentVoxel, Vector3Int position)
    {
        this.colorGrayScale = colorGreyScale;
        this.isSegmentVoxel = isSegmentVoxel;
        this.position = position;
    }

    public UnityEngine.Color color()
    {
        // Normalise the greyscale value to the range [0, 1]
        float normalizedValue = colorGrayScale / 254f;

        if (normalizedValue < 0.5f)
        {
            // Interpolate from red to green
            return UnityEngine.Color.Lerp(UnityEngine.Color.red, UnityEngine.Color.green, normalizedValue * 2f);
        }
        else
        {
            // Interpolate from green to blue
            return UnityEngine.Color.Lerp(UnityEngine.Color.green, UnityEngine.Color.blue, (normalizedValue - 0.5f) * 2f);
        }
    }

    public UnityEngine.Color colorInGrayScale()
    {
        // Normalise the greyscale value to the range [0, 1]
        float normalizedValue = colorGrayScale / 254f;

        return new UnityEngine.Color(normalizedValue, normalizedValue, normalizedValue);
    }
}