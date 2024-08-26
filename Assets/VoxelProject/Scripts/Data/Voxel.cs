using System;
using System.Drawing;
using Unity.Collections;
using static UnityEngine.Rendering.DebugUI;
using Unity.VisualScripting;
using UnityEngine;

/**
* A VoxelCell represents information about a particular type of cell.
*/
public struct Voxel
{
/*    public readonly float colorR;
    public readonly float colorG;
    public readonly float colorB;*/
    public readonly int colorGrayScale;

    public readonly bool isSegmentVoxel;

    public Voxel(int colorGreyScale)
    {
/*        this.colorR = colorR;
        this.colorG = colorG;
        this.colorB = colorB;
*/      
        this.colorGrayScale = colorGreyScale;
        isSegmentVoxel = false;
    }

    public Voxel(int colorGreyScale, bool isSegmentVoxel)
    {
/*        this.colorR = colorR;
        this.colorG = colorG;
        this.colorB = colorB;
*/
        this.colorGrayScale = colorGreyScale;
        this.isSegmentVoxel = isSegmentVoxel;
    }

    public UnityEngine.Color color()
    {
        // Normalize the grayscale value to the range [0, 1]
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
        // Normalize the grayscale value to the range [0, 1]
        float normalizedValue = colorGrayScale / 254f;

        return new UnityEngine.Color(normalizedValue, normalizedValue, normalizedValue);
    }
}