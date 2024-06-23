using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
/**
 * This struct represents a single Voxel and its properties
 */
public class VoxelElement
{
    public int ID;
    //public Vector3Int position { get; }
    public int positionX;
    public int positionY;
    public int positionZ;
    public Color color = Color.white;
    public string colorString = "";
    public bool isActive = true;

    public VoxelElement(Vector3Int position, Color color, bool isActive = true)
    {
        this.positionX = position.x;
        this.positionY = position.y;
        this.positionZ = position.z;
        this.color = color;
        this.isActive = isActive;
        // @TODO Need to do something with this ID...
        this.ID = 1;
    }

    public VoxelElement(Vector3Int position)
    {
        this.positionX = position.x;
        this.positionY = position.y;
        this.positionZ = position.z;
    }

    public VoxelElement(Vector3Int position, string colorString)
    {
        this.positionX = position.x;
        this.positionY = position.y;
        this.positionZ = position.z;
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

    public Vector3Int position {
        get
        {            
            return new Vector3Int(positionX,positionY,positionZ);
        }  
    }

}