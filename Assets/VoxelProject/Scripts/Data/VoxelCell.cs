/**
* A VoxelCell represents information about a particular type of cell. Could be colour, if it makes a beeping noise, anything really!
*/
public class VoxelCell
{
    public float value { get; }

    public int depthZ { get; }

    public int heightY { get; }

    public int widthX { get; }

    public VoxelCell(int depthZ, int heightY, int widthX, float inputValue)
    {
        this.depthZ = depthZ;
        this.heightY = heightY;
        this.widthX = widthX;
        this.value = inputValue;
    }
}