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
                _instance = FindFirstObjectByType<SourceDataTextFileLoader>();
            return _instance;
        }
    }

    static string[] voxelFileLines = null;

    static VoxelCell[] voxelData = null;
    public static int widthX = 0;
    public static int heightY = 0;
    public static int depthZ = 0;

    public static VoxelCell[] LoadSourceData()
    {
        Debug.Log("Loading source data...");
        //use streaming assets for the file path
        return LoadVoxelFile("Assets/Resources/blue.txt");
    }

    public static VoxelCell[] LoadSourceData(string filepath)
    {
        Debug.Log("Loading source data...");
        return LoadVoxelFile(filepath);
    }    


    public static string[] GetHeader()
    {
        return voxelFileLines;
    }
    

    public static VoxelCell[] LoadVoxelFile(string voxelFilePath = "Assets/Resources/z.txt")
    {
        // Load default file
        voxelFileLines = VoxelTextHandler.ReadVoxelTextFile(voxelFilePath);

        // Get the dimensions
        widthX = 255;//voxelFileLines.Dimensions[0];
        heightY = 255;//voxelFileLines.Dimensions[1];
        depthZ = 255;//voxelFileLines.Dimensions[2];


        Debug.Log("NII width:" + widthX);
        Debug.Log("NII height:" + heightY);
        Debug.Log("NII depth:" + depthZ);

        // Read the voxel data
        voxelData = VoxelTextHandler.ReadVoxelData(voxelFileLines, widthX, heightY, depthZ);
        Debug.Log("Data now read in");

        return voxelData;
    }
}