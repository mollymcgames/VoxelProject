
using UnityEngine;

/**
* A VoxelCell represents information about a particular type of cell.
*/
public class Voxel
{
    public readonly string color;

    //public readonly int z;

    //public readonly int y;

    //public readonly int x;

    public readonly bool isSegmentVoxel;

    //public bool isOuterVoxel;

    //public readonly Vector3Int Position;

    public Voxel(string color)//int z, int y, int x, string color) // , bool isOuterVoxel)
    {
        //this.z = z;
        //this.y = y;
        //this.x = x;
        this.color = color;
        isSegmentVoxel = false;
        //this.isOuterVoxel = false; // isOuterVoxel;
        //Position = new Vector3Int(x, y, z);
    }

    public Voxel(string color, bool isSegmentVoxel)//int z, int y, int x, string color, bool isSegmentVoxel) //, bool isOuterVoxel)
    {
        //this.z = z;
        //this.y = y;
        //this.x = x;
        this.color = color;
        this.isSegmentVoxel = isSegmentVoxel;
        //this.isOuterVoxel = false; // isOuterVoxel;
        //Position = new Vector3Int(x, y, z);
    }
}