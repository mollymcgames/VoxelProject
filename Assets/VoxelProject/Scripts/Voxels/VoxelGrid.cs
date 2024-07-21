using System;
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
        Vector3Int chunkWorldPosition = GetChunkPosition(voxel.worldPosition);

        if (!chunks.ContainsKey(chunkWorldPosition))
        {
            chunks[chunkWorldPosition] = new VoxelChunk(chunkWorldPosition, chunkSize);
        }
        chunks[chunkWorldPosition].voxels.Add(voxel);

        // Flag this chunk as ACTIVE if even just one voxel is active.
        if (chunks[chunkWorldPosition].hasAtLeastOneActiveVoxel != true && voxel.colourGreyScaleValue > 0)
            chunks[chunkWorldPosition].hasAtLeastOneActiveVoxel = true;

        voxelsRepresented++;
    }

    public Voxel GetVoxel(Vector3Int position)
    {
        Vector3Int chunkPos = GetChunkPosition(position);
        if (chunks.ContainsKey(chunkPos))
        {
            // Debug.Log("Got voxel from chunk:" + chunkPos);
            Vector3Int localPos = GetLocalPositionInChunk(position);
            // Debug.Log("Voxel local pos in chunk:" + localPos);
            // Debug.Log("Voxels in chunk:" + chunks[chunkPos].voxels.Count);
            // Debug.Log("Chunk is active: " + chunks[chunkPos].hasAtLeastOneActiveVoxel);
            return chunks[chunkPos].GetVoxelUsingWorldPosition(localPos);
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