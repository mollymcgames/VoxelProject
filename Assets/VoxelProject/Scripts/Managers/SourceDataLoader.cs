using Nifti.NET;
using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;

public class SourceDataLoader : MonoBehaviour
{

    //static string niftiFilePath = "Assets/Resources/la_007.nii";
    static string niftiFilePath = "Assets/Resources/avg152T1_LR_nifti.nii";
    //static string niftiFilePath = "Assets/Resources/jhu.nii";

    static VoxelCell[] voxelData = null;
    public static int widthX = 0;
    public static int heightY = 0;
    public static int depthZ = 0;

    public static VoxelCell[] LoadSourceData()
    {
        Debug.Log("Loading source data...");
        return LoadNiftiFile(niftiFilePath);
    }

    public static VoxelCell[] LoadNiftiFile(string filePath)
    {
        //Program.NiftiHeader header = Program.ReadNiftiHeader(niftiFilePath);
        Nifti.NET.Nifti niftiFile = NiftiHandler.ReadNiftiFile(niftiFilePath);

        // Get the dimensions
        widthX = niftiFile.Dimensions[0];
        heightY = niftiFile.Dimensions[1];
        depthZ = niftiFile.Dimensions[2];


        Debug.Log("NII width:" + widthX);
        Debug.Log("NII height:" + heightY);
        Debug.Log("NII depth:" + depthZ);

        // Calculate the number of voxels
        //int numVoxels = width * height * depth;

        // Read the voxel data
        voxelData = NiftiHandler.ReadNiftiData(niftiFile, widthX, heightY, depthZ);
        Debug.Log("Data now read in");

        return voxelData;
    }
}