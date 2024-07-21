using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
/**
 * This struct represents a single Voxel and its properties
 */
public class Voxel
{
    public int ID;
    public Vector3Int worldPosition { get; set; }
    //public Color color = Color.white;
    public int colourGreyScaleValue = 0;
    public bool isActive = true;

    public Voxel(Vector3Int position, Color color, bool isActive = true)
    {
        this.worldPosition = position;
        //this.color = color;
        this.isActive = isActive;
        // @TODO Need to do something with this ID...
        this.ID = 1;
    }

    public Voxel(Vector3Int worldPosition)
    {
        this.worldPosition = worldPosition;
    }

    public Voxel(Vector3Int worldPosition, string colorString)
    {
        this.worldPosition = worldPosition;
        //this.colorString = colorString;
    }

    public Voxel() { }

    public bool isSolid
    {
        get
        {
            //return ID != 0;
            return true;
        }
    }

    public int getColourRGBLayer(int layerNumber)
    {
        if (layerNumber < 0 || layerNumber >= 10)
        {
            throw new System.IndexOutOfRangeException("Layer number out of range");
        }
        return colourGreyScaleValue;
        //return this.colourGreyScaleValue[layerNumber];
    }

    public int getHotVoxelColourRGB()
    {
        // KJP TODO WE NEED TO ADD PROPER COLOURS
        return 200;
        //return this.colourGreyScaleValue[layerNumber];
    }

    public bool isHotVoxel()
    {
            return false;
    }

    public bool isGreyScale()
    {
        return true;
    }

}