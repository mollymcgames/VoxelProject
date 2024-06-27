using System.Runtime.InteropServices;
using UnityEngine;

public struct Voxel
{
    // This struct has to be "blittable"!
    private byte isSolidInternal; // Use byte instead of bool
    public int colourRGBValue;

    public bool isSolid
    {
        get
        {
            return isSolidInternal != 0;
        }
        set
        {
            isSolidInternal = (byte)(value ? 1 : 0);
        }
    }

    public Voxel(bool isSolid, int colourRGBValue)
    {
        this.isSolidInternal = (byte)(isSolid ? 1 : 0);
        this.colourRGBValue = colourRGBValue;
    }
}