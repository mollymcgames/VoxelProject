using System.Collections.Generic;
using System.IO;
using Nifti.NET;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class SourceDataTextFileLoaderAsDictionary : ASourceDataLoader
{   
    public SourceDataTextFileLoaderAsDictionary(int chunkSize) : base(chunkSize) {}

    private string[] voxelFileLines = null;

    public override Dictionary<Vector3Int, Chunk> LoadSourceData(string filepath)
    {
        Debug.Log("Loading file source data as Dictionary...");
        return LoadVoxelFile(filepath);
    }    

    public override object GetHeader()
    {
        return voxelFileLines;
    }

    #region
    private Dictionary<Vector3Int, Chunk> LoadVoxelFile(string voxelFilePath = "Assets/Resources/z.txt")
    {
        // Load default file
        voxelFileLines = ReadVoxelTextFile(voxelFilePath);

        // Read the voxel data
        List<VoxelElement> voxelDataList = ReadVoxelData(voxelFileLines);
        Debug.Log("Data now read in, data list size: "+voxelDataList.Count);

        // Get the dimensions
        widthX = maxX - minX;
        heightY = maxY - minY;
        depthZ = maxZ - minZ;

        Debug.Log("DICTIONARY width:" + widthX);
        Debug.Log("DICTIONARY height:" + heightY);
        Debug.Log("DICTIONARY depth:" + depthZ);

        return ConstructChunks(voxelDataList);
    }
  
    private string[] ReadVoxelTextFile(string voxelTextFilePath)
    {
        // Load the text file
        return File.ReadAllLines(voxelTextFilePath);
    }

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

            // Assign the parsed color value as the voxel value
            voxelDataList.Add(new VoxelElement(new Vector3Int(x, y, z), parts[3])); 
            index++;
        }
        Debug.Log("Lines processed:" + index);
        return voxelDataList;
    }
    #endregion
}