using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VoxelTextHandler : MonoBehaviour
{
    public static int xOffset = 16;
    public static int yOffset = 0;
    public static int zOffset = 0;

    public static string[] ReadVoxelTextFile(string voxelTextFilePath)
    {
        // Load the text file
        return File.ReadAllLines(voxelTextFilePath);
    }

    public static Dictionary<Vector3Int, Voxel> ReadVoxelData(string[] lines, int width, int height, int depth)
    {
        int numVoxels = width * height * depth;

        Dictionary<Vector3Int, Voxel> voxelDataList = new Dictionary<Vector3Int, Voxel>(numVoxels, new FastVector3IntComparer());

        int index = 0;
        foreach (var line in lines)
        {
            // Skip empty lines and comments
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            { 
                continue;
            }

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

            Color color;
            if (ColorUtility.TryParseHtmlString("#" + parts[3], out color))
            {
                voxelDataList.Add(new Vector3Int(x+xOffset, y+yOffset, z+zOffset), new Voxel((int)color.r));
            }             
        }
        return voxelDataList;
    }
}