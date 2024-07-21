using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid
{
    public Dictionary<Vector3Int, VoxelChunk> Chunks { get; private set; }
    private const int chunkSize = 16;

    public VoxelGrid()
    {
        Chunks = new Dictionary<Vector3Int, VoxelChunk>();
    }

    public void AddVoxel(Voxel voxel)
    {
        Vector3Int chunkPos = GetChunkPosition(voxel.position);
        if (!Chunks.ContainsKey(chunkPos))
        {
            Chunks[chunkPos] = new VoxelChunk(chunkPos, chunkSize);
        }
        Chunks[chunkPos].voxels.Add(voxel);
    }

    public Voxel GetVoxel(Vector3Int position)
    {
        Vector3Int chunkPos = GetChunkPosition(position);
        if (Chunks.ContainsKey(chunkPos))
        {
            Vector3Int localPos = GetLocalPositionInChunk(position);
            return Chunks[chunkPos].GetVoxel(localPos);
        }
        return null;
    }

    public static Vector3Int GetChunkPosition(Vector3Int voxelPosition)
    {
        return new Vector3Int(
            Mathf.FloorToInt((float)voxelPosition.x / chunkSize) * chunkSize,
            Mathf.FloorToInt((float)voxelPosition.y / chunkSize) * chunkSize,
            Mathf.FloorToInt((float)voxelPosition.z / chunkSize) * chunkSize
        );

/*        return new Vector3Int(
            Mathf.FloorToInt((float)voxelPosition.x / chunkSize),
            Mathf.FloorToInt((float)voxelPosition.y / chunkSize),
            Mathf.FloorToInt((float)voxelPosition.z / chunkSize)
        );
*/    }

    public static Vector3Int GetLocalPositionInChunk(Vector3Int voxelPosition)
    {
        return new Vector3Int(
            voxelPosition.x % chunkSize,
            voxelPosition.y % chunkSize,
            voxelPosition.z % chunkSize
        );
    }
}