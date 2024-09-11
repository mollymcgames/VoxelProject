using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    private ASourceDataLoader loader = null;

    private string fileLastLoaded = ""; // Used to store the last file loaded, to prevent reloading the same file.

    private Nifti.NET.Nifti niftiFile = null; // Used to store the header data for the nifti file.

    public float transitionDelayTime = 1.0f;
    private Animator animator; //Animator for the transition effect (fade in/out)

    [HideInInspector]
    public string voxelFileFormat = "nii";

    public void LoadNextScene()
    {
        animator = GameObject.Find("Transition").GetComponent<Animator>();
        animator.SetTrigger("TriggerTransition");
        SceneManager.LoadScene("World");        
    }

    //Loads the next scene (world) with the selected file and animation
    public void LoadWorldScene()
    {
        animator = GameObject.Find("Transition").GetComponent<Animator>();
        animator.SetTrigger("TriggerTransition");        
        SceneManager.LoadScene("Model");
    }

    public void LoadInstructionsScene()
    {
        animator = GameObject.Find("Transition").GetComponent<Animator>();
        animator.SetTrigger("TriggerTransition");
        SceneManager.LoadScene("MenuInstructions");
    }

    public void LoadMenuScene()
    {
        animator = GameObject.Find("Transition").GetComponent<Animator>();
        animator.SetTrigger("TriggerTransition");
        SceneManager.LoadScene("MainMenu");
    }

    //Loads a pre-configured heart voxel file 
    public void LoadHeartFile()
    {
        int voxelOmissionThreshold = 2;
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "natbrainlab.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        WorldManager.Instance.worldSettings.sparseVoxels = false;
        LoadAFile(false, voxelOmissionThreshold);
    }

    //Loads the heart file header
    public string LoadHeartHeader()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "natbrainlab.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        WorldManager.Instance.worldSettings.sparseVoxels = true;
        return LoadHeaderOnly();
    }

    //Loads a pre-configured spine voxel file
    public void LoadSpineFile()
    {
        int voxelOmissionThreshold = 0;
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "spine.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        WorldManager.Instance.worldSettings.sparseVoxels = true;
        LoadAFile(false, voxelOmissionThreshold);
    }
    //Loads the spine file header
    public string LoadSpineHeader()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "spine.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        WorldManager.Instance.worldSettings.sparseVoxels = true;
        return LoadHeaderOnly();
    }    

    public void LoadLiverFile()
    {
        int voxelOmissionThreshold = 0;
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "rkT1.nii";        
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = "rkT1-hot-voxels.csv";
        WorldManager.Instance.worldSettings.sparseVoxels = false;
        LoadAFile(false, voxelOmissionThreshold);
    }

    public string LoadLiverHeader()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "rkT1.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        WorldManager.Instance.worldSettings.sparseVoxels = true;
        return LoadHeaderOnly();
    }

    public void LoadBrainFile()
    {
        int voxelOmissionThreshold = 18;
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "avg152T1_LR_nifti.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        WorldManager.Instance.worldSettings.sparseVoxels = false;
        LoadAFile(false, voxelOmissionThreshold);
    }

    public string LoadBrainHeader()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "avg152T1_LR_nifti.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName = null;
        WorldManager.Instance.worldSettings.sparseVoxels = true;
        return LoadHeaderOnly();
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
    }

    private string LoadHeaderOnly()
    {
        // Use Steaming Assets folder to load the file
        string niftiFilePath = Path.Combine(Application.streamingAssetsPath, WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);

        Debug.Log("Header datafile name: " + WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);
        Debug.Log("Loading header: " + niftiFilePath);
        loader = DataLoaderUtils.LoadDataFile(0);
        if (niftiFile == null)
        {
            niftiFile = (Nifti.NET.Nifti)loader.GetHeader();
        }

        string retString = "<br>Dimensions: x(" + niftiFile.Dimensions[0] + ")/ y(" + niftiFile.Dimensions[1] + ")/ z(" + niftiFile.Dimensions[2] + ") <br>Filename: " + WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName;
        Debug.Log("RETSTRING: " + retString);

        return retString;
    }

    private void LoadAFile(bool hasSegmentLayers, int voxelOmissionThreshold)
    {
        // Use Steaming Assets folder to load the file
        string niftiFilePath = Path.Combine(Application.streamingAssetsPath, WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);

        // Load in one voxel model, currently support types are .txt and .nii files.
        // If more types need supporting, will need additional SourceData loader implementations.
        // Note, currently the getHeader method returns either a Nifti.NET.Nifti or string[] as appropriate.
        Debug.Log("Last file loaded: " + fileLastLoaded);
        Debug.Log("Settings datafile name: " + WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);
        //if (!fileLastLoaded.Equals(WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName))
        //{
            fileLastLoaded = WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName;
            Debug.Log("Loading file: " + fileLastLoaded);
            loader = DataLoaderUtils.LoadDataFile(voxelOmissionThreshold);
            WorldManager.Instance.voxelDictionary = loader.LoadSourceData(WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath);
        //}

        WorldManager.Instance.worldSettings.maxWidthX = loader.X;
        WorldManager.Instance.worldSettings.maxHeightY = loader.Y;
        WorldManager.Instance.worldSettings.maxDepthZ = loader.Z;

        if (WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName != null)
        {
            Debug.Log("Loading Voxel Segment Definition File: " + WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName);
            // Use Steaming Assets folder to load the file
            string voxelSegmentDefinitionFilePath = Path.Combine(Application.streamingAssetsPath, WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataSegmentFileName);

            WorldManager.Instance.voxelDictionary = loader.LoadVoxelSegmentDefinitionFileExtra(voxelSegmentDefinitionFilePath);
        }

        DetermineInitialCameraLocation();

        Debug.Log("Loaded world dimensions: " + WorldManager.Instance.worldSettings.maxWidthX + ", " + WorldManager.Instance.worldSettings.maxHeightY + ", " + WorldManager.Instance.worldSettings.maxDepthZ);
    }

    //Determines the initial camera position based on the world dimensions

    private void DetermineInitialCameraLocation()
    {
        Vector3Int minCorner = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
        Vector3Int maxCorner = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
        
        foreach (var key in WorldManager.Instance.voxelChunks.Keys)
        {
            minCorner = Vector3Int.Min(minCorner, key);
            maxCorner = Vector3Int.Max(maxCorner, key);
        }
        Vector3Int centerChunkPosition = (minCorner + maxCorner) / 2;
        
        WorldManager.Instance.worldSettings.intialCameraPosition = new Vector3Int(centerChunkPosition.x, centerChunkPosition.y + 10, centerChunkPosition.z - 100);
        Debug.Log("Camera will be pointed at:" + WorldManager.Instance.worldSettings.intialCameraPosition);
    }

    public void LoadZoom()
    {
        SceneManager.LoadScene("Zooming"); //The blood cell scene
    }

    public void BackButton()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "";
        SceneManager.LoadScene("Model");
    }

    public void BackToWorldButton()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "";
        SceneManager.LoadScene("World");
    }

    public void BackToMenuButton()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "";
        SceneManager.LoadScene("MainMenu");
    }
}
