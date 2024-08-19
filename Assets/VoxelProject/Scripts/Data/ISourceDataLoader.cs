using System.Collections.Generic;
using UnityEngine;

public interface ISourceDataLoader
{   
    public Dictionary<Vector3Int, Voxel> LoadSourceData(string filepath);

    public Dictionary<Vector3Int, Voxel> LoadVoxelSegmentDefinitionFile(int segmentLayer, string voxelSegmentDefinitionFilePath);

    public object GetHeader();
}