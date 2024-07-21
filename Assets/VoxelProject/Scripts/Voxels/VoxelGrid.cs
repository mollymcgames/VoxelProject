using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid
{
    public Dictionary<Vector3Int, VoxelChunk> chunks { get; private set; }
    
    public int chunkSize { get; private set; }

    public int voxelsRepresented { get; private set; }

    public VoxelGrid(int chunkSize) 
    {
        voxelsRepresented = 0;
        this.chunkSize = chunkSize;
        chunks = new Dictionary<Vector3Int, VoxelChunk>();
    }

    public void AddVoxel(Voxel voxel)
    {
        if (voxel.position.x > 59 || voxel.position.y > 65 || voxel.position.z > 36)
            Debug.Log("key voxel!");

            Vector3Int chunkPos = GetChunkPosition(voxel.position);
        if (!chunks.ContainsKey(chunkPos))
        {
            chunks[chunkPos] = new VoxelChunk(chunkPos, chunkSize);
        }
        chunks[chunkPos].voxels.Add(voxel);
        chunks[chunkPos].hasAtLeastOneActiveVoxel = voxel.isActive;
        voxelsRepresented++;
    }

    public Voxel GetVoxel(Vector3Int position)
    {
        Vector3Int chunkPos = GetChunkPosition(position);
        if (chunks.ContainsKey(chunkPos))
        {
            Vector3Int localPos = GetLocalPositionInChunk(position);
            return chunks[chunkPos].GetVoxel(localPos);
        }
        return null;
    }

    public Vector3Int GetChunkPosition(Vector3Int voxelPosition)
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

    public Vector3Int GetLocalPositionInChunk(Vector3Int voxelPosition)
    {
        return new Vector3Int(
            voxelPosition.x % chunkSize,
            voxelPosition.y % chunkSize,
            voxelPosition.z % chunkSize
        );
    }
}