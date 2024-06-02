using Nifti.NET;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class SourceDataTextFileLoader : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private static SourceDataTextFileLoader _instance;

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

    public static SourceDataTextFileLoader Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SourceDataTextFileLoader>();
            return _instance;
        }
    }

    static string voxelFilePath = "Assets/Resources/voxtest.txt";

    static string[] voxelFileLines = null;

    static VoxelCell[] voxelData = null;
    public static int widthX = 0;
    public static int heightY = 0;
    public static int depthZ = 0;

    public static VoxelCell[] LoadSourceData()
    {
        Debug.Log("Loading source data...");
        return LoadVoxelFile();
    }


    public static string[] GetHeader()
    {
        return voxelFileLines;
    }
    
    public static void OpenNiftiFile(string filePath)
    {
        voxelFileLines = null;
        voxelFileLines = VoxelTextHandler.ReadVoxelTextFile(voxelFilePath);
    }

    public static VoxelCell[] LoadVoxelFile()
    {
        // Load default file
        if (voxelFileLines == null)
        {
            voxelFileLines = VoxelTextHandler.ReadVoxelTextFile(voxelFilePath);
        }

        // Get the dimensions
        widthX = 255;//voxelFileLines.Dimensions[0];
        heightY = 255;//voxelFileLines.Dimensions[1];
        depthZ = 255;//voxelFileLines.Dimensions[2];


        Debug.Log("NII width:" + widthX);
        Debug.Log("NII height:" + heightY);
        Debug.Log("NII depth:" + depthZ);

        // Calculate the number of voxels
        //int numVoxels = width * height * depth;

        // Read the voxel data
        voxelData = VoxelTextHandler.ReadVoxelData(voxelFileLines, widthX, heightY, depthZ);
        Debug.Log("Data now read in");

        return voxelData;
    }

    public static VoxelCell[] LoadNiftiFile(string filePath)
    {
        SourceDataLoader.OpenNiftiFile(filePath);            

        // Get the dimensions
        widthX = 255;//voxelFileLines.Dimensions[0];
        heightY = 255;//voxelFileLines.Dimensions[1];
        depthZ = 255;//voxelFileLines.Dimensions[2];


        Debug.Log("NII width:" + widthX);
        Debug.Log("NII height:" + heightY);
        Debug.Log("NII depth:" + depthZ);

        // Calculate the number of voxels
        //int numVoxels = width * height * depth;

        // Read the voxel data
        voxelData = VoxelTextHandler.ReadVoxelData(voxelFileLines, widthX, heightY, depthZ);
        Debug.Log("Data now read in");

        return voxelData;
    }
}