using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SourceDataLoader : ASourceDataLoader
{
    public SourceDataLoader(int chunkSize) : base(chunkSize) { }

    private static Nifti.NET.Nifti niftiFile = null;
    private static Nifti.NET.Nifti niftiSegmentFile = null;    
    
    // D AS L public override Dictionary<long, VoxelCell> LoadSourceData(string filepath)
    public override Dictionary<Vector3Int, Voxel> LoadSourceData(string filepath)
    {
        Debug.Log("Loading nii source data...:" + filepath);
        LoadNiftiFile(filepath);
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelMeshCenter = CalculateCenter(niftiFile.Dimensions[0], niftiFile.Dimensions[1], niftiFile.Dimensions[2]);
        CreateVoxelsArray();
        return voxelDictionary;
    }

/*    public override VoxelGrid LoadSourceDataGrid(string filepath)
    {
        Debug.Log("Loading nii source data in GRID format...:" + filepath);
        LoadNiftiFile(filepath);
        CreateVoxelsArrayGrid();
        return voxelGrid;
    }*/
   
    public override object GetHeader()
    {
        return niftiFile;
    }

    #region
    //private Dictionary<Vector3Int, Chunk> LoadNiftiFile(string niftiFilePath)
    private void LoadNiftiFile(string filePath)
    {
        // Load default file
        niftiFile = ReadNiftiFile(filePath);

        // Get the dimensions
        widthX = niftiFile.Dimensions[0];
        heightY = niftiFile.Dimensions[1];
        depthZ = niftiFile.Dimensions[2];

        Debug.Log("NII width:" + widthX);
        Debug.Log("NII height:" + heightY);
        Debug.Log("NII depth:" + depthZ);
    }

    private void LoadNiftiSegmentFile(string filePath)
    {
        // Load default file
        niftiSegmentFile = ReadNiftiFile(filePath);

        // Get the dimensions
        widthX = niftiSegmentFile.Dimensions[0];
        heightY = niftiSegmentFile.Dimensions[1];
        depthZ = niftiSegmentFile.Dimensions[2];
    }

    private void CreateVoxelsArray()
    {
        Debug.Log("Data being read in and loaded into Dictionary...");
        // Read the voxel data
        voxelDictionary = NiftiHandler.ReadNiftiData(niftiFile, widthX, heightY, depthZ);
        Debug.Log("Data now read in and Dictionary created. Size: "+voxelDictionary.Count);
    }

/*    private void CreateVoxelsArrayGrid()
    {
        // Read the voxel data
        voxelGrid = NiftiHandler.ReadNiftiDataGrid(niftiFile, x, y, z, chunkSize);
        Debug.Log("Data (grid) now read in");
    }*/

    public void OpenNiftiFile(string filePath)
    {
        niftiFile = ReadNiftiFile(filePath);
    }

    private Nifti.NET.Nifti ReadNiftiFile(string niftiFilePath)
    {
        // Load the NIfTI file
        return NiftiHandler.ReadNiftiFile(niftiFilePath);
    }
    #endregion

}