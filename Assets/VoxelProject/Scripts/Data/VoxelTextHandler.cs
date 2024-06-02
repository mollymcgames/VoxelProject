using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VoxelTextHandler : MonoBehaviour
{
    public string voxelDataFilePath = "Assets/Resources/blue.txt";

    public static string[] ReadVoxelTextFile(string voxelTextFilePath)
    {
        // Load the file
        return File.ReadAllLines(voxelTextFilePath);
    }

    public static VoxelCell[] ReadVoxelData(string[] lines, int width, int height, int depth)
    {
        int numVoxels = width * height * depth;

        VoxelCell[] voxelDataList = new VoxelCell[numVoxels];
        
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

            int argb = System.Int32.Parse(parts[3], System.Globalization.NumberStyles.HexNumber);            

            if (!int.TryParse(parts[0], out int x) ||
                !int.TryParse(parts[1], out int y) ||
                !int.TryParse(parts[2], out int z)) // Parse the string value as a float
            {
                Debug.LogError($"Invalid integer values in line: {line}");
                continue;
            }            

            voxelDataList[index++] = new VoxelCell(z, y, x, 127); // Assign the parsed float value
        }
        return voxelDataList;
    }
}