using System.Collections.Generic;
using UnityEngine;

public class SourceDataLoader : ASourceDataLoader
{
    private static Nifti.NET.Nifti niftiFile = null;
    private static Nifti.NET.Nifti niftiSegmentFile = null;

    public SourceDataLoader(int voxelOmissionThreshold) { 
        this.voxelOmissionThreshold = voxelOmissionThreshold;
    }
    
    public override Dictionary<Vector3Int, Voxel> LoadSourceData(string filepath)
    {
        Debug.Log("Loading nii source data...:" + filepath);
        LoadNiftiFile(filepath);
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelMeshCenter = CalculateCenter(niftiFile.Dimensions[0], niftiFile.Dimensions[1], niftiFile.Dimensions[2]);
        CreateVoxelsArray();
        WorldManager.Instance.voxelChunks = ConstructChunks(voxelDictionary);
        return voxelDictionary;
    }

    public override object GetHeader()
    {
        return niftiFile;
    }
    
    private void LoadNiftiFile(string filePath)
    {
        // Load default file
        niftiFile = ReadNiftiFile(filePath);

        // Get the dimensions
        X = niftiFile.Dimensions[0];
        Y = niftiFile.Dimensions[1];
        Z = niftiFile.Dimensions[2];

        Debug.Log("NII width:" + X);
        Debug.Log("NII height:" + Y);
        Debug.Log("NII depth:" + Z);
    }

    private void LoadNiftiSegmentFile(string filePath)
    {
        // Load default file
        niftiSegmentFile = ReadNiftiFile(filePath);

        // Get the dimensions
        X = niftiSegmentFile.Dimensions[0];
        Y = niftiSegmentFile.Dimensions[1];
        Z = niftiSegmentFile.Dimensions[2];

        // Rebuild chunks
        WorldManager.Instance.voxelChunks = ConstructChunks(voxelDictionary);
    }

    private void CreateVoxelsArray()
    {
        Debug.Log("Data being read in and loaded into Dictionary...");
        // Read the voxel data
        voxelDictionary = NiftiHandler.ReadNiftiData(niftiFile, X, Y, Z, voxelOmissionThreshold);
        Debug.Log("Data now read in and Dictionary created. Size: "+voxelDictionary.Count);
    }

    private Nifti.NET.Nifti ReadNiftiFile(string niftiFilePath)
    {
        // Load the NIfTI file
        return NiftiHandler.ReadNiftiFile(niftiFilePath);
    }

}