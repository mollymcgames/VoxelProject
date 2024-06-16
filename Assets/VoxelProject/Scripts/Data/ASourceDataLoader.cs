using System.Collections.Generic;
using UnityEngine;

public abstract class ASourceDataLoader : ISourceDataLoader
{
    public int chunkSize = 0;

    public int maxX = 0;
    public int maxY = 0;
    public int maxZ = 0;
    public int minX = 0;
    public int minY = 0;
    public int minZ = 0;

    public int widthX = 0;
    public int heightY = 0;
    public int depthZ = 0;

    public ASourceDataLoader(int chunkSize)
    {
        this.chunkSize = chunkSize;
    }

    public abstract Dictionary<Vector3Int, Chunk> LoadSourceData(string filepath);

    public abstract object GetHeader();

    public Dictionary<Vector3Int, Chunk> ConstructChunks(List<VoxelElement> sourceData)
    {
        Debug.Log("Data now read in, data list size: " + sourceData.Count);
        Debug.Log("Creating chunks of size [" + chunkSize + "] cubed.");

        Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

        int voxelsProcessed = 0;
        foreach (VoxelElement nextVoxelElement in sourceData)
        {
            Vector3Int chunkCoordinates = Chunk.GetChunkCoordinates(nextVoxelElement.position, chunkSize);

            // Create new chunk if it doesn't exist
            if (!chunks.ContainsKey(chunkCoordinates))
            {
                Debug.Log("Creating new Chunk at position: " + chunkCoordinates);
                chunks[chunkCoordinates] = new Chunk(chunkCoordinates);
            }

            // Add voxel to the corresponding chunk
            chunks[chunkCoordinates].AddVoxel(nextVoxelElement);
            voxelsProcessed++;
        }
        Debug.Log("Voxels processed:" + voxelsProcessed);
        Debug.Log("Number of chunks created: " + chunks.Count);
        return chunks;
    }

}