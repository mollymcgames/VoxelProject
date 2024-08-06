using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    private ASourceDataLoader loader = null;
    private string fileLastLoaded = "";
    private Nifti.NET.Nifti niftiFile = null;

    public float transitionDelayTime = 1.0f;
    private Animator animator;

    [HideInInspector]
    public string voxelFileFormat = "nii";

    public void LoadNextScene()
    {
        animator = GameObject.Find("Transition").GetComponent<Animator>();
        animator.SetTrigger("TriggerTransition");
        SceneManager.LoadScene("World");
    }

    public void LoadHeartFile()
    {
        // WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "ircad_e01_liver.nii";
        //WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "JHU-WhiteMatter-labels-2mm.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "la_003.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        LoadAFile(false);
    }
    public string LoadHeartFileHeader()
    {
        //WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "JHU-WhiteMatter-labels-2mm.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "la_003.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        return LoadAFileForTheHeader();
    }

    public void LoadLiverData()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "rkT1.nii";        
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = "rkT1-hot-voxels.csv";
        // WorldManager.Instance.voxelMeshConfigurationSettings.voxelSegmentLayers = new string[] { "rkT2.nii" };
        LoadAFile(true);
    }

    public string LoadLiverFileHeader()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "rkT1.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = "rkT1-hot-voxels.csv";
        return LoadAFileForTheHeader();
    }

    public void LoadBrainFile()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "avg152T1_LR_nifti.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        LoadAFile(false);
    }

    public string LoadBrainFileHeader()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "avg152T1_LR_nifti.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        return LoadAFileForTheHeader();
    }

    private string LoadAFileForTheHeader()
    {
        Debug.Log("Getting file header information...");
        //Use Steaming Assets folder to load the file
        string niftiFilePath = Path.Combine(Application.streamingAssetsPath, WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);

        // Load in one voxel model, currently support types are .txt and .nii files.
        // If more types need supporting, will need additional SourceData loader implementations.
        // Note, currently the getHeader method returns either a Nifti.NET.Nifti or string[] as appropriate.
        Debug.Log("Last file loaded: " + fileLastLoaded);
        Debug.Log("Settings datafile name: " + WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);
        if (!fileLastLoaded.Equals(WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName))
        {
            fileLastLoaded = WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName;
            Debug.Log("Loading file: " + fileLastLoaded);            
            loader = DataLoaderUtils.LoadDataFile();
            WorldManager.Instance.sourceData = loader.LoadSourceData(WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath);
            //original WorldManager.Instance.voxelGrid = loader.LoadSourceDataGrid(WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath);            
            niftiFile = (Nifti.NET.Nifti)loader.GetHeader(); // SourceDataLoader.GetHeader();
        }
        
        string retString = "<br>Dimensions: x(" + niftiFile.Dimensions[0] + ")/ y(" + niftiFile.Dimensions[1] + ")/ z(" + niftiFile.Dimensions[2] + ") <br>Filename: " + WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName;
        Debug.Log("RETSTRING: "+retString);   

        return retString;
    }

    private string ByteToString(byte[] source)
    {
        try
        {
            return source != null ? System.Text.Encoding.UTF8.GetString(source) : "";
        }
        catch
        {
            return "";
        }

/*        string response = string.Empty;

        foreach (byte b in bytes)
            response += (Char)b;

        return response;
*/    }

/*    private string ConvertByteToString(this byte[] source)
    {
        return source != null ? System.Text.Encoding.UTF8.GetString(source) : null;
    }*/

    private void LoadAFile(bool hasSegmentLayers)
    {
        Debug.Log("Loading file to display it: " + WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);
        // Use Steaming Assets folder to load the file
        string niftiFilePath = Path.Combine(Application.streamingAssetsPath, WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);

        // Load in one voxel model, currently support types are .txt and .nii files.
        // If more types need supporting, will need additional SourceData loader implementations.
        // Note, currently the getHeader method returns either a Nifti.NET.Nifti or string[] as appropriate.
        Debug.Log("Last file loaded: " + fileLastLoaded);
        Debug.Log("Settings datafile name: " + WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);
        if (!fileLastLoaded.Equals(WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName))
        {
            fileLastLoaded = WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName;
            Debug.Log("Loading file: " + fileLastLoaded);
            loader = DataLoaderUtils.LoadDataFile();
            WorldManager.Instance.sourceData = loader.LoadSourceData(WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath);
            //original WorldManager.Instance.voxelGrid = loader.LoadSourceDataGrid(WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath);
            niftiFile = (Nifti.NET.Nifti)loader.GetHeader(); // SourceDataLoader.GetHeader();
        }
        
        Debug.Log("Description, " + System.Text.Encoding.Default.GetString(niftiFile.Header.descrip));
        Debug.Log("Dimensions, " + niftiFile.Dimensions[0] + ", " + niftiFile.Dimensions[1] + ", " + niftiFile.Dimensions[2]);
        Debug.Log("Filename, " + niftiFilePath);

        if (WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName != null)
        {
            Debug.Log("Loading Voxel Segment Definition File: " + WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName);
            //Use Steaming Assets folder to load the file
            string voxelSegmentDefinitionFilePath = Path.Combine(Application.streamingAssetsPath, WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName);

            WorldManager.Instance.sourceData = loader.LoadVoxelSegmentDefinitionFileExtra(voxelSegmentDefinitionFilePath);
        }

        WorldManager.Instance.worldSettings.maxWidthX = loader.widthX;
        WorldManager.Instance.worldSettings.maxHeightY = loader.heightY;
        WorldManager.Instance.worldSettings.maxDepthZ = loader.depthZ;

        Debug.Log("Loaded world dimensions: " + WorldManager.WorldSettings.maxWidthX + ", " + WorldManager.WorldSettings.maxHeightY + ", " + WorldManager.WorldSettings.maxDepthZ);
        Debug.Log("Loaded world dimensions2: " + WorldManager.Instance.worldSettings.maxWidthX + ", " + WorldManager.Instance.worldSettings.maxHeightY + ", " + WorldManager.Instance.worldSettings.maxDepthZ);

        //Debug.Log("Random voxel color 0: " + WorldManager.Instance.sourceData[20, 20, 20].getColourRGBLayer(0));
        //Debug.Log("Voxel hot color: " + WorldManager.Instance.sourceData[21, 30, 0].getHotVoxelColourRGB().ToString());
        //Debug.Log("Voxel hot bool: " + WorldManager.Instance.sourceData[21, 30, 0].isHotVoxel.ToString());

        //Debug.Log("Random voxel color 0: " + WorldManager.Instance.voxelGrid.GetVoxelUsingWorldPosition(new Vector3Int(50, 50, 50)).getColourRGBLayer(0));

        // TODO
/*        if (hasSegmentLayers)
        {
            int segmentLayer = 1;
            foreach (string nextSegmentFile in WorldManager.Instance.voxelMeshConfigurationSettings.voxelSegmentLayers)
            {
                WorldManager.Instance.sourceData = loader.LoadVoxelSegmentDefinitionFile(ref WorldManager.Instance.sourceData, segmentLayer++, nextSegmentFile);
            }        
            voxelFileFormat = DataLoaderUtils.GetDataFileFormat();
            Debug.Log("Random voxel color 1: " + WorldManager.Instance.sourceData[20, 20, 20].getColourRGBLayer(1));
        }  */ 
    }

    public void LoadZoom()
    {
        SceneManager.LoadScene("Zooming");
    }

    public void BackButton()
    {
        // TODO animator = GameObject.Find("Transition").GetComponent<Animator>();
        // TODO animator.SetTrigger("TriggerTransition");
        SceneManager.LoadScene("Model");
    }

    public void BackToWorldButton()
    {
        // TODO animator = GameObject.Find("Transition").GetComponent<Animator>();
        // TODO animator.SetTrigger("TriggerTransition");
        SceneManager.LoadScene("World");
    }
}
