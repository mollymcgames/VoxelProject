using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public abstract class ASourceDataLoader : ISourceDataLoader
{
    public Voxel[,,] voxelData = null;

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

    //public abstract Dictionary<Vector3Int, Chunk> LoadSourceData(string filepath);
    public abstract Voxel[,,] LoadSourceData(string filepath);


    public abstract Voxel[,,] LoadSegmentData(ref Voxel[,,] sourceData, int segmentLayer, string nextSegmentFile);

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
            // MEMORY SAVER: If we know the coordinates, which we're using as a chunk index, why do we
            // need to save the coordinates twice (i.e. in the voxel object)?
            chunks[chunkCoordinates].AddVoxel(nextVoxelElement);
            voxelsProcessed++;
        }
        Debug.Log("Voxels processed:" + voxelsProcessed);
        Debug.Log("Number of chunks created: " + chunks.Count);
        return chunks;
    }

    // A HotVoxelFile is a CSV file with x y z coordinates and a colour in HTML format for each hot voxel
    // e.g.
    // 10,10,20,#FF0000
    // 10,20,20,#FF00FF
    public Voxel[,,] LoadHotVoxelFile(string hotVoxelsFilePath)
    {
        Voxel nextHotVoxel;

        // Load the text file
        string[] hotVoxels = File.ReadAllLines(hotVoxelsFilePath);

        int index = 0;
        foreach (var hotVoxelLine in hotVoxels)
        {
            if (string.IsNullOrWhiteSpace(hotVoxelLine) || hotVoxelLine.StartsWith("#")) continue; // Skip empty lines and comments

            var parts = hotVoxelLine.Split(',');
            if (parts.Length != 4)
            {
                Debug.LogError($"Invalid line format: {hotVoxelLine}");
                continue;
            }
            if (!int.TryParse(parts[0], out int x) ||
                !int.TryParse(parts[1], out int y) ||
                !int.TryParse(parts[2], out int z))
            {
                Debug.LogError($"Invalid integer values in line: {hotVoxelLine}");
                continue;
            }

            nextHotVoxel = voxelData[x, y, z]; // = new VoxelCell(z, y, x, parts[3]); // Assign the parsed color value as the voxel value
            nextHotVoxel.isHotVoxel = true;
            nextHotVoxel.addHotVoxelColourRGB(Convert.ToInt32(parts[3].Replace("#", ""), 16));
            voxelData[x, y, z] = nextHotVoxel;
        }

        return voxelData;
    }
}