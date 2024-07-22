using Nifti.NET;
using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class SourceDataLoaderV1 : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private static SourceDataLoaderV1 _instance;

    void Start()
    {
        if (_instance != null)
        {
            if (_instance != this)
                Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    public static SourceDataLoaderV1 Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SourceDataLoaderV1>();
            return _instance;
        }
    }

    //static string niftiFilePath = "Assets/Resources/la_007.nii";
    static string niftiFilePath = "Assets/Resources/avg152T1_LR_nifti.nii";
    //static string niftiFilePath = "Assets/Resources/jhu.nii";

    //Use Steaming Assets folder to load the file
    static string niftiFilePath2 = Path.Combine(Application.streamingAssetsPath, "JHU-WhiteMatter-labels-2mm.nii");
    // static string niftiFilePath2 = "Assets/Resources/JHU-WhiteMatter-labels-2mm.nii"; //small datafile 

    static Nifti.NET.Nifti niftiFile = null;

    static VoxelCell[] voxelData = null;
    public static int widthX = 0;
    public static int heightY = 0;
    public static int depthZ = 0;

    public static VoxelCell[] LoadSourceData()
    {
        Debug.Log("Loading source data...");
        return LoadNiftiFile();
    }

    
    public static VoxelCell[] LoadSourceData(string filepath)
    {
        Debug.Log("Loading source data...");
        return LoadNiftiFile(filepath);
    }    


    public static Nifti.NET.Nifti GetHeader()
    {
        return niftiFile;
    }
    
    public static void OpenNiftiFile(string filePath)
    {
        niftiFile = null;
        niftiFile = NiftiHandler.ReadNiftiFile(filePath);
    }

    public static VoxelCell[] LoadNiftiFile()
    {
        // Load default file
        if (niftiFile == null)
        {
            niftiFile = NiftiHandler.ReadNiftiFile(niftiFilePath2);
        }

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

    public static VoxelCell[] LoadNiftiFile(string filePath)
    {
        // original SourceDataLoader.OpenNiftiFile(filePath);            

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