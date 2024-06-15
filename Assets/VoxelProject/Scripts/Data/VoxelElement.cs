using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
/**
 * This struct represents a single Voxel and its properties
 */
public class VoxelElement
{
    public int ID;
    public Vector3Int position { get; private set; }
    public Color color = Color.white;
    public string colorString = "";
    public bool isActive = true;

    public VoxelElement(Vector3Int position, Color color, bool isActive = true)
    {
        this.position = position;
        this.color = color;
        this.isActive = isActive;
        // @TODO Need to do something with this ID...
        this.ID = 1;
    }

    public VoxelElement(Vector3Int position)
    {
        this.position = position;
    }

    public VoxelElement(Vector3Int position, string colorString)
    {
        this.position = position;
        this.colorString = colorString;
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