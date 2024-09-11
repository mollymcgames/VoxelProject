using System;
using System.IO;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class VoxelMeshConfigurationSettings
{
    [Header("Voxel Data Filename")]
    //Use streaming assets for the file path. e.g. "blue.txt" "voxtest.txt"
    public string voxelDataFileName = "";
    public string previousVoxelDataFileName = "";
    public string voxelDataSegmentFileName = "";

    [Header("Voxel Segment Layers")]
    public string[] voxelSegmentLayers;
       
    [Header("Voxel Configuration Attributes")]
    [Range(1, 64)]
    public int voxelChunkSize = 16;
    [Range(1, 6)]
    public int voxelChunkFieldOfViewMultiplier = 1;

    public Vector3Int standardVoxelSize = new Vector3Int(1, 1, 1);
    public Vector3Int voxelMeshCenter;

    [TagOptions()]
    public string voxelMeshContainerTagName;

    public int domainOffsetX = 0;
    public int domainOffsetY = 0;
    public int domainOffsetZ = 0;

    public string sceneTargetName;

    //[HideInInspector]
    public int visibilityThreshold = 1;

    [HideInInspector]
    public string voxelDataFilePath
    {
        get { return Path.Combine(Application.streamingAssetsPath, voxelDataFileName); }
    }
}