using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public unsafe struct Voxel
{
    // This struct has to be "blittable"!
    private byte isSolidInternal; // Use byte instead of bool
    private byte isGreyScaleInternal; // Use byte instead of bool
    private byte isHotVoxelInternal; // Use byte instead of bool

    private fixed int colourRGBValue[10]; // Fixed-size array, adjust size as needed
    private int hotVoxelColourRBGValue;

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

    public bool isHotVoxel
    {
        get
        {
            return isHotVoxelInternal != 0;
        }
        set
        {
            isHotVoxelInternal = (byte)(value ? 1 : 0);
        }
    }

    public bool isGreyScale
    {
        get
        {
            return isGreyScaleInternal != 0;
        }
        set
        {
            isGreyScaleInternal = (byte)(value ? 1 : 0);
        }
    }


    public Voxel(bool isSolid, bool isGreyScale, int colourRGBValue)
    {
        this.isSolidInternal = (byte)(isSolid ? 1 : 0);
        this.isGreyScaleInternal = (byte)(isGreyScale ? 1 : 0);
        this.colourRGBValue[0] = colourRGBValue;
        this.isHotVoxelInternal = (byte)0;
        this.hotVoxelColourRBGValue = 0;

        // fixed (int* colourPtr = this.colourRGBValue)
        // {
        //     for (int i = 0; i < 10; i++)
        //     {
        //         colourPtr[i] = 0;
        //     }
        //     colourPtr[0] = colourRGBValue;
        // }
    }

    public Voxel(bool isSolid, bool isGreyScale, int layerNumber, int colourRGBValue)
    {
        isSolidInternal = (byte)(isSolid ? 1 : 0);
        this.isGreyScaleInternal = (byte)(isGreyScale ? 1 : 0);
        this.colourRGBValue[layerNumber] = colourRGBValue;
        this.isHotVoxelInternal = (byte)0;
        this.hotVoxelColourRBGValue = 0;
    }

    public void addHotVoxelColourRGB(int colourRGBValue)
    {
        this.hotVoxelColourRBGValue = colourRGBValue;
    }

    public int getHotVoxelColourRGB()
    {
        return this.hotVoxelColourRBGValue;
    }

    public void addColourRGBLayer(int layerNumber, int colourRGBValue)
    {
        this.colourRGBValue[layerNumber] = colourRGBValue;
    }

    public int getColourRGBLayer(int layerNumber)
    {
        if (layerNumber < 0 || layerNumber >= 10)
        {
            throw new System.IndexOutOfRangeException("Layer number out of range");
        }
        return this.colourRGBValue[layerNumber];
    }
}