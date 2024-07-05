using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public unsafe struct Voxel
{
    // This struct has to be "blittable"!
    private byte isSolidInternal; // Use byte instead of bool
    private fixed int colourRGBValue[10]; // Fixed-size array, adjust size as needed


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
        this.colourRGBValue[0] = colourRGBValue;        
        // fixed (int* colourPtr = this.colourRGBValue)
        // {
        //     for (int i = 0; i < 10; i++)
        //     {
        //         colourPtr[i] = 0;
        //     }
        //     colourPtr[0] = colourRGBValue;
        // }
    }

    public Voxel(bool isSolid, int layerNumber, int colourRGBValue)
    {
        isSolidInternal = (byte)(isSolid ? 1 : 0);
        this.colourRGBValue[layerNumber] = colourRGBValue;
        // fixed (int* colourPtr = this.colourRGBValue)
        // {
        //     for (int i = 0; i < 10; i++)
        //     {
        //         colourPtr[i] = 0;
        //     }
        //     if (numberOfLayers > 0 && numberOfLayers <= 10)
        //     {
        //         colourPtr[0] = colourRGBValue;
        //     }
        // }
    }

    public void addColourRGBLayer(int layerNumber, int colourRGBValue)
    {
        if (layerNumber < 0 || layerNumber >= 10)
        {
            throw new System.IndexOutOfRangeException("Layer number out of range");
        }
        this.colourRGBValue[layerNumber] = colourRGBValue;
        // fixed (int* colourPtr = this.colourRGBValue)
        // {
        //     colourPtr[layerNumber] = colourRGBValue;
        // }
    }

    public int getColourRGBLayer(int layerNumber)
    {
        if (layerNumber < 0 || layerNumber >= 10)
        {
            throw new System.IndexOutOfRangeException("Layer number out of range");
        }
        return this.colourRGBValue[layerNumber];
        // fixed (int* colourPtr = this.colourRGBValue)
        // {
        //     return colourPtr[layerNumber];
        // }
    }
}