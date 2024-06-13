using System.Collections.Generic;
using System.IO;
using Nifti.NET;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class SourceDataTextFileLoaderAsDictionary
{
    public int chunkSize = OuterWorldManager.Instance.chunkSize;

    string[] voxelFileLines = null;

    VoxelCell[] voxelData = null;
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
        List<VoxelElement> voxelDataList = ReadVoxelData(voxelFileLines);
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
  
    private Dictionary<Vector3Int, Chunk> ConstructChunks(List<VoxelElement> sourceData)
    {
        Debug.Log("INNER Data now read in, data list size: " + sourceData.Count);
        Debug.Log("Creating chunks of size ["+chunkSize+"] cubed.");

        Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

        int voxelsProcessed = 0;
        foreach (VoxelElement nextVoxelElement in sourceData)
        {
            Vector3Int chunkCoordinates = Chunk.GetChunkCoordinates(nextVoxelElement.position, chunkSize);

            // Create new chunk if it doesn't exist
            if (!chunks.ContainsKey(chunkCoordinates))
            {
                Debug.Log("Creating new Chunk at position: "+chunkCoordinates);
                chunks[chunkCoordinates] = new Chunk(chunkCoordinates);
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

    private int maxX = 0;
    private int maxY = 0;
    private int maxZ = 0;
    private int minX = 0;
    private int minY = 0;
    private int minZ = 0;

    private List<VoxelElement> ReadVoxelData(string[] lines)
    {
        //int numVoxels = width * height * depth;

        List<VoxelElement> voxelDataList = new List<VoxelElement>();

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
               
            voxelDataList.Add(new VoxelElement(new Vector3Int(x, y, z), parts[3])); // Assign the parsed color value as the voxel value
        }
        Debug.Log("Lines processed:" + index);
        return voxelDataList;
    }
}