using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public abstract class ASourceDataLoader : ISourceDataLoader
{
    // FIXP public VoxelCell[,,] voxelDictionary = null;
    // DICT AS LONG public Dictionary<Vector3Int, VoxelCell> voxelDictionary = null;
    public Dictionary<long, VoxelCell> voxelDictionary = null;
    //public VoxelGrid voxelGrid = null;

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

    // FIXP public abstract VoxelCell[,,] LoadSourceData(string filepath);
    public abstract Dictionary<long, VoxelCell> LoadSourceData(string filepath);

    //public abstract VoxelGrid LoadSourceDataGrid(string filepath);

    // FIXP public VoxelCell[,,] LoadVoxelSegmentDefinitionFile(int segmentLayer, string voxelSegmentDefinitionFilePath)
    public Dictionary<long, VoxelCell> LoadVoxelSegmentDefinitionFile(int segmentLayer, string voxelSegmentDefinitionFilePath)
    {
        VoxelCell nextSegmentVoxel;

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

            // FIXP nextSegmentVoxel = voxelDictionary[x, y, z]; // = new VoxelCell(z, y, x, parts[3]); // Assign the parsed color color as the voxel color
            voxelDictionary.TryGetValue(Vector3IntConvertor.EncodeVector3Int(new Vector3Int(x, y, z)), out nextSegmentVoxel);
            voxelDictionary[Vector3IntConvertor.EncodeVector3Int(new Vector3Int(x, y, z))] = new VoxelCell(x, y, z, parts[3].Replace("#", ""), true);
            // FIXP voxelDictionary[x, y, z] = new VoxelCell(x, y, z, parts[3].Replace("#", ""), true);
            // TODO nextSegmentVoxel.isHotVoxel = true;
            // TODO nextSegmentVoxel.addHotVoxelColourRGB(Convert.ToInt32(parts[3].Replace("#", ""), 16));
            // TODO voxelDictionary[x, y, z] = nextSegmentVoxel;
        }

        return voxelDictionary;
    }

    public abstract object GetHeader();

    // A HotVoxelFile is a CSV file with x y z coordinates and a colour in HTML format for each hot voxel
    // e.g.
    // 10,10,20,#FF0000
    // 10,20,20,#FF00FF
    // public VoxelCell[,,] LoadVoxelSegmentDefinitionFileExtra(string voxelSegmentDefinitionFilePath)
    public Dictionary<long, VoxelCell> LoadVoxelSegmentDefinitionFileExtra(string voxelSegmentDefinitionFilePath)
    {
        VoxelCell nextSegmentVoxel;

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

            // FIXP nextSegmentVoxel = voxelDictionary[x, y, z]; // = new VoxelCell(z, y, x, parts[3]); // Assign the parsed color color as the voxel color
            voxelDictionary.TryGetValue(Vector3IntConvertor.EncodeVector3Int(new Vector3Int(x, y, z)), out nextSegmentVoxel);
            voxelDictionary[Vector3IntConvertor.EncodeVector3Int(new Vector3Int(x, y, z))] = new VoxelCell(x, y, z, parts[3].Replace("#", ""), true);
            // TODO nextSegmentVoxel.isHotVoxel = true;
            // TODO nextSegmentVoxel.addHotVoxelColourRGB(Convert.ToInt32(parts[3].Replace("#", ""), 16));
            // TODO voxelDictionary[x, y, z] = nextSegmentVoxel;
        }

        return voxelDictionary;
    }

    public Vector3Int CalculateCenter(int x, int y, int z)
    {
        int centerX = (int)Math.Round(x / 2.0);
        int centerY = (int)Math.Round(y / 2.0);
        int centerZ = (int)Math.Round(z / 2.0);

        return new Vector3Int(centerX, centerY, centerZ);
    }

    /*public Dictionary<Vector3Int, VoxelChunk> ConstructChunks(List<Voxel> voxelDictionary)
    {
        Debug.Log("Data now read in, data list size: " + voxelDictionary.Count);
        Debug.Log("Creating chunks of size [" + chunkSize + "] cubed.");

        Dictionary<Vector3Int, VoxelChunk> chunks = new Dictionary<Vector3Int, VoxelChunk>();

        int voxelsProcessed = 0;
        foreach (Voxel nextVoxelElement in voxelDictionary)
        {
            Vector3Int chunkCoordinates = VoxelChunk.GetChunkCoordinates(nextVoxelElement.worldPosition, chunkSize);

            // Create new chunk if it doesn't exist
            if (!chunks.ContainsKey(chunkCoordinates))
            {
                Debug.Log("Creating new Chunk at worldPosition: " + chunkCoordinates);
                chunks[chunkCoordinates] = new VoxelChunk(chunkCoordinates);
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
    }*/

}