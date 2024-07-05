using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    [HideInInspector]
    public string voxelFileFormat = "nii";

    public void LoadNextScene()
    {
        SceneManager.LoadScene("World");
    }

    public void LoadHeartFile()
    {
        // WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "ircad_e01_liver.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "JHU-WhiteMatter-labels-2mm.nii";
        LoadAFile(false);
        //Use Steaming Assets folder to load the file
        //oldway string niftiFilePath = Path.Combine(Application.streamingAssetsPath, "JHU-WhiteMatter-labels-2mm.nii");
        // string niftiFilePath = "Assets/Resources/JHU-WhiteMatter-labels-2mm.nii"; 
        //oldway SourceDataLoader.OpenNiftiFile(niftiFilePath);
        //oldway Nifti.NET.Nifti niftiFile = SourceDataLoader.GetHeader();
        //oldway Debug.Log("Description, " + System.Text.Encoding.Default.GetString(niftiFile.Header.descrip));
        //oldway Debug.Log("Dimensions, " + niftiFile.Dimensions[0] + ", " + niftiFile.Dimensions[1] + ", " + niftiFile.Dimensions[2]);
        //oldway Debug.Log("Filename, " + niftiFilePath);
    }

    public void LoadLRFile()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "avg152T1_LR_nifti.nii";
        LoadAFile(false);
        //oldway string niftiFilePath = Path.Combine(Application.streamingAssetsPath, "avg152T1_LR_nifti.nii");
        // string niftiFilePath = "Assets/Resources/avg152T1_LR_nifti.nii"; 
        //oldway SourceDataLoader.OpenNiftiFile(niftiFilePath);
        //oldway Nifti.NET.Nifti niftiFile = SourceDataLoader.GetHeader();
        //oldway Debug.Log("Description, " + System.Text.Encoding.Default.GetString(niftiFile.Header.descrip));
        //oldway Debug.Log("Dimensions, " + niftiFile.Dimensions[0] + ", " + niftiFile.Dimensions[1] + ", " + niftiFile.Dimensions[2]);
        //oldway Debug.Log("Filename, " + niftiFilePath);
    }

    public void LoadSegmentData()
    {
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName = "rkT1.nii";
        WorldManager.Instance.voxelMeshConfigurationSettings.voxelSegmentLayers = new string[] { "rkT2.nii" };
        LoadAFile(true);
    }

    private void LoadAFile(bool hasSegmentLayers)
    {
        Debug.Log("Loading file: " + WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);
        //Use Steaming Assets folder to load the file
        string niftiFilePath = Path.Combine(Application.streamingAssetsPath, WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFileName);

        // Load in one voxel model, currently support types are .txt and .nii files.
        // If more types need supporting, will need additional SourceData loader implementations.
        // Note, currently the getHeader method returns either a Nifti.NET.Nifti or string[] as appropriate.
        ASourceDataLoader loader = DataLoaderUtils.LoadDataFile();        
        WorldManager.Instance.sourceData = loader.LoadSourceData(WorldManager.Instance.voxelMeshConfigurationSettings.voxelDataFilePath);

        Nifti.NET.Nifti niftiFile = (Nifti.NET.Nifti)loader.GetHeader(); // SourceDataLoader.GetHeader();
        Debug.Log("Description, " + System.Text.Encoding.Default.GetString(niftiFile.Header.descrip));
        Debug.Log("Dimensions, " + niftiFile.Dimensions[0] + ", " + niftiFile.Dimensions[1] + ", " + niftiFile.Dimensions[2]);
        Debug.Log("Filename, " + niftiFilePath);

        WorldManager.Instance.worldSettings.maxWidthX = loader.widthX;
        WorldManager.Instance.worldSettings.maxHeightY = loader.heightY;
        WorldManager.Instance.worldSettings.maxDepthZ = loader.depthZ;

        Debug.Log("Loaded world dimensions: " + WorldManager.WorldSettings.maxWidthX + ", " + WorldManager.WorldSettings.maxHeightY + ", " + WorldManager.WorldSettings.maxDepthZ);
        Debug.Log("Loaded world dimensions2: " + WorldManager.Instance.worldSettings.maxWidthX + ", " + WorldManager.Instance.worldSettings.maxHeightY + ", " + WorldManager.Instance.worldSettings.maxDepthZ);

        Debug.Log("Random voxel color 0: " + WorldManager.Instance.sourceData[20, 20, 20].getColourRGBLayer(0));
        if (hasSegmentLayers)
        {
            int segmentLayer = 1;
            foreach (string nextSegmentFile in WorldManager.Instance.voxelMeshConfigurationSettings.voxelSegmentLayers)
            {
                WorldManager.Instance.sourceData = loader.LoadSegmentData(ref WorldManager.Instance.sourceData, segmentLayer++, nextSegmentFile);
            }        
            voxelFileFormat = DataLoaderUtils.GetDataFileFormat();
            Debug.Log("Random voxel color 1: " + WorldManager.Instance.sourceData[20, 20, 20].getColourRGBLayer(1));
        }

        // Load in one voxel model, currently support types are .txt and .nii files.
        // If more types need supporting, will need additional SourceData loader implementations.
        // Note, currently the getHeader method returns either a Nifti.NET.Nifti or string[] as appropriate.
        //ASourceDataLoader loader = DataLoaderUtils.LoadDataFile();
        //sourceData = loader.LoadSourceData(voxelMeshConfigurationSettings.voxelDataFilePath);

        //sourceData = SourceDataLoader.LoadSourceData();        
    }

    public void LoadZoom()
    {
        SceneManager.LoadScene("Zooming");
    }

    public void BackButton()
    {
        SceneManager.LoadScene("Model");
    }
}
