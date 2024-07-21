using System.Collections.Generic;
using UnityEngine;

/**
 * This class represents a single Chunk.
 * A Chunk is a group of Voxels that can POTENTIALLY be rendered.
 * Typically a Chunk will form a group of related (by proximity) chunks that can POTENTIALLY be rendered.
 */
public class VoxelChunk
{
    public Vector3Int chunkCoordinates { get; set; }

    public List<Voxel> voxels { get; set; }

    public bool hasAtLeastOneActiveVoxel = false;

    //Each chunk needs to have boundaries to help determine if it is within the camera's view port.
    public Bounds chunkBounds { get; set; }

    public VoxelChunk(Vector3Int chunkCoordinates)
    {
        this.chunkCoordinates = chunkCoordinates;
        this.voxels = new List<Voxel>();
    }

    public VoxelChunk(Vector3Int chunkCoordinates, int chunkSize)
    {
        this.chunkCoordinates = chunkCoordinates;
        this.voxels = new List<Voxel>();
        Vector3Int centre = new Vector3Int(chunkCoordinates.x * chunkSize + chunkSize / 2, chunkCoordinates.y * chunkSize + chunkSize / 2, chunkCoordinates.z * chunkSize + chunkSize / 2);
        this.chunkBounds = new Bounds(centre, new Vector3Int(chunkSize, chunkSize, chunkSize));
    }

    public void AddVoxel(int x, int y, int z)
    {
        AddVoxel(new Voxel(new Vector3Int(x, y, z)));
    }

    public void AddVoxel(Voxel voxel)
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

    public Voxel GetVoxel(Vector3Int localPosition)
    {
        return voxels.Find(v => v.position == localPosition);
    }
}