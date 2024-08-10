using System.Collections.Generic;
using UnityEngine;

public interface ISourceDataLoader
{   
    //public Dictionary<Vector3Int, Chunk> LoadSourceData(string filepath);
    // FIXP public VoxelCell[,,] LoadSourceData(string filepath);
    public Dictionary<long, VoxelCell> LoadSourceData(string filepath);

    // public VoxelCell[,,] LoadVoxelSegmentDefinitionFile(int segmentLayer, string voxelSegmentDefinitionFilePath);
    public Dictionary<long, VoxelCell> LoadVoxelSegmentDefinitionFile(int segmentLayer, string voxelSegmentDefinitionFilePath);

    //public VoxelGrid LoadSourceDataGrid(string filepath);

    public object GetHeader();
}