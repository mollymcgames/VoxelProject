
using Unity.Collections;

/**
* A VoxelCell represents information about a particular type of cell.
*/
public struct Voxel
{
    public readonly FixedString32Bytes colorR;
    public readonly FixedString32Bytes colorG;
    public readonly FixedString32Bytes colorB;

    //public readonly int z;

    //public readonly int y;

    //public readonly int x;

    public readonly bool isSegmentVoxel;

    //public bool isOuterVoxel;

    //public readonly Vector3Int Position;

    public Voxel(string colorR, string colorG, string colorB)//int z, int y, int x, string color) // , bool isOuterVoxel)
    {
        //this.z = z;
        //this.y = y;
        //this.x = x;
        this.colorR = colorR;
        this.colorG = colorG;
        this.colorB = colorB;
        isSegmentVoxel = false;
        //this.isOuterVoxel = false; // isOuterVoxel;
        //Position = new Vector3Int(x, y, z);
    }

    public Voxel(string colorR, string colorG, string colorB, bool isSegmentVoxel)//int z, int y, int x, string color, bool isSegmentVoxel) //, bool isOuterVoxel)
    {
        //this.z = z;
        //this.y = y;
        //this.x = x;
        this.colorR = colorR;
        this.colorG = colorG;
        this.colorB = colorB;
        this.isSegmentVoxel = isSegmentVoxel;
        //this.isOuterVoxel = false; // isOuterVoxel;
        //Position = new Vector3Int(x, y, z);
    }
}