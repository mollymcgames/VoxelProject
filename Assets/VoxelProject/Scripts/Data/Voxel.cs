using System;
using Unity.Collections;

/**
* A VoxelCell represents information about a particular type of cell.
*/
public struct Voxel
{
    public readonly FixedString32Bytes colorR;
    public readonly FixedString32Bytes colorG;
    public readonly FixedString32Bytes colorB;
    public readonly int color;

    public readonly bool isSegmentVoxel;

    public Voxel(string colorR, string colorG, string colorB)
    {
        this.colorR = colorR;
        this.colorG = colorG;
        this.colorB = colorB;
        this.color = Int32.Parse(colorR);
        isSegmentVoxel = false;
    }

    public Voxel(string colorR, string colorG, string colorB, bool isSegmentVoxel)
    {
        this.colorR = colorR;
        this.colorG = colorG;
        this.colorB = colorB;
        this.color = Int32.Parse(colorR);
        this.isSegmentVoxel = isSegmentVoxel;
    }
}