using System.IO;
using UnityEngine;

[System.Serializable]
public class VoxelMeshConfigurationSettings
{
    [Header("Voxel Data Filename")]
    //Use streaming assets for the file path. e.g. "blue.txt" "voxtest.txt"
    public string voxelDataFileName = "";
       
    [Header("Voxel Configuration Attributes")]
    [Range(1, 64)]
    public int voxelChunkSize = 16;
    [Range(1, 6)]
    public int voxelChunkFieldOfViewMultiplier = 1;
    public string voxelMeshContainerTagName;

    [HideInInspector]
    public string voxelDataFilePath
    {
        get { return Path.Combine(Application.streamingAssetsPath, voxelDataFileName); }
    }
}