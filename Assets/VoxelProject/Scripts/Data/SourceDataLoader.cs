using Nifti.NET;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class SourceDataLoader : ASourceDataLoader
{
    public SourceDataLoader(int chunkSize) : base(chunkSize) { }

    private static Nifti.NET.Nifti niftiFileLines = null;
    
    //public static VoxelCell[] LoadSourceData(string filepath)
    public override Dictionary<Vector3Int, Chunk> LoadSourceData(string filepath)
    {
        Debug.Log("Loading nii source data as Dictionary...");
        return LoadNiftiFile(filepath);
    }    

    public override object GetHeader()
    {
        return niftiFileLines;
    }

    #region
    //private VoxelCell[] LoadNiftiFile()
    private Dictionary<Vector3Int, Chunk> LoadNiftiFile(string niftiFilePath)
    {
        // Load default file
        niftiFileLines = ReadNiftiFile(niftiFilePath);
        
        // Get the dimensions
        widthX = niftiFileLines.Dimensions[0];
        heightY = niftiFileLines.Dimensions[1];
        depthZ = niftiFileLines.Dimensions[2];

        Debug.Log("NII width:" + widthX);
        Debug.Log("NII height:" + heightY);
        Debug.Log("NII depth:" + depthZ);

        // Calculate the number of voxels
        //int numVoxels = width * height * depth;

        // Read the voxel data
        List<VoxelElement> voxelDataList = ReadNiftiData(niftiFileLines, widthX, heightY, depthZ);
        Debug.Log("Data now read in");

        return ConstructChunks(voxelDataList);
    }

    public void OpenNiftiFile(string filePath)
    {
        niftiFileLines = ReadNiftiFile(filePath);
    }

    private Nifti.NET.Nifti ReadNiftiFile(string niftiFilePath)
    {
        // Load the NIfTI file
        return NiftiFile.Read(niftiFilePath);
    }


    private List<VoxelElement> ReadNiftiData(Nifti.NET.Nifti niftiData, int width, int height, int depth)
    {
        List<VoxelElement> voxelDataList = new List<VoxelElement>();

        // Iterate through each voxel
        int index = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x > maxX) maxX = x;
                    if (x < minX) minX = x;
                    if (y > maxY) maxY = y;
                    if (y < minY) minY = y;
                    if (z > maxZ) maxZ = z;
                    if (z < minZ) minZ = z;

                    // Assign the parsed color value as the voxel value if it's going to be visible
                    if (niftiData.Data[index] > 10)
                        voxelDataList.Add(new VoxelElement(new Vector3Int(x, y, z), niftiData.Data[index].ToString()));
                    index++;
                }
            }
        }
        Debug.Log("Lines processed:" + index);
        return voxelDataList;
    }

    #endregion

    //static string niftiFilePath = "Assets/Resources/la_007.nii";
    //static string niftiFilePath = "Assets/Resources/avg152T1_LR_nifti.nii";
    //static string niftiFilePath = "Assets/Resources/jhu.nii";

    //Use Steaming Assets folder to load the file
    //static string niftiFilePath2 = Path.Combine(Application.streamingAssetsPath, "JHU-WhiteMatter-labels-2mm.nii");
    // static string niftiFilePath2 = "Assets/Resources/JHU-WhiteMatter-labels-2mm.nii"; //small datafile 

    /*    public void Awake()
        {
            DontDestroyOnLoad(this);
        }*/

    /*    private static SourceDataLoader _instance;

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
    */
    /*    public static SourceDataLoader Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<SourceDataLoader>();
                return _instance;
            }
        }*/


    //public static VoxelCell[] LoadSourceData()
    /*    public Dictionary<Vector3Int, Chunk> LoadSourceData()    
        {
            Debug.Log("Loading nii source data as Dictionary [a]...");
            return LoadNiftiFile();
        }*/

}