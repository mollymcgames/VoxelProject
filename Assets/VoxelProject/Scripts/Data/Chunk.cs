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

/*    void OnDrawGizmos()
    {
        if (voxels != null)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        if (voxels[x, y, z].isActive)
                        {
                            Gizmos.color = voxels[x, y, z].color;
                            Gizmos.DrawCube(transform.position + new Vector3(x, y, z), Vector3.one);
                        }
                    }
                }
            }
        }
    }*/
/*
    private void InitializeVoxels()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    voxels[x, y, z] = new Voxel(transform.position + new Vector3(x, y, z), Color.white);
                }
            }
        }
    }*/
}