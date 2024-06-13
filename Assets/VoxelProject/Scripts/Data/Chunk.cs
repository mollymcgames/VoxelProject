using System.Collections.Generic;
using UnityEngine;

/**
 * This class represents a single Chunk.
 * A Chunk is a group of Voxels that can POTENTIALLY be rendered.
 * Typically a Chunk will form a group of related (by proximity) Chunks that can POTENTIALLY be rendered.
 */
public class Chunk
{
    public Vector3Int chunkCoordinates { get; private set; }
    public List<VoxelElement> voxels { get; private set; }

    private int chunkSize = 8;

    public Chunk(Vector3Int chunkCoordinates)
    {
        this.chunkCoordinates = chunkCoordinates;
        this.voxels = new List<VoxelElement>();
    }

    public void AddVoxel(int x, int y, int z)
    {
        AddVoxel(new VoxelElement(new Vector3Int(x, y, z)));
    }

    public void AddVoxel(VoxelElement voxel)
    {
        voxels.Add(voxel);
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