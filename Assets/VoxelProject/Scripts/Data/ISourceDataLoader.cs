using System.Collections.Generic;
using UnityEngine;

//Interface with three methods for structure.
public interface ISourceDataLoader
{   
    public Dictionary<Vector3Int, Voxel> LoadSourceData(string filepath); //Load the source data from a file

    public Dictionary<Vector3Int, Voxel> LoadVoxelSegmentDefinitionFile(int segmentLayer, string voxelSegmentDefinitionFilePath); //Load the source data from a file

    public object GetHeader(); //Get the header of the file
}