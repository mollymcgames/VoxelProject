using System.IO;
using UnityEngine;

public class VoxelTextHandler : MonoBehaviour
{
    public static string[] ReadVoxelTextFile(string voxelTextFilePath)
    {
        // Load the text file
        return File.ReadAllLines(voxelTextFilePath);
    }

    public static Voxel[] ReadVoxelData(string[] lines, int width, int height, int depth)
    {
        int numVoxels = width * height * depth;

        Voxel[] voxelDataList = new Voxel[numVoxels];
        
        int index = 0;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue; // Skip empty lines and comments

            var parts = line.Split(' ');
            if (parts.Length != 4)
            {
                Debug.LogError($"Invalid line format: {line}");
                continue;
            }
            if (!int.TryParse(parts[0], out int x) ||
                !int.TryParse(parts[1], out int y) ||
                !int.TryParse(parts[2], out int z))
            {
                Debug.LogError($"Invalid integer values in line: {line}");
                continue;
            }

            voxelDataList[index++] = new Voxel(parts[3]);// x,y,z,parts[3]); // Assign the parsed color color as the voxel color
        }
        return voxelDataList;
    }
}