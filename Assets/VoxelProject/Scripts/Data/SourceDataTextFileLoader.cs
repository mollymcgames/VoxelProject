using Nifti.NET;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SourceDataTextFileLoader : ASourceDataLoader
{
    public SourceDataTextFileLoader(int voxelOmissionThreshold)
    {
        this.voxelOmissionThreshold = voxelOmissionThreshold;
    }

/*    public void Awake()
    {
        DontDestroyOnLoad(this);
    }*/

    private static SourceDataTextFileLoader _instance;

/*    void Start()
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
    }*/

/*    public static SourceDataTextFileLoader Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<SourceDataTextFileLoader>();
            return _instance;
        }
    }*/

    private string[] voxelFileLines = null;

    Dictionary<Vector3Int, Voxel> voxelData = null;

    public override Dictionary<Vector3Int, Voxel> LoadSourceData(string filepath)
    {
        Debug.Log("Loading TEXTFILE source data...");
        voxelDictionary = LoadVoxelFile(filepath);
        Debug.Log("The data has this many voxels: "+voxelDictionary.Count);

        //WorldManager.Instance.voxelMeshConfigurationSettings.voxelMeshCenter = CalculateCenter(this.X, this.Y, this.Z);
        WorldManager.Instance.voxelChunks = ConstructChunks(voxelDictionary);
        Debug.Log("The data has this many chunks: " + WorldManager.Instance.voxelChunks.Count);

        return voxelDictionary;
    }

/*    public Dictionary<Vector3Int, Voxel> LoadSourceData()
    {
        Debug.Log("Loading TEXTFILE source data...");
        //use streaming assets for the file path
        return LoadVoxelFile("Assets/Resources/blue.txt");
    }*/

    // KJP PRE CONVERSION DELETE ONCE WORKING
    // public static Voxel[] LoadSourceData(string filepath)
    //{
    //    Debug.Log("Loading source data...");
    //    return LoadVoxelFile(filepath);
    //}    

    public override object GetHeader()
    {
        Nifti.NET.Nifti returnThing = new Nifti.NET.Nifti();
        returnThing.Dimensions = new int[3];        
        returnThing.Dimensions[0] = X;
        returnThing.Dimensions[1] = Y;
        returnThing.Dimensions[2] = Z;

        //returnThing.Header = new NiftiHeader();
        //returnThing.Header.descrip = Encoding.UTF8.GetBytes("Manually loaded voxel file.");

        //string retString = "<br>Dimensions: x(" + returnThing.Dimensions[0] + ")/ y(" + returnThing.Dimensions[1] + ")/ z(" + returnThing.Dimensions[2] + ")"; // <br>Filename: " + WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName;
        //Debug.Log("RRRRETSTRING: " + retString);

        return returnThing;
    }

    public Dictionary<Vector3Int, Voxel> LoadVoxelFile(string voxelFilePath = "Assets/Resources/z.txt")
    {
        // Load default file
        voxelFileLines = VoxelTextHandler.ReadVoxelTextFile(voxelFilePath);

        // Get the dimensions
        X = 255;//voxelFileLines.Dimensions[0];
        Y = 255;//voxelFileLines.Dimensions[1];
        Z = 255;//voxelFileLines.Dimensions[2];


        Debug.Log("TEXTFILE width:" + X);
        Debug.Log("TEXTFILE height:" + Y);
        Debug.Log("TEXTFILE depth:" + Z);

        // Read the voxel data
        voxelData = VoxelTextHandler.ReadVoxelData(voxelFileLines, X, Y, Z);
        Debug.Log("TEXTFILE Data now read in");

        return voxelData;
    }


/*    public static Voxel[] LoadVoxelFile(string voxelFilePath = "Assets/Resources/z.txt")
    {
        // Load default file
        voxelFileLines = VoxelTextHandler.ReadVoxelTextFile(voxelFilePath);

        // Get the dimensions
        X = 255;//voxelFileLines.Dimensions[0];
        Y = 255;//voxelFileLines.Dimensions[1];
        Z = 255;//voxelFileLines.Dimensions[2];


        Debug.Log("NII width:" + X);
        Debug.Log("NII height:" + Y);
        Debug.Log("NII depth:" + Z);

        // Read the voxel data
        voxelData = VoxelTextHandler.ReadVoxelData(voxelFileLines, X, Y, Z);
        Debug.Log("Data now read in");

        return voxelData;
    }*/
}