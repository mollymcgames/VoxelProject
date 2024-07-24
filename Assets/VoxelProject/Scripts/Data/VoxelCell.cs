/**
* A VoxelCell represents information about a particular type of cell.
*/
public class VoxelCell
{
    public readonly string color;

    public readonly int z;

    public readonly int y;

    public readonly int x;

    public readonly bool isSegmentVoxel;

    public VoxelCell(int z, int y, int x, string color)
    {
        this.z = z;
        this.y = y;
        this.x = x;
        this.color = color;
        isSegmentVoxel = false;
    }

    public VoxelCell(int z, int y, int x, string color, bool isSegmentVoxel)
    {
        this.z = z;
        this.y = y;
        this.x = x;
        this.color = color;
        this.isSegmentVoxel = isSegmentVoxel;
    }

}