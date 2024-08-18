using System.Collections.Generic;
using UnityEngine;

/**
 * This class represents a single Chunk.
 * A Chunk is a group of Voxels that can POTENTIALLY be rendered.
 * Typically a Chunk will form a group of related (by proximity) Chunks that can POTENTIALLY be rendered.
 */
public class Chunk
{
    public Bounds bounds;

    public Vector3Int chunkPosition { get; private set; }
    public Dictionary<Vector3Int, Voxel> voxels { get; private set; }

    // Creates a new Chunk, including it's Unity "Bounds" which makes doing Frustrum visibility calculations quicker later on.
    public Chunk(Vector3Int chunkCoordinates, int chunkSize)
    {
        this.chunkPosition = chunkCoordinates;
        this.voxels = new Dictionary<Vector3Int, Voxel>(chunkSize* chunkSize* chunkSize, new FastVector3IntComparer());
        bounds = new Bounds(chunkCoordinates, new Vector3(chunkSize, chunkSize, chunkSize));
    }

/*    public void AddVoxel(int x, int y, int z)
    {
        AddVoxel(new VoxelElement(new Vector3Int(x, y, z)));
    }*/

    public void AddVoxel(Vector3Int position, Voxel voxel)
    {
        voxels.Add(position, voxel);
    }
    public static Vector3Int GetChunkCoordinates(Vector3 voxelPosition, int chunkSize)
    {
        // Calculate chunk coordinates
        int chunkX = Mathf.FloorToInt((float)voxelPosition.x / chunkSize)*chunkSize;
        int chunkY = Mathf.FloorToInt((float)voxelPosition.y / chunkSize)*chunkSize;
        int chunkZ = Mathf.FloorToInt((float)voxelPosition.z / chunkSize)*chunkSize;

        return new Vector3Int(chunkX, chunkY, chunkZ);
    }
}