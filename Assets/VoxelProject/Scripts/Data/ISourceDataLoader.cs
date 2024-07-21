using System.Collections.Generic;
using UnityEngine;

public interface ISourceDataLoader
{   
    //public Dictionary<Vector3Int, Chunk> LoadSourceData(string filepath);
    public VoxelStruct[,,] LoadSourceData(string filepath);

    public VoxelGrid LoadSourceDataGrid(string filepath);

    public object GetHeader();
}