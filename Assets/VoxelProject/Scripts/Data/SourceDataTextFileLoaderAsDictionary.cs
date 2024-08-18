using System.Collections.Generic;
using System.IO;
using Nifti.NET;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class SourceDataTextFileLoaderAsDictionary
{
    public int chunkSize = 0;

    public SourceDataTextFileLoaderAsDictionary(int chunkSize)
    {
        this.chunkSize = chunkSize;
    }

    string[] voxelFileLines = null;

    Voxel[] voxelData = null;
    public int widthX = 0;
    public int heightY = 0;
    public int depthZ = 0;

    public Dictionary<Vector3Int, Chunk> LoadSourceData()
    {
        Debug.Log("Loading source data...");
        //use streaming assets for the file path
        return LoadVoxelFile("Assets/Resources/blue.txt");
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
        //Debug.Log("Data now read in, data list size: "+voxelDataList.Count);

        // Get the dimensions
        widthX = maxX - minX; //voxelFileLines.Dimensions[0];
        heightY = maxY - minY; //voxelFileLines.Dimensions[1];
        depthZ = maxZ - minZ; //voxelFileLines.Dimensions[2];

        Debug.Log("DICTIONARY width:" + widthX);
        Debug.Log("DICTIONARY height:" + heightY);
        Debug.Log("DICTIONARY depth:" + depthZ);

        return ConstructChunks(voxelDataList);

      }
  
    private Dictionary<Vector3Int, Chunk> ConstructChunks(Dictionary<Vector3Int, Voxel> sourceData)
    {
        // USEFUL BUT SLOWS THINGS DOWN:Debug.Log("INNER Data now read in, data list size: " + sourceData.Count);
        // USEFUL BUT SLOWS THINGS DOWN:Debug.Log("Creating chunks of size ["+chunkSize+"] cubed.");

        // Assuming chunks is smaller than the number of voxels.
        Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>(sourceData.Count/4, new FastVector3IntComparer());

        int voxelsProcessed = 0;
        foreach (var nextVoxelElement in sourceData)
        {
            Vector3Int chunkCoordinates = Chunk.GetChunkCoordinates(nextVoxelElement.Key, chunkSize);

            // Create new chunk if it doesn't exist
            if (!chunks.ContainsKey(chunkCoordinates))
            {
                // USEFUL BUT SLOWS THINGS DOWN: Debug.Log("Creating new Chunk at position: "+chunkPosition);
                chunks[chunkCoordinates] = new Chunk(chunkCoordinates, chunkSize);
            }

            // Add voxel to the corresponding chunk
            chunks[chunkCoordinates].AddVoxel(nextVoxelElement.Key, nextVoxelElement.Value);
            voxelsProcessed++;
        }
        // USEFUL BUT SLOWS THINGS DOWN:Debug.Log("Voxels processed:" + voxelsProcessed);
        // USEFUL BUT SLOWS THINGS DOWN:Debug.Log("Number of chunks created: "+chunks.Count);
        return chunks;
    }


    public string[] ReadVoxelTextFile(string voxelTextFilePath)
    {
        // Load the text file
        return File.ReadAllLines(voxelTextFilePath);
    }

    private int maxX = 0;
    private int maxY = 0;
    private int maxZ = 0;
    private int minX = 0;
    private int minY = 0;
    private int minZ = 0;

    private Dictionary<Vector3Int, Voxel> ReadVoxelData(string[] lines)
    {
        //int numVoxels = width * height * depth;

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
               
            voxelDataList.Add(new Vector3Int(x, y, z), new Voxel(parts[3])); // Assign the parsed color color as the voxel color
        }
        Debug.Log("Lines processed:" + index);
        return voxelDataList;
    }
}