using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
/**
 * This struct represents a single Voxel and its properties
 */
public class VoxelElementSmaller
{
    public int ID;
    public float colorR = 0f;
    public float colorG = 0f;
    public float colorB = 0f;
    public string colorString = "";
    public bool isActive = true;

    public VoxelElementSmaller(Color color, bool isActive = true)
    {
        colorR = color.r;
        colorG = color.g;
        colorB = color.b;
        this.isActive = isActive;
        // @TODO Need to do something with this ID...
        this.ID = 1;
    }

    public VoxelElementSmaller(string colorString)
    {
        this.colorString = colorString;
        // @TODO Need to do something with this ID...
        this.ID = 1;
    }

    public bool isSolid
    {
        get
        {
            //return ID != 0;
            return true;
        }
    }
}