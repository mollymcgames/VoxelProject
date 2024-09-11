using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//This class is used to load the source data from a text file and store it in a dictionary.
//Used by zooming scene to load the source data from a text file.
public class SourceDataTextFileLoaderAsDictionary
{
    public int chunkSize = 0;

    private int maxX = 0;
    private int maxY = 0;
    private int maxZ = 0;
    private int minX = 0;
    private int minY = 0;
    private int minZ = 0;

    string[] voxelFileLines = null;

    public int X = 0;
    public int Y = 0;
    public int Z = 0;

    public SourceDataTextFileLoaderAsDictionary(int chunkSize)
    {
        this.chunkSize = chunkSize;
    }

    public Dictionary<Vector3Int, Chunk> LoadSourceData(string filepath)
    {
        Debug.Log("Loading source data as Dictionary...");
        return LoadVoxelFile(filepath);
    }    

    public string[] GetHeader()
    {
        return voxelFileLines;
    }

    public Dictionary<Vector3Int, Chunk> LoadVoxelFile(string voxelFilePath = "Assets/Resources/z.txt")
    {
        // Load default file
        voxelFileLines = ReadVoxelTextFile(voxelFilePath);

        // Read the voxel data
        Dictionary<Vector3Int, Voxel> voxelDataList = ReadVoxelData(voxelFileLines);        

        // Get the dimensions
        X = maxX - minX; //voxelFileLines.Dimensions[0];
        Y = maxY - minY; //voxelFileLines.Dimensions[1];
        Z = maxZ - minZ; //voxelFileLines.Dimensions[2];

        Debug.Log("DICTIONARY width:" + X);
        Debug.Log("DICTIONARY height:" + Y);
        Debug.Log("DICTIONARY depth:" + Z);

        return ConstructChunks(voxelDataList);
    }
  
    private Dictionary<Vector3Int, Chunk> ConstructChunks(Dictionary<Vector3Int, Voxel> sourceData)
    {
        // Assuming chunks is smaller than the number of voxels.
        Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>(sourceData.Count/4, new FastVector3IntComparer());

        int voxelsProcessed = 0;
        foreach (var nextVoxelElement in sourceData)
        {
            Vector3Int chunkCoordinates = Chunk.GetChunkCoordinates(nextVoxelElement.Key, chunkSize);

            // Create new chunk if it doesn't exist
            if (!chunks.ContainsKey(chunkCoordinates))
            {
                chunks[chunkCoordinates] = new Chunk(chunkCoordinates, chunkSize);
            }

            // Add voxel to the corresponding chunk
            chunks[chunkCoordinates].AddVoxel(nextVoxelElement.Key, nextVoxelElement.Value);
            voxelsProcessed++;
        }
        return chunks;
    }

    public string[] ReadVoxelTextFile(string voxelTextFilePath)
    {
        // Load the text file
        return File.ReadAllLines(voxelTextFilePath);
    }

    private Dictionary<Vector3Int, Voxel> ReadVoxelData(string[] lines)
    {
        Dictionary<Vector3Int, Voxel> voxelDataList = new Dictionary<Vector3Int, Voxel>(lines.Length, new FastVector3IntComparer());

        Debug.Log("Lines detected in file:" + lines.Length);
        int index = 0;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue; // Skip empty lines and comments

            var parts = line.Split(' ');
            if (parts.Length != 4)
            {
                Debug.Log($"Invalid line format: {line}");
                continue;
            }
            if (!int.TryParse(parts[0], out int x) ||
                !int.TryParse(parts[1], out int y) ||
                !int.TryParse(parts[2], out int z))
            {
                Debug.Log($"Invalid integer values in line: {line}");
                continue;
            }

            if (x>maxX) maxX = x;
            if (x<minX) minX = x;
            if (y>maxY) maxY = y;
            if (y<minY) minY = y;
            if (z>maxZ) maxZ = z;
            if (z<minZ) minZ = z;

            Color color;
            if (ColorUtility.TryParseHtmlString("#" + parts[3], out color))
            {             
                voxelDataList.Add(new Vector3Int(x, y, z), new Voxel((int)color.r));
            }
        }
        Debug.Log("Lines processed:" + index);
        return voxelDataList;
    }
}