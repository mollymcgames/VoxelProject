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
        int voxelOmissionThreshold = 0;
        // WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "ircad_e01_liver.nii";
        //WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "JHU-WhiteMatter-labels-2mm.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "la_007.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        LoadAFile(false, voxelOmissionThreshold);
    }
    public string LoadHeartFileHeader()
    {
        int voxelOmissionThreshold = 0;
        //WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "JHU-WhiteMatter-labels-2mm.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "la_007.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        return LoadAFileForTheHeader(voxelOmissionThreshold);
    }

    public void LoadLiverData()
    {
        int voxelOmissionThreshold = 0;
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "rkT1.nii";        
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = "rkT1-hot-voxels.csv";
        // WorldManager.Instance.voxelMeshConfigurationSettings.voxelSegmentLayers = new string[] { "rkT2.nii" };
        LoadAFile(false, voxelOmissionThreshold);
    }

    public string LoadLiverFileHeader()
    {
        int voxelOmissionThreshold = 0;
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "rkT1.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = "rkT1-hot-voxels.csv";
        return LoadAFileForTheHeader(voxelOmissionThreshold);
    }

    public void LoadBrainFile()
    {
        int voxelOmissionThreshold = 18;
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "avg152T1_LR_nifti.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        LoadAFile(false, voxelOmissionThreshold);
    }

    public string LoadBrainFileHeader()
    {
        int voxelOmissionThreshold = 18;
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "avg152T1_LR_nifti.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        return LoadAFileForTheHeader(voxelOmissionThreshold);
    }

    private string LoadAFileForTheHeader(int voxelOmissionThreshold)
    {
        // Debug.Log("Getting file header information...");
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
            loader = DataLoaderUtils.LoadDataFile(voxelOmissionThreshold);
            WorldManager.Instance.voxelDictionary = loader.LoadSourceData(WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath);
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

    private void LoadAFile(bool hasSegmentLayers, int voxelOmissionThreshold)
    {
        // Debug.Log("Loading file to display it: " + WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);
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
            loader = DataLoaderUtils.LoadDataFile(voxelOmissionThreshold);
            WorldManager.Instance.voxelDictionary = loader.LoadSourceData(WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath);
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

            WorldManager.Instance.voxelDictionary = loader.LoadVoxelSegmentDefinitionFileExtra(voxelSegmentDefinitionFilePath);
        }

        WorldManager.Instance.worldSettings.maxWidthX = loader.widthX;
        WorldManager.Instance.worldSettings.maxHeightY = loader.heightY;
        WorldManager.Instance.worldSettings.maxDepthZ = loader.depthZ;

        Debug.Log("Loaded world dimensions: " + WorldManager.Instance.worldSettings.maxWidthX + ", " + WorldManager.Instance.worldSettings.maxHeightY + ", " + WorldManager.Instance.worldSettings.maxDepthZ);

        // WorldManager.Instance.voxelsSelected = WorldManager.Instance.worldSettings.maxWidthX * WorldManager.Instance.worldSettings.maxHeightY * WorldManager.Instance.worldSettings.maxDepthZ;
        //Debug.Log("Random voxel color 0: " + WorldManager.Instance.voxelDictionary[20, 20, 20].getColourRGBLayer(0));
        //Debug.Log("Voxel hot color: " + WorldManager.Instance.voxelDictionary[21, 30, 0].getHotVoxelColourRGB().ToString());
        //Debug.Log("Voxel hot bool: " + WorldManager.Instance.voxelDictionary[21, 30, 0].isHotVoxel.ToString());

        //Debug.Log("Random voxel color 0: " + WorldManager.Instance.voxelGrid.GetVoxelUsingWorldPosition(new Vector3Int(50, 50, 50)).getColourRGBLayer(0));

        // TODO
/*        if (hasSegmentLayers)
        {
            int segmentLayer = 1;
            foreach (string nextSegmentFile in WorldManager.Instance.voxelMeshConfigurationSettings.voxelSegmentLayers)
            {
                WorldManager.Instance.voxelDictionary = loader.LoadVoxelSegmentDefinitionFile(ref WorldManager.Instance.voxelDictionary, segmentLayer++, nextSegmentFile);
            }        
            voxelFileFormat = DataLoaderUtils.GetDataFileFormat();
            Debug.Log("Random voxel color 1: " + WorldManager.Instance.voxelDictionary[20, 20, 20].getColourRGBLayer(1));
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
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "";
        SceneManager.LoadScene("Model");
    }

    public void BackToWorldButton()
    {
        // TODO animator = GameObject.Find("Transition").GetComponent<Animator>();
        // TODO animator.SetTrigger("TriggerTransition");
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "";
        SceneManager.LoadScene("World");
    }
}
