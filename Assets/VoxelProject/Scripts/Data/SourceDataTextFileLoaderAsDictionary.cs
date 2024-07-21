using System.Collections.Generic;
using System.IO;
using Nifti.NET;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class SourceDataTextFileLoaderAsDictionary
{
    private int chunkSize = 0;
    
    public int maxX = 0;
    public int maxY = 0;
    public int maxZ = 0;
    public int minX = 0;
    public int minY = 0;
    public int minZ = 0;

    public SourceDataTextFileLoaderAsDictionary(int chunkSize)
    {
        this.chunkSize = chunkSize;
    }

    string[] voxelFileLines = null;

    VoxelCell[] voxelData = null;

    public int widthX = 0;
    public int heightY = 0;
    public int depthZ = 0;

    public Dictionary<Vector3Int, VoxelChunk> LoadSourceData(string filepath)
    {
        Debug.Log("Loading source data as Dictionary...");
        return LoadVoxelFile(filepath);
    }    

    public string[] GetHeader()
    {
        return voxelFileLines;
    }

    public Dictionary<Vector3Int, VoxelChunk> LoadVoxelFile(string voxelFilePath = "Assets/Resources/z.txt")
    {
        // Load default file
        voxelFileLines = ReadVoxelTextFile(voxelFilePath);

        // Read the voxel data
        List<Voxel> voxelDataList = ReadVoxelData(voxelFileLines);
        Debug.Log("Data now read in, data list size: "+voxelDataList.Count);

        // Get the dimensions
        widthX = maxX - minX; //voxelFileLines.Dimensions[0];
        heightY = maxY - minY; //voxelFileLines.Dimensions[1];
        depthZ = maxZ - minZ; //voxelFileLines.Dimensions[2];

        Debug.Log("DICTIONARY width:" + widthX);
        Debug.Log("DICTIONARY height:" + heightY);
        Debug.Log("DICTIONARY depth:" + depthZ);

        return ConstructChunks(voxelDataList);

      }
  
    private Dictionary<Vector3Int, VoxelChunk> ConstructChunks(List<Voxel> sourceData)
    {
        Debug.Log("Data now read in, data list size: " + sourceData.Count);
        Debug.Log("Creating chunks of size ["+chunkSize+"] cubed.");

        Dictionary<Vector3Int, VoxelChunk> chunks = new Dictionary<Vector3Int, VoxelChunk>();

        int voxelsProcessed = 0;
        foreach (Voxel nextVoxelElement in sourceData)
        {
            Vector3Int chunkCoordinates = VoxelChunk.GetChunkCoordinates(nextVoxelElement.worldPosition, chunkSize);

            // Create new chunk if it doesn't exist
            if (!chunks.ContainsKey(chunkCoordinates))
            {
                Debug.Log("Creating new Chunk at position: "+chunkCoordinates);
                chunks[chunkCoordinates] = new VoxelChunk(chunkCoordinates);
            }

            // Add voxel to the corresponding chunk
            chunks[chunkCoordinates].AddVoxel(nextVoxelElement);
            voxelsProcessed++;
        }
        Debug.Log("Voxels processed:" + voxelsProcessed);
        Debug.Log("Number of chunks created: "+chunks.Count);
        return chunks;
    }


    public string[] ReadVoxelTextFile(string voxelTextFilePath)
    {
        // Load the text file
        return File.ReadAllLines(voxelTextFilePath);
    }

    private List<Voxel> ReadVoxelData(string[] lines)
    {
        //int numVoxels = width * height * depth;

        List<Voxel> voxelDataList = new List<Voxel>();

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
               
            voxelDataList.Add(new Voxel(new Vector3Int(x, y, z), parts[3])); // Assign the parsed color value as the voxel value
            index++;
        }
        Debug.Log("Lines processed:" + index);
        return voxelDataList;
    }
}