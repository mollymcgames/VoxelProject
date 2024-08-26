using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public abstract class ASourceDataLoader : ISourceDataLoader
{
    public Dictionary<Vector3Int, Voxel> voxelDictionary = null;

    public int maxX = 0;
    public int maxY = 0;
    public int maxZ = 0;
    public int minX = 0;
    public int minY = 0;
    public int minZ = 0;

    public int X = 0;
    public int Y = 0;
    public int Z = 0;

    public int voxelOmissionThreshold = 0;

    public abstract Dictionary<Vector3Int, Voxel> LoadSourceData(string filepath);

    public Dictionary<Vector3Int, Voxel> LoadVoxelSegmentDefinitionFile(int segmentLayer, string voxelSegmentDefinitionFilePath)
    {
        Voxel nextSegmentVoxel;

        // Load the text file
        string[] segmentVoxels = File.ReadAllLines(voxelSegmentDefinitionFilePath);

        int index = 0;
        foreach (var segmentVoxelLine in segmentVoxels)
        {
            // Skip empty lines and comments
            if (string.IsNullOrWhiteSpace(segmentVoxelLine) || segmentVoxelLine.StartsWith("#"))
            {
                continue;
            }

            var parts = segmentVoxelLine.Split(',');
            if (parts.Length != 4)
            {
                Debug.LogError($"Invalid line format: {segmentVoxelLine}");
                continue;
            }
            if (!int.TryParse(parts[0], out int x) ||
                !int.TryParse(parts[1], out int y) ||
                !int.TryParse(parts[2], out int z))
            {
                Debug.LogError($"Invalid integer values in line: {segmentVoxelLine}");
                continue;
            }

            voxelDictionary.TryGetValue(new Vector3Int(x, y, z), out nextSegmentVoxel);
            voxelDictionary[new Vector3Int(x, y, z)] = new Voxel(Int32.Parse(parts[3].Replace("#", "")), true);// x, y, z, parts[3].Replace("#", ""), true);
        }

        return voxelDictionary;
    }

    public abstract object GetHeader();

    // A HotVoxelFile is a CSV file with x y z coordinates and a colour in HTML format for each hot voxel
    // e.g.
    // 10,10,20,#FF0000
    // 10,20,20,#FF00FF
    public Dictionary<Vector3Int, Voxel> LoadVoxelSegmentDefinitionFileExtra(string voxelSegmentDefinitionFilePath)
    {
        Voxel nextSegmentVoxel;

        // Load the text file
        string[] segmentVoxels = File.ReadAllLines(voxelSegmentDefinitionFilePath);

        int index = 0;
        foreach (var segmentVoxelLine in segmentVoxels)
        {
            if (string.IsNullOrWhiteSpace(segmentVoxelLine) || segmentVoxelLine.StartsWith("#")) continue; // Skip empty lines and comments

            var parts = segmentVoxelLine.Split(',');
            if (parts.Length != 4)
            {
                Debug.LogError($"Invalid line format: {segmentVoxelLine}");
                continue;
            }

            if (!int.TryParse(parts[0], out int x) ||
                !int.TryParse(parts[1], out int y) ||
                !int.TryParse(parts[2], out int z))
            {
                Debug.LogError($"Invalid integer values in line: {segmentVoxelLine}");
                continue;
            }

            voxelDictionary.TryGetValue(new Vector3Int(x, y, z), out nextSegmentVoxel);
            voxelDictionary[new Vector3Int(x, y, z)] = new Voxel(Int32.Parse(parts[3].Replace("#", "")) , true);            
        }

        WorldManager.Instance.voxelChunks = ConstructChunks(voxelDictionary);

        return voxelDictionary;
    }

    public Vector3Int CalculateCenter(int x, int y, int z)
    {
        int centerX = (int)Math.Round(x / 2.0);
        int centerY = (int)Math.Round(y / 2.0);
        int centerZ = (int)Math.Round(z / 2.0);

        return new Vector3Int(centerX, centerY, centerZ);
    }

    public Dictionary<Vector3Int, Chunk> ConstructChunks(Dictionary<Vector3Int, Voxel> voxelDictionary)
    {
        Vector3Int chunkCoordinates;
        Debug.Log("Data now read in, data list size: " + voxelDictionary.Count);
        Debug.Log("Creating chunks of size [" + WorldManager.Instance.voxelMeshConfigurationSettings.voxelChunkSize + "] cubed.");

        // Assuming the number of Chunks is going to be smaller than the straight voxels!
        Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>(voxelDictionary.Count/4);

        int voxelsProcessed = 0;
        foreach (var nextVoxelElement in voxelDictionary)
        {
            chunkCoordinates = Chunk.GetChunkCoordinates(nextVoxelElement.Key, WorldManager.Instance.voxelMeshConfigurationSettings.voxelChunkSize);

            // Create new chunk if it doesn't exist
            if (!chunks.ContainsKey(chunkCoordinates))
            {
                chunks[chunkCoordinates] = new Chunk(chunkCoordinates, WorldManager.Instance.voxelMeshConfigurationSettings.voxelChunkSize);
            }

            // Add voxel to the corresponding chunk
            chunks[chunkCoordinates].AddVoxel(nextVoxelElement.Key, nextVoxelElement.Value);
            voxelsProcessed++;
        }
        Debug.Log("Voxels processed:" + voxelsProcessed);
        Debug.Log("Number of chunks created: " + chunks.Count);
        return chunks;
    }

}