using System.Collections.Generic;
using UnityEngine;

public interface ISourceDataLoader
{   
    //public Dictionary<Vector3Int, Chunk> LoadSourceData(string filepath);
    public Voxel[,,] LoadSourceData(string filepath);

    public object GetHeader();
}