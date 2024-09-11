using System.Collections.Generic;
using UnityEngine;

//This class inherits from ASourceDataLoader which is an abstract class that implements the ISourceDataLoader interface.
//Used by zooming scene to load the source data from a text file.
public class SourceDataTextFileLoader : ASourceDataLoader
{
    public SourceDataTextFileLoader(int voxelOmissionThreshold)
    {
        this.voxelOmissionThreshold = voxelOmissionThreshold; //The level at which voxels are omitted (not displayed)
    }

    private string[] voxelFileLines = null;

    Dictionary<Vector3Int, Voxel> voxelData = null;

    public override Dictionary<Vector3Int, Voxel> LoadSourceData(string filepath)
    {
        Debug.Log("Loading TEXTFILE source data...");
        voxelDictionary = LoadVoxelFile(filepath);
        Debug.Log("The data has this many voxels: "+voxelDictionary.Count);

        WorldManager.Instance.voxelChunks = ConstructChunks(voxelDictionary);
        Debug.Log("The data has this many chunks: " + WorldManager.Instance.voxelChunks.Count);

        return voxelDictionary;
    }

    public override object GetHeader()
    {
        Nifti.NET.Nifti returnNiftiObject = new Nifti.NET.Nifti();
        returnNiftiObject.Dimensions = new int[3];        
        returnNiftiObject.Dimensions[0] = X;
        returnNiftiObject.Dimensions[1] = Y;
        returnNiftiObject.Dimensions[2] = Z;

        return returnNiftiObject;
    }

    public Dictionary<Vector3Int, Voxel> LoadVoxelFile(string voxelFilePath = "Assets/Resources/z.txt")
    {
        // Load default file
        voxelFileLines = VoxelTextHandler.ReadVoxelTextFile(voxelFilePath);

        // Get the dimensions
        X = 255;
        Y = 255;
        Z = 255;


        Debug.Log("TEXTFILE width:" + X);
        Debug.Log("TEXTFILE height:" + Y);
        Debug.Log("TEXTFILE depth:" + Z);

        // Read the voxel data
        voxelData = VoxelTextHandler.ReadVoxelData(voxelFileLines, X, Y, Z);
        Debug.Log("TEXTFILE Data now read in");

        return voxelData;
    }
}